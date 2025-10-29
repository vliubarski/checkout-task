using PaymentGateway.Api.Application.Services;
using PaymentGateway.Api.Application.Validators;
using PaymentGateway.Api.Infrastructure.Gateways;
using PaymentGateway.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddHttpClient<IBankService, BankService>();

builder.Services.AddTransient<IPaymentValidator, PaymentValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
