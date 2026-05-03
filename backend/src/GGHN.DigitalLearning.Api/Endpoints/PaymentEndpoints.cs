using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/payments").WithTags("Payments");

        group.MapPost("/initialize", async (InitializePaymentRequest request, IPaymentService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await service.InitializePaymentAsync(userId, request);
            return Results.Ok(result);
        })
        .WithSummary("Initialize a payment for a premium template")
        .RequireAuthorization();

        group.MapGet("/verify/{reference}", async (string reference, IPaymentService service) =>
        {
            var result = await service.VerifyPaymentAsync(reference);
            return Results.Ok(result);
        })
        .WithSummary("Verify payment status")
        .RequireAuthorization();

        group.MapPost("/webhook", async (HttpRequest request, IPaymentService service) =>
        {
            using var reader = new StreamReader(request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = request.Headers["x-paystack-signature"].FirstOrDefault() ?? "";

            var handled = await service.HandleWebhookAsync(payload, signature);
            return handled ? Results.Ok(new { message = "Webhook processed" }) : Results.BadRequest();
        })
        .WithSummary("Paystack webhook handler")
        .AllowAnonymous();

        return endpoints;
    }
}