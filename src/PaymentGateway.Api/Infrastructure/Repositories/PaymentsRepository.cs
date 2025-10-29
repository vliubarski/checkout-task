using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Infrastructure.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly List<Payment> _payments = new();

    public void Add(Payment payment)
    {
        _payments.Add(payment);
    }

    public Task<Payment?> Get(Guid id)
    {
        return Task.FromResult(_payments.FirstOrDefault(p => p.Id == id));
    }
}