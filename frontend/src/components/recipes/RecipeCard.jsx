import { Bookmark, BookmarkCheck } from 'lucide-react';

export function RecipeCard({ recipe, saved, onToggleSaved }) {
  return (
    <article className="recipe-card">
      <div className="recipe-card-header">
        <div>
          <strong>{recipe.name}</strong>
          <span>{recipe.mealType}</span>
        </div>
        {onToggleSaved && (
          <button type="button" className="icon-button" aria-label="Guardar receta" onClick={() => onToggleSaved(recipe)}>
            {saved ? <BookmarkCheck size={18} /> : <Bookmark size={18} />}
          </button>
        )}
      </div>
      <p>{recipe.description}</p>
      <div className="metrics">
        <span>{Number(recipe.calories).toFixed(0)} kcal</span>
        <span>{Number(recipe.proteinGrams).toFixed(1)} g proteina</span>
        <span>{recipe.isGlutenFree ? 'Sin gluten' : 'Con gluten'}</span>
        <span>{recipe.isDiabeticFriendly ? 'Diabetico ok' : 'Control glucosa'}</span>
      </div>
    </article>
  );
}
