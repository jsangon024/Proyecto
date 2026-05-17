using MealPlanner.Api.Data.Entities;

namespace MealPlanner.Api.Dtos;

public sealed record InventoryItemDto(Guid Id, Guid IngredientId, string IngredientName, decimal Quantity, string Unit)
{
    public static InventoryItemDto From(InventoryItemEntity item)
    {
        return new InventoryItemDto(item.Id, item.IngredientId, item.Ingredient?.Name ?? string.Empty, item.Quantity, item.Unit);
    }
}

public sealed record InventoryCreateRequest(Guid IngredientId, decimal Quantity, string Unit);

public sealed record InventoryUpdateRequest(Guid IngredientId, decimal Quantity, string Unit);

public sealed record PurchaseListRequest(PurchaseListItemRequest[] Items);

public sealed record PurchaseListItemRequest(Guid IngredientId, decimal Quantity, string Unit);
