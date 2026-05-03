using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IPaymentService
{
    Task<InitializePaymentResponse> InitializePaymentAsync(string userId, InitializePaymentRequest request);
    Task<VerifyPaymentResponse> VerifyPaymentAsync(string reference);
    Task<bool> HandleWebhookAsync(string payload, string signature);
}