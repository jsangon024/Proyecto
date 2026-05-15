import { CalendarDays, ShoppingBasket, Target, Trash2 } from 'lucide-react';
import { useEffect, useState } from 'react';
import { plansApi } from '../api/plansApi.js';
import { userApi } from '../api/userApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useToast } from '../hooks/useToast.jsx';

const today = new Date();

export function PlannerPage() {
  const { token, user, updateUser } = useAuth();
  const { showToast } = useToast();
  const [plan, setPlan] = useState(null);
  const [plans, setPlans] = useState([]);
  const [shoppingList, setShoppingList] = useState([]);
  const [loadingPlans, setLoadingPlans] = useState(true);
  const [plansError, setPlansError] = useState('');
  const [form, setForm] = useState({ year: today.getFullYear(), month: today.getMonth() + 1 });
  const [goals, setGoals] = useState({
    dailyCalories: user.goals.dailyCalories,
    minimumProteinGrams: user.goals.minimumProteinGrams,
    excludedIngredients: user.goals.excludedIngredients ?? [],
  });

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
    setShoppingList(await plansApi.shoppingList(token, selectedPlan.id));
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
    await selectPlan(nextPlan);
    setPlans([nextPlan, ...plans.filter((savedPlan) => savedPlan.id !== nextPlan.id)]);
    showToast('Plan mensual generado');
  }

  async function deletePlan(planToDelete) {
    const confirmed = window.confirm(`Eliminar el plan de ${planToDelete.month}/${planToDelete.year}?`);
    if (!confirmed) {
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
      }
    }

    showToast('Plan eliminado');
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

      <Panel title="Planes guardados">
        {loadingPlans && <div className="empty-state">Cargando planes...</div>}
        {plansError && <div className="notice">{plansError}</div>}
        <div className="data-list">
          {plans.map((savedPlan) => (
            <div className={`plan-row ${plan?.id === savedPlan.id ? 'active' : ''}`} key={savedPlan.id}>
              <button type="button" className="plan-select" onClick={() => selectPlan(savedPlan)}>
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
      </Panel>

      <Panel title="Lista de compra" icon={<ShoppingBasket size={18} />}>
        <div className="data-list">
          {shoppingList.map((item) => (
            <div className="data-row" key={`${item.ingredientId}-${item.unit}`}>
              <strong>{item.name}</strong>
              <span>{item.quantity} {item.unit}</span>
            </div>
          ))}
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
            <div className="day-card" key={day.date}>
              <strong>{day.date}</strong>
              {day.meals.map((meal) => (
                <span key={`${day.date}-${meal.mealType}`}>{meal.recipeName}</span>
              ))}
            </div>
          ))}
        </div>
      </Panel>
    </section>
  );
}
