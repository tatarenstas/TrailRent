using AutoMapper;
using TrailRent.Bll.Contracts;
using TrailRent.Dal.UnitOfWork;
using TrailRent.Domain.Entities;
using TrailRent.Domain.Exceptions;

namespace TrailRent.Bll.Services;

public sealed class CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default) =>
        mapper.Map<IReadOnlyList<CustomerDto>>(await unitOfWork.Customers.GetAllAsync(ct));

    public async Task<CustomerDto> GetByIdAsync(long id, CancellationToken ct = default) =>
        mapper.Map<CustomerDto>(await FindAsync(id, ct));

    public async Task<CustomerDto> CreateAsync(UpsertCustomerRequest input, CancellationToken ct = default)
    {
        var customer = mapper.Map<Customer>(input);
        customer.Id = await unitOfWork.Customers.AddAsync(customer, ct);
        return mapper.Map<CustomerDto>(customer);
    }

    public async Task UpdateAsync(long id, UpsertCustomerRequest input, CancellationToken ct = default)
    {
        var customer = mapper.Map<Customer>(input);
        customer.Id = id;
        if (!await unitOfWork.Customers.UpdateAsync(customer, ct))
            throw new NotFoundException($"Customer {id} was not found.");
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        if (!await unitOfWork.Customers.DeleteAsync(id, ct))
            throw new NotFoundException($"Customer {id} was not found.");
    }

    private async Task<Customer> FindAsync(long id, CancellationToken ct) =>
        await unitOfWork.Customers.GetByIdAsync(id, ct) ?? throw new NotFoundException($"Customer {id} was not found.");
}
