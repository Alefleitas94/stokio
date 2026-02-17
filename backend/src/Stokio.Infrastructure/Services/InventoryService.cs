using Microsoft.EntityFrameworkCore;
using Stokio.Application.Common.Interfaces;
using Stokio.Domain.Entities;
using Stokio.Infrastructure.Persistence;

namespace Stokio.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentTenantService _currentTenantService;

    public InventoryService(ApplicationDbContext dbContext, ICurrentTenantService currentTenantService)
    {
        _dbContext = dbContext;
        _currentTenantService = currentTenantService;
    }

    public async Task<int> GetStockOnHandAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenantId();

        var sum = await _dbContext.StockMovements
            .AsNoTracking()
            .Where(sm => sm.TenantId == tenantId && sm.ProductId == productId && sm.WarehouseId == warehouseId)
            .Select(sm => (int?)sm.Quantity)
            .SumAsync(cancellationToken);

        return sum ?? 0;
    }

    public async Task<StockMovement> RecordPurchaseAsync(
        int productId,
        int warehouseId,
        int quantity,
        decimal unitPrice,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        RequirePositive(quantity, nameof(quantity));
        RequireNonNegative(unitPrice, nameof(unitPrice));

        var tenantId = RequireTenantId();
        await RequireProductAsync(tenantId, productId, cancellationToken);
        await RequireWarehouseAsync(tenantId, warehouseId, cancellationToken);

        var movement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = warehouseId,
            MovementType = MovementType.Purchase,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Notes = notes,
            RelatedWarehouseId = null
        };

        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return movement;
    }

    public async Task<StockMovement> RecordSaleAsync(
        int productId,
        int warehouseId,
        int quantity,
        decimal unitPrice,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        RequirePositive(quantity, nameof(quantity));
        RequireNonNegative(unitPrice, nameof(unitPrice));

        var tenantId = RequireTenantId();
        await RequireProductAsync(tenantId, productId, cancellationToken);
        await RequireWarehouseAsync(tenantId, warehouseId, cancellationToken);

        var delta = -quantity;
        await RequireSufficientStockAsync(tenantId, productId, warehouseId, delta, cancellationToken);

        var movement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = warehouseId,
            MovementType = MovementType.Sale,
            Quantity = delta,
            UnitPrice = unitPrice,
            Notes = notes,
            RelatedWarehouseId = null
        };

        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return movement;
    }

    public async Task<StockMovement> RecordAdjustmentAsync(
        int productId,
        int warehouseId,
        int quantityDelta,
        decimal unitPrice = 0,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        if (quantityDelta == 0)
        {
            throw new ArgumentException("quantityDelta must be non-zero.", nameof(quantityDelta));
        }

        RequireNonNegative(unitPrice, nameof(unitPrice));

        var tenantId = RequireTenantId();
        await RequireProductAsync(tenantId, productId, cancellationToken);
        await RequireWarehouseAsync(tenantId, warehouseId, cancellationToken);

        if (quantityDelta < 0)
        {
            await RequireSufficientStockAsync(tenantId, productId, warehouseId, quantityDelta, cancellationToken);
        }

        var movement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = warehouseId,
            MovementType = MovementType.Adjustment,
            Quantity = quantityDelta,
            UnitPrice = unitPrice,
            Notes = notes,
            RelatedWarehouseId = null
        };

        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return movement;
    }

    public async Task<(StockMovement Out, StockMovement In)> RecordTransferAsync(
        int productId,
        int fromWarehouseId,
        int toWarehouseId,
        int quantity,
        decimal unitPrice = 0,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        RequirePositive(quantity, nameof(quantity));
        RequireNonNegative(unitPrice, nameof(unitPrice));

        if (fromWarehouseId == toWarehouseId)
        {
            throw new ArgumentException("fromWarehouseId and toWarehouseId must be different.", nameof(toWarehouseId));
        }

        var tenantId = RequireTenantId();
        await RequireProductAsync(tenantId, productId, cancellationToken);
        await RequireWarehouseAsync(tenantId, fromWarehouseId, cancellationToken);
        await RequireWarehouseAsync(tenantId, toWarehouseId, cancellationToken);

        var deltaOut = -quantity;
        await RequireSufficientStockAsync(tenantId, productId, fromWarehouseId, deltaOut, cancellationToken);

        var outMovement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = fromWarehouseId,
            MovementType = MovementType.Transfer,
            Quantity = deltaOut,
            UnitPrice = unitPrice,
            Notes = notes,
            RelatedWarehouseId = toWarehouseId
        };

        var inMovement = new StockMovement
        {
            TenantId = tenantId,
            ProductId = productId,
            WarehouseId = toWarehouseId,
            MovementType = MovementType.Transfer,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Notes = notes,
            RelatedWarehouseId = fromWarehouseId
        };

        _dbContext.StockMovements.AddRange(outMovement, inMovement);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (outMovement, inMovement);
    }

    private int RequireTenantId()
    {
        var tenantId = _currentTenantService.TenantId;
        if (tenantId is null)
        {
            throw new InvalidOperationException("Tenant is not resolved for the current request.");
        }

        return tenantId.Value;
    }

    private static void RequirePositive(int value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"{paramName} must be positive.", paramName);
        }
    }

    private static void RequireNonNegative(decimal value, string paramName)
    {
        if (value < 0)
        {
            throw new ArgumentException($"{paramName} must be non-negative.", paramName);
        }
    }

    private async Task RequireProductAsync(int tenantId, int productId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == productId && p.TenantId == tenantId && p.IsActive, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException($"Product {productId} not found for tenant {tenantId}.");
        }
    }

    private async Task RequireWarehouseAsync(int tenantId, int warehouseId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Warehouses
            .AsNoTracking()
            .AnyAsync(w => w.Id == warehouseId && w.TenantId == tenantId && w.IsActive, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException($"Warehouse {warehouseId} not found for tenant {tenantId}.");
        }
    }

    private async Task RequireSufficientStockAsync(
        int tenantId,
        int productId,
        int warehouseId,
        int delta,
        CancellationToken cancellationToken)
    {
        if (delta >= 0)
        {
            return;
        }

        var onHand = await GetStockOnHandInternalAsync(tenantId, productId, warehouseId, cancellationToken);
        if (onHand + delta < 0)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for product {productId} in warehouse {warehouseId}. OnHand={onHand}, RequestedDelta={delta}.");
        }
    }

    private async Task<int> GetStockOnHandInternalAsync(
        int tenantId,
        int productId,
        int warehouseId,
        CancellationToken cancellationToken)
    {
        var sum = await _dbContext.StockMovements
            .AsNoTracking()
            .Where(sm => sm.TenantId == tenantId && sm.ProductId == productId && sm.WarehouseId == warehouseId)
            .Select(sm => (int?)sm.Quantity)
            .SumAsync(cancellationToken);

        return sum ?? 0;
    }
}
