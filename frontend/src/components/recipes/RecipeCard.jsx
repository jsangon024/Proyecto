import { Bookmark, BookmarkCheck, Edit3, Trash2 } from 'lucide-react';

export function RecipeCard({ recipe, saved, onToggleSaved, onOpen, canManage, onEdit, onDelete }) {
  return (
    <article className="recipe-card clickable-card" onClick={() => onOpen?.(recipe)}>
      <div className="recipe-card-header">
        <div className="item-title">
          <strong>{recipe.name}</strong>
          <span>{recipe.mealType} · {recipe.isGlobal ? 'Comun' : 'Propia'}</span>
        </div>
        {onToggleSaved && (
          <div className="row-actions compact-actions">
            <button
              type="button"
              className="icon-button"
              aria-label="Guardar receta"
              onClick={(event) => {
                event.stopPropagation();
                onToggleSaved(recipe);
              }}
            >
              {saved ? <BookmarkCheck size={18} /> : <Bookmark size={18} />}
            </button>
            {canManage && (
              <>
                <button
                  type="button"
                  className="icon-button"
                  aria-label="Editar receta"
                  onClick={(event) => {
                    event.stopPropagation();
                    onEdit(recipe);
                  }}
                >
                  <Edit3 size={16} />
                </button>
                <button
                  type="button"
                  className="icon-button danger"
                  aria-label="Eliminar receta"
                  onClick={(event) => {
                    event.stopPropagation();
                    onDelete(recipe);
                  }}
                >
                  <Trash2 size={16} />
                </button>
              </>
            )}
          </div>
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
