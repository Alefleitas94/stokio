using Stokio.Domain.Entities;

namespace Stokio.Application.Common.Interfaces;

public interface IInventoryService
{
    Task<int> GetStockOnHandAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);

    Task<StockMovement> RecordPurchaseAsync(
        int productId,
        int warehouseId,
        int quantity,
        decimal unitPrice,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task<StockMovement> RecordSaleAsync(
        int productId,
        int warehouseId,
        int quantity,
        decimal unitPrice,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task<StockMovement> RecordAdjustmentAsync(
        int productId,
        int warehouseId,
        int quantityDelta,
        decimal unitPrice = 0,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task<(StockMovement Out, StockMovement In)> RecordTransferAsync(
        int productId,
        int fromWarehouseId,
        int toWarehouseId,
        int quantity,
        decimal unitPrice = 0,
        string? notes = null,
        CancellationToken cancellationToken = default);
}
