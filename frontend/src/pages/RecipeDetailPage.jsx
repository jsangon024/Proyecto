import { ArrowLeft, BookOpen, Carrot } from 'lucide-react';
import { recipesApi } from '../api/recipesApi.js';
import { Panel } from '../components/ui/Panel.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';

export function RecipeDetailPage({ recipeId, onNavigate }) {
  const { token } = useAuth();
  const recipe = useResource(() => recipesApi.get(token, recipeId), [token, recipeId]);

  if (recipe.loading) {
    return <div className="empty-state">Cargando receta...</div>;
  }

  if (recipe.error) {
    return <div className="notice">{recipe.error}</div>;
  }

  if (!recipe.data) {
    return <div className="empty-state">Receta no encontrada</div>;
  }

  return (
    <section className="recipe-detail-page">
      <Panel title={recipe.data.name} className="wide">
        <button type="button" className="secondary-button back-button" onClick={() => onNavigate('recipes')}>
          <ArrowLeft size={16} />
          Volver
        </button>
        <p className="muted">{recipe.data.description}</p>
        <div className="metrics">
          <span>{recipe.data.mealType}</span>
          <span>{recipe.data.servings} raciones</span>
          <span>{Number(recipe.data.calories).toFixed(0)} kcal</span>
          <span>{Number(recipe.data.proteinGrams).toFixed(1)} g proteina</span>
          <span>{recipe.data.isGlobal ? 'Comun' : 'Propia'}</span>
        </div>
      </Panel>

      <Panel title="Ingredientes" icon={<Carrot size={18} />} className="ingredients-summary-panel">
        <div className="ingredients-mini-list">
          {recipe.data.ingredients.map((ingredient) => (
            <div className="ingredient-mini-row" key={`${ingredient.ingredientId}-${ingredient.unit}`}>
              <strong>{ingredient.name}</strong>
              <span>{ingredient.quantity} {ingredient.unit}</span>
            </div>
          ))}
        </div>
      </Panel>

      <Panel title="Pasos" icon={<BookOpen size={18} />} className="steps-panel">
        <ol className="step-list">
          {recipe.data.steps.map((step) => (
            <li key={step.stepNumber}>{step.description}</li>
          ))}
        </ol>
      </Panel>
    </section>
  );
}
