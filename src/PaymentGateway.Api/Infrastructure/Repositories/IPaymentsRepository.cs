using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Infrastructure.Repositories;

public interface IPaymentsRepository
{
    void Add(Payment payment);
    Task<Payment?> Get(Guid id);
}