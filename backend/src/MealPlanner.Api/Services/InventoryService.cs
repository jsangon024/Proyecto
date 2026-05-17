using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class InventoryService
{
    private readonly MealPlannerDbContext _db;

    public InventoryService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public IReadOnlyCollection<InventoryItemEntity> GetAll(UserEntity user)
    {
        return _db.InventoryItems
            .AsNoTracking()
            .Include(item => item.Ingredient)
            .Where(item => item.UserId == user.Id)
            .OrderBy(item => item.Ingredient!.Name)
            .ToArray();
    }

    public InventoryItemEntity? GetById(UserEntity user, Guid id)
    {
        return _db.InventoryItems
            .AsNoTracking()
            .Include(item => item.Ingredient)
            .FirstOrDefault(item => item.Id == id && item.UserId == user.Id);
    }

    public InventoryItemEntity Create(UserEntity user, InventoryCreateRequest request)
    {
        ValidateItem(request.IngredientId, request.Quantity, request.Unit);

        if (!_db.Ingredients.Any(ingredient => ingredient.Id == request.IngredientId))
        {
            throw new ValidationException("El ingrediente indicado no existe.");
        }

        var existing = _db.InventoryItems.FirstOrDefault(item =>
            item.UserId == user.Id &&
            item.IngredientId == request.IngredientId &&
            item.Unit == request.Unit.Trim().ToLowerInvariant());

        if (existing is not null)
        {
            existing.Quantity = request.Quantity;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            _db.SaveChanges();
            return GetById(user, existing.Id) ?? existing;
        }

        var item = new InventoryItemEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IngredientId = request.IngredientId,
            Quantity = request.Quantity,
            Unit = request.Unit.Trim().ToLowerInvariant(),
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.InventoryItems.Add(item);
        _db.SaveChanges();
        return GetById(user, item.Id) ?? item;
    }

    public InventoryItemEntity? Update(UserEntity user, Guid id, InventoryUpdateRequest request)
    {
        var item = _db.InventoryItems.FirstOrDefault(candidate => candidate.Id == id && candidate.UserId == user.Id);
        if (item is null)
        {
            return null;
        }

        ValidateItem(request.IngredientId, request.Quantity, request.Unit);

        if (!_db.Ingredients.Any(ingredient => ingredient.Id == request.IngredientId))
        {
            throw new ValidationException("El ingrediente indicado no existe.");
        }

        item.IngredientId = request.IngredientId;
        item.Quantity = request.Quantity;
        item.Unit = request.Unit.Trim().ToLowerInvariant();
        item.UpdatedAt = DateTimeOffset.UtcNow;

        _db.SaveChanges();
        return GetById(user, id);
    }

    public bool Delete(UserEntity user, Guid id)
    {
        var item = _db.InventoryItems.FirstOrDefault(candidate => candidate.Id == id && candidate.UserId == user.Id);
        if (item is null)
        {
            return false;
        }

        _db.InventoryItems.Remove(item);
        _db.SaveChanges();
        return true;
    }

    public IReadOnlyCollection<InventoryItemEntity> AddPurchaseList(UserEntity user, PurchaseListRequest request)
    {
        if (request.Items.Length == 0)
        {
            throw new ValidationException("La lista de compra no contiene ingredientes.");
        }

        foreach (var purchaseItem in request.Items)
        {
            ValidateItem(purchaseItem.IngredientId, purchaseItem.Quantity, purchaseItem.Unit);

            if (!_db.Ingredients.Any(ingredient => ingredient.Id == purchaseItem.IngredientId))
            {
                throw new ValidationException("Uno de los ingredientes de la lista no existe.");
            }

            var unit = purchaseItem.Unit.Trim().ToLowerInvariant();
            var existing = _db.InventoryItems.FirstOrDefault(item =>
                item.UserId == user.Id &&
                item.IngredientId == purchaseItem.IngredientId &&
                item.Unit == unit);

            if (existing is null)
            {
                _db.InventoryItems.Add(new InventoryItemEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    IngredientId = purchaseItem.IngredientId,
                    Quantity = purchaseItem.Quantity,
                    Unit = unit,
                    UpdatedAt = DateTimeOffset.UtcNow
                });
            }
            else
            {
                existing.Quantity += purchaseItem.Quantity;
                existing.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        _db.SaveChanges();
        return GetAll(user);
    }

    private static void ValidateItem(Guid ingredientId, decimal quantity, string unit)
    {
        if (ingredientId == Guid.Empty || string.IsNullOrWhiteSpace(unit) || quantity < 0)
        {
            throw new ValidationException("El ingrediente, la unidad y la cantidad son obligatorios.");
        }
    }
}
