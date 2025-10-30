namespace PaymentGateway.Api.Application.Dto;

public record PaymentValidatorResult
(
    bool IsValid,

    string? ErrorMessage
);
