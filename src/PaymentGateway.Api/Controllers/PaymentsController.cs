using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Application.Dto;
using PaymentGateway.Api.Application.Services;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProcessPaymentResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment([FromBody] PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _paymentService.ProcessPayment(request);

        return result.PaymentStatus == PaymentStatus.Rejected.ToString()
            ? new BadRequestObjectResult(new { result.PaymentStatus, result.ErrorMessage })
            : CreatedAtAction(nameof(GetPaymentById), new { id = result.Payment.Id }, new { result.Payment, result.PaymentStatus });
    }

    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        var status = payment.Status.ToString();
        return payment switch
        {
            GetPaymentNullResponse => NotFound(new { Message = "Payment not found" }),
            _ => Ok(new { payment, status})
        };
    }
}