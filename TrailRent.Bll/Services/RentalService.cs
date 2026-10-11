using System.Data;
using AutoMapper;
using TrailRent.Bll.Contracts;
using TrailRent.Dal.UnitOfWork;
using TrailRent.Domain.Entities;
using TrailRent.Domain.Exceptions;

namespace TrailRent.Bll.Services;

public sealed class RentalService(IUnitOfWork unitOfWork, IMapper mapper) : IRentalService
{
    public async Task<IReadOnlyList<RentalDto>> GetAllAsync(CancellationToken ct = default) =>
        mapper.Map<IReadOnlyList<RentalDto>>(await unitOfWork.Rentals.GetAllAsync(ct));

    public async Task<RentalDto> GetByIdAsync(long id, CancellationToken ct = default) =>
        mapper.Map<RentalDto>(await FindAsync(id, ct));

    public async Task<RentalDto> CreateAsync(CreateRentalRequest input, CancellationToken ct = default)
    {
        if (input.EndDate <= input.StartDate) throw new DomainValidationException("End date must be after start date.");
        if (await unitOfWork.Customers.GetByIdAsync(input.CustomerId, ct) is null)
            throw new NotFoundException($"Customer {input.CustomerId} was not found.");

        var days = input.EndDate.DayNumber - input.StartDate.DayNumber;
        var items = new List<RentalItem>();
        foreach (var requestItem in input.Items)
        {
            var equipment = await unitOfWork.Equipment.GetRentableByIdAsync(requestItem.EquipmentId, ct)
                ?? throw new NotFoundException($"Equipment {requestItem.EquipmentId} was not found or is unavailable.");
            if (equipment.AvailableUnits < requestItem.Quantity)
                throw new BusinessConflictException($"Equipment {equipment.Id} has insufficient available units.");
            items.Add(new RentalItem { EquipmentId = equipment.Id, EquipmentName = equipment.Name,
                DailyRate = equipment.DailyRate, Quantity = requestItem.Quantity });
        }

        var rental = new Rental { CustomerId = input.CustomerId, StartDate = input.StartDate, EndDate = input.EndDate,
            PickupLocation = input.PickupLocation, Status = RentalStatuses.Draft,
            TotalAmount = items.Sum(i => i.DailyRate * i.Quantity * days), Items = items };

        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            rental.Id = await unitOfWork.Rentals.AddAsync(rental, ct);
            await unitOfWork.Rentals.AddItemsAsync(rental.Id, items, ct);
            await unitOfWork.CommitAsync(ct);
            return mapper.Map<RentalDto>(rental);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task ConfirmAsync(long id, CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            var rental = await unitOfWork.Rentals.GetWithItemsAsync(id, ct)
                ?? throw new NotFoundException($"Rental {id} was not found.");
            if (rental.Items.Count == 0) throw new BusinessConflictException("A rental must contain an item.");
            await unitOfWork.Rentals.ConfirmAsync(id, ct);
            foreach (var item in rental.Items)
                await unitOfWork.Equipment.DecreaseAvailabilityAsync(item.EquipmentId, item.Quantity, ct);
            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        if (!await unitOfWork.Rentals.DeleteAsync(id, ct))
            throw new NotFoundException($"Rental {id} was not found.");
    }

    private async Task<Rental> FindAsync(long id, CancellationToken ct) =>
        await unitOfWork.Rentals.GetWithItemsAsync(id, ct) ?? throw new NotFoundException($"Rental {id} was not found.");
}
