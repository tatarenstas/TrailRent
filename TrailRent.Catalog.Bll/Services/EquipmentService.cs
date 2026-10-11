using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Bll.Dtos;
using TrailRent.Catalog.Dal.Repositories;
using TrailRent.Catalog.Dal.Specifications;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Bll.Services;

public sealed class EquipmentService(IUnitOfWork unitOfWork, IMapper mapper) : IEquipmentService
{
    public async Task<PagedResult<EquipmentListItemDto>> GetPageAsync(EquipmentQuery query, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var count = await unitOfWork.Equipment.CountAsync(new EquipmentFilterSpecification(query.Search, query.CategoryId, query.Rentable, query.Sort, 1, pageSize, false), cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
        var page = Math.Clamp(query.Page, 1, totalPages);
        var items = await unitOfWork.Equipment.ListAsync(new EquipmentFilterSpecification(query.Search, query.CategoryId, query.Rentable, query.Sort, page, pageSize), cancellationToken);
        return new(mapper.Map<IReadOnlyList<EquipmentListItemDto>>(items), count, page, pageSize, totalPages, page < totalPages, page > 1);
    }

    public async Task<EquipmentDetailsDto> GetDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        mapper.Map<EquipmentDetailsDto>(await unitOfWork.Equipment.GetWithDetailsAsync(id, cancellationToken) ?? throw new EntityNotFoundException("Equipment", id));

    public async Task<EquipmentDetailsDto> CreateAsync(CreateEquipmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken) is null) throw new EntityNotFoundException("EquipmentCategory", request.CategoryId);
        var entity = new Equipment(request.Sku, request.Name, request.DailyRate, request.AvailableUnits, request.CategoryId);
        await unitOfWork.Equipment.AddAsync(entity, cancellationToken);
        try { await unitOfWork.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException) { throw new ConflictException($"Equipment with SKU '{request.Sku}' already exists."); }
        return await GetDetailsAsync(entity.Id, cancellationToken);
    }

    public async Task ChangeDailyRateAsync(int id, ChangeDailyRateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await unitOfWork.Equipment.GetByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException("Equipment", id);
        entity.ChangeDailyRate(request.DailyRate);
        unitOfWork.Equipment.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveStockAsync(int id, StockChangeRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await unitOfWork.Equipment.GetByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException("Equipment", id);
        entity.RemoveFromStock(request.Units);
        unitOfWork.Equipment.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EquipmentListItemDto>> GetRentableAsync(CancellationToken cancellationToken = default) =>
        mapper.Map<IReadOnlyList<EquipmentListItemDto>>(await unitOfWork.Equipment.ListAsync(new RentableEquipmentSpecification(), cancellationToken));
}
