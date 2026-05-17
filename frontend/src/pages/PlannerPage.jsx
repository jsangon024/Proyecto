import { CalendarDays, ClipboardList, PackageCheck, ShoppingBasket, Target, Trash2 } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { inventoryApi } from '../api/inventoryApi.js';
import { plansApi } from '../api/plansApi.js';
import { userApi } from '../api/userApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Modal } from '../components/ui/Modal.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { SearchBox } from '../components/ui/SearchBox.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';
import { filterByText } from '../utils/search.js';

const today = new Date();

export function PlannerPage({ onNavigate }) {
  const { token, user, updateUser } = useAuth();
  const { showToast } = useToast();
  const [plan, setPlan] = useState(null);
  const [plans, setPlans] = useState([]);
  const [shoppingList, setShoppingList] = useState([]);
  const [weeks, setWeeks] = useState([]);
  const [selectedWeeks, setSelectedWeeks] = useState([]);
  const [planSearch, setPlanSearch] = useState('');
  const [shoppingSearch, setShoppingSearch] = useState('');
  const [plansModalOpen, setPlansModalOpen] = useState(false);
  const [shoppingModalOpen, setShoppingModalOpen] = useState(false);
  const [loadingPlans, setLoadingPlans] = useState(true);
  const [plansError, setPlansError] = useState('');
  const [form, setForm] = useState({ year: today.getFullYear(), month: today.getMonth() + 1 });
  const [goals, setGoals] = useState({
    dailyCalories: user.goals.dailyCalories,
    minimumProteinGrams: user.goals.minimumProteinGrams,
    excludedIngredients: user.goals.excludedIngredients ?? [],
  });
  const filteredPlans = useMemo(
    () => filterByText(plans, planSearch, (savedPlan) => `${savedPlan.month}/${savedPlan.year} ${savedPlan.year}-${savedPlan.month} ${new Date(savedPlan.createdAtUtc).toLocaleDateString()}`),
    [plans, planSearch],
  );
  const filteredShoppingList = useMemo(
    () => filterByText(shoppingList, shoppingSearch, (item) => `${item.name} ${item.quantity} ${item.unit}`),
    [shoppingList, shoppingSearch],
  );

  useEffect(() => {
    loadSavedPlans();
  }, [token]);

  async function loadSavedPlans() {
    setLoadingPlans(true);
    setPlansError('');
    try {
      const savedPlans = await plansApi.list(token);
      setPlans(savedPlans);
      if (savedPlans.length > 0) {
        await selectPlan(savedPlans[0]);
      }
    } catch (caught) {
      setPlansError(caught.message);
    } finally {
      setLoadingPlans(false);
    }
  }

  async function selectPlan(selectedPlan) {
    setPlan(selectedPlan);
    const nextWeeks = buildWeeks(selectedPlan.days);
    const allWeekNumbers = nextWeeks.map((week) => week.number);
    setWeeks(nextWeeks);
    setSelectedWeeks(allWeekNumbers);
    setShoppingList(await plansApi.shoppingList(
      token,
      [selectedPlan.id],
      selectedPlan.id,
      nextWeeks.flatMap((week) => week.dates),
    ));
  }

  async function toggleWeek(weekNumber) {
    if (!plan) {
      return;
    }

    const nextSelectedWeeks = selectedWeeks.includes(weekNumber)
      ? selectedWeeks.filter((number) => number !== weekNumber)
      : [...selectedWeeks, weekNumber].sort((left, right) => left - right);

    setSelectedWeeks(nextSelectedWeeks);
    const selectedDates = weeks
      .filter((week) => nextSelectedWeeks.includes(week.number))
      .flatMap((week) => week.dates);

    setShoppingList(await plansApi.shoppingList(token, [plan.id], plan.id, selectedDates));
  }

  async function saveGoals(event) {
    event.preventDefault();
    const updatedUser = await userApi.updateGoals(token, {
      dailyCalories: Number(goals.dailyCalories),
      minimumProteinGrams: Number(goals.minimumProteinGrams),
      excludedIngredients: goals.excludedIngredients,
    });
    updateUser(updatedUser);
    showToast('Objetivos actualizados');
  }

  async function generate(event) {
    event.preventDefault();
    const nextPlan = await plansApi.generate(token, Number(form.year), Number(form.month));
    const nextPlans = [nextPlan, ...plans.filter((savedPlan) => savedPlan.id !== nextPlan.id)];
    setPlans(nextPlans);
    await selectPlan(nextPlan);
    showToast('Plan mensual generado');
  }

  async function deletePlan(planToDelete) {
    if (!confirmDelete(`el plan de ${planToDelete.month}/${planToDelete.year}`)) {
      return;
    }

    await plansApi.remove(token, planToDelete.id);
    const remainingPlans = plans.filter((savedPlan) => savedPlan.id !== planToDelete.id);
    setPlans(remainingPlans);

    if (plan?.id === planToDelete.id) {
      if (remainingPlans.length > 0) {
        await selectPlan(remainingPlans[0]);
      } else {
        setPlan(null);
        setShoppingList([]);
        setWeeks([]);
        setSelectedWeeks([]);
      }
    }

    showToast('Plan eliminado');
  }

  async function moveShoppingListToInventory() {
    if (shoppingList.length === 0) {
      showToast('La lista de compra esta vacia');
      return;
    }

    await inventoryApi.purchaseList(token, shoppingList.map((item) => ({
      ingredientId: item.ingredientId,
      quantity: item.quantity,
      unit: item.unit,
    })));
    showToast('Compra anadida al inventario');
    if (plan) {
      const selectedDates = weeks
        .filter((week) => selectedWeeks.includes(week.number))
        .flatMap((week) => week.dates);
      setShoppingList(await plansApi.shoppingList(token, [plan.id], plan.id, selectedDates));
    }
  }

  async function completeDay(day) {
    if (!plan || day.isCompleted) {
      return;
    }

    const updatedPlan = await plansApi.completeDay(token, plan.id, day.date);
    const updatedPlans = plans.map((savedPlan) => (savedPlan.id === updatedPlan.id ? updatedPlan : savedPlan));
    setPlan(updatedPlan);
    setPlans(updatedPlans);

    const selectedDates = weeks
      .filter((week) => selectedWeeks.includes(week.number))
      .flatMap((week) => week.dates);
    setShoppingList(await plansApi.shoppingList(token, [updatedPlan.id], updatedPlan.id, selectedDates));
    showToast('Dia completado e inventario actualizado');
  }

  return (
    <section className="content-grid">
      <Panel title="Objetivos del plan" icon={<Target size={18} />}>
        <form className="stack" onSubmit={saveGoals}>
          <div className="form-grid">
            <Field label="Calorias diarias">
              <input
                type="number"
                min="1"
                value={goals.dailyCalories}
                onChange={(event) => setGoals({ ...goals, dailyCalories: event.target.value })}
              />
            </Field>
            <Field label="Proteina diaria">
              <input
                type="number"
                min="0"
                value={goals.minimumProteinGrams}
                onChange={(event) => setGoals({ ...goals, minimumProteinGrams: event.target.value })}
              />
            </Field>
          </div>
          <button className="primary">Guardar objetivos</button>
        </form>
      </Panel>

      <Panel title="Generar plan" icon={<CalendarDays size={18} />}>
        <form className="stack" onSubmit={generate}>
          <div className="form-grid">
            <Field label="Mes">
              <input type="number" min="1" max="12" value={form.month} onChange={(event) => setForm({ ...form, month: event.target.value })} />
            </Field>
            <Field label="Ano">
              <input type="number" min="2024" value={form.year} onChange={(event) => setForm({ ...form, year: event.target.value })} />
            </Field>
          </div>
          <button className="primary">Generar</button>
        </form>
      </Panel>

      <Panel title="Gestion del plan" icon={<ClipboardList size={18} />} className="wide">
        {plansError && <div className="notice">{plansError}</div>}
        <div className="planner-actions">
          <button type="button" className="action-tile" onClick={() => setPlansModalOpen(true)}>
            <CalendarDays size={22} />
            <span>Planes guardados</span>
            <strong>{plans.length}</strong>
          </button>
          <button type="button" className="action-tile" onClick={() => setShoppingModalOpen(true)}>
            <ShoppingBasket size={22} />
            <span>Lista de compra</span>
            <strong>{shoppingList.length}</strong>
          </button>
        </div>
      </Panel>

      <Panel title="Calendario" className="wide">
        {!plan && !loadingPlans && (
          <div className="empty-state">
            <strong>No hay plan guardado</strong>
            <span>Genera un plan mensual para que quede asociado a tu usuario.</span>
          </div>
        )}
        <div className="calendar-grid">
          {plan?.days?.map((day) => (
            <div className={`day-card ${day.isCompleted ? 'completed-day' : ''}`} key={day.date}>
              <strong>{day.date}</strong>
              {day.meals.map((meal) => (
                <button
                  type="button"
                  className="recipe-link"
                  key={`${day.date}-${meal.mealType}`}
                  onClick={() => onNavigate({ page: 'recipeDetail', recipeId: meal.recipeId })}
                >
                  {meal.recipeName}
                </button>
              ))}
              <button type="button" className="complete-day-button" disabled={day.isCompleted} onClick={() => completeDay(day)}>
                {day.isCompleted ? 'Completado' : 'Terminar Dia'}
              </button>
            </div>
          ))}
        </div>
      </Panel>

      {plansModalOpen && (
        <Modal title="Planes guardados" onClose={() => setPlansModalOpen(false)}>
          {loadingPlans && <div className="empty-state">Cargando planes...</div>}
          {plansError && <div className="notice">{plansError}</div>}
          <SearchBox value={planSearch} onChange={setPlanSearch} placeholder="Buscar por mes, ano o fecha..." />
          <div className="data-list">
            {filteredPlans.map((savedPlan) => (
              <div className={`plan-row ${plan?.id === savedPlan.id ? 'active' : ''}`} key={savedPlan.id}>
                <button
                  type="button"
                  className="plan-select"
                  onClick={async () => {
                    await selectPlan(savedPlan);
                    setPlansModalOpen(false);
                  }}
                >
                  <strong>{savedPlan.month}/{savedPlan.year}</strong>
                  <span>{savedPlan.days.length} dias</span>
                  <span>{new Date(savedPlan.createdAtUtc).toLocaleDateString()}</span>
                </button>
                <button type="button" className="icon-button danger" aria-label="Eliminar plan" onClick={() => deletePlan(savedPlan)}>
                  <Trash2 size={16} />
                </button>
              </div>
            ))}
          </div>
        </Modal>
      )}

      {shoppingModalOpen && (
        <Modal title="Lista de compra" onClose={() => setShoppingModalOpen(false)}>
          <SearchBox value={shoppingSearch} onChange={setShoppingSearch} placeholder="Buscar ingredientes de la compra..." />
          <div className="week-filter">
            {weeks.map((week) => (
              <button
                type="button"
                key={week.number}
                className={selectedWeeks.includes(week.number) ? 'active' : ''}
                onClick={() => toggleWeek(week.number)}
              >
                Semana {week.number}
              </button>
            ))}
          </div>
          <div className="data-list modal-list">
            {filteredShoppingList.map((item) => (
              <div className="data-row compact-data-row" key={`${item.ingredientId}-${item.unit}`}>
                <strong>{item.name}</strong>
                <span>{item.quantity} {item.unit}</span>
              </div>
            ))}
          </div>
          <button type="button" className="primary full-width" onClick={moveShoppingListToInventory}>
            <PackageCheck size={18} />
            Pasar compra al inventario
          </button>
        </Modal>
      )}
    </section>
  );
}

function buildWeeks(days) {
  const sortedDays = [...days].sort((left, right) => left.date.localeCompare(right.date));
  const weeks = [];

  for (let index = 0; index < sortedDays.length; index += 7) {
    const weekDays = sortedDays.slice(index, index + 7);
    weeks.push({
      number: weeks.length + 1,
      label: `${weekDays[0].date} - ${weekDays[weekDays.length - 1].date}`,
      dates: weekDays.map((day) => day.date),
    });
  }

  return weeks;
}
