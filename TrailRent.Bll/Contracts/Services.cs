namespace TrailRent.Bll.Contracts;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<CustomerDto> CreateAsync(UpsertCustomerRequest input, CancellationToken ct = default);
    Task UpdateAsync(long id, UpsertCustomerRequest input, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}

public interface IRentalService
{
    Task<IReadOnlyList<RentalDto>> GetAllAsync(CancellationToken ct = default);
    Task<RentalDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<RentalDto> CreateAsync(CreateRentalRequest input, CancellationToken ct = default);
    Task ConfirmAsync(long id, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
