using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private string SecretKey => _configuration["Paystack:SecretKey"] ?? throw new InvalidOperationException("Paystack SecretKey not configured");
    private string BaseUrl => _configuration["Paystack:BaseUrl"] ?? "https://api.paystack.co";

    public PaymentService(AppDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<InitializePaymentResponse> InitializePaymentAsync(string userId, InitializePaymentRequest request)
    {
        var template = await _context.Templates.FindAsync(request.TemplateId)
            ?? throw new KeyNotFoundException("Template not found");

        if (template.Tier != Domain.Enums.TemplateTier.Premium || template.Price == null)
            throw new InvalidOperationException("Template is not a premium template");

        var alreadyPurchased = await _context.PaymentTransactions
            .AnyAsync(pt => pt.UserId == userId && pt.TemplateId == request.TemplateId && pt.Status == "success");
        if (alreadyPurchased)
            throw new InvalidOperationException("Template already purchased");

        var reference = $"GGHN-{Guid.NewGuid():N}"[..20];
        var amountInKobo = (long)(template.Price.Value * 100);

        var transaction = new PaymentTransaction
        {
            UserId = userId,
            TemplateId = request.TemplateId,
            Amount = template.Price.Value,
            Currency = "NGN",
            Reference = reference,
            Status = "pending"
        };

        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        var client = _httpClientFactory.CreateClient();
        var payload = new
        {
            email = (await _context.Users.FindAsync(userId))?.Email ?? userId,
            amount = amountInKobo,
            reference,
            metadata = new { template_id = request.TemplateId.ToString(), user_id = userId }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        content.Headers.Add("Authorization", $"Bearer {SecretKey}");

        var response = await client.PostAsync($"{BaseUrl}/transaction/initialize", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            transaction.Status = "failed";
            transaction.PaystackResponse = responseBody;
            await _context.SaveChangesAsync();
            throw new InvalidOperationException($"Paystack initialization failed: {responseBody}");
        }

        transaction.PaystackResponse = responseBody;
        await _context.SaveChangesAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var data = doc.RootElement.GetProperty("data");
        var authorizationUrl = data.GetProperty("authorization_url").GetString()!;
        var paystackRef = data.GetProperty("reference").GetString()!;

        return new InitializePaymentResponse
        {
            AuthorizationUrl = authorizationUrl,
            Reference = paystackRef
        };
    }

    public async Task<VerifyPaymentResponse> VerifyPaymentAsync(string reference)
    {
        var transaction = await _context.PaymentTransactions
            .FirstOrDefaultAsync(pt => pt.Reference == reference)
            ?? throw new KeyNotFoundException("Transaction not found");

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/transaction/verify/{reference}");
        request.Headers.Add("Authorization", $"Bearer {SecretKey}");

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        transaction.PaystackResponse = responseBody;

        if (!response.IsSuccessStatusCode)
        {
            transaction.Status = "failed";
            await _context.SaveChangesAsync();
            return new VerifyPaymentResponse
            {
                Status = "failed",
                Reference = reference,
                Amount = transaction.Amount,
                TemplateId = transaction.TemplateId
            };
        }

        using var doc = JsonDocument.Parse(responseBody);
        var data = doc.RootElement.GetProperty("data");
        var status = data.GetProperty("status").GetString() ?? "unknown";

        if (status == "success")
        {
            transaction.Status = "success";
            await _context.SaveChangesAsync();

            return new VerifyPaymentResponse
            {
                Status = "success",
                Reference = reference,
                Amount = transaction.Amount,
                TemplateId = transaction.TemplateId,
                DownloadUrl = transaction.Template?.FileUrl
            };
        }

        transaction.Status = status;
        await _context.SaveChangesAsync();

        return new VerifyPaymentResponse
        {
            Status = status,
            Reference = reference,
            Amount = transaction.Amount,
            TemplateId = transaction.TemplateId
        };
    }

    public async Task<bool> HandleWebhookAsync(string payload, string signature)
    {
        var computedSignature = ComputeHmacSha512(SecretKey, payload);
        if (!computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase))
            return false;

        using var doc = JsonDocument.Parse(payload);
        var evt = doc.RootElement.GetProperty("event").GetString();
        var data = doc.RootElement.GetProperty("data");
        var reference = data.GetProperty("reference").GetString()!;

        var transaction = await _context.PaymentTransactions.FirstOrDefaultAsync(pt => pt.Reference == reference);
        if (transaction == null) return false;

        if (evt == "charge.success")
        {
            transaction.Status = "success";
            transaction.PaystackResponse = payload;
        }
        else if (evt == "charge.failed")
        {
            transaction.Status = "failed";
            transaction.PaystackResponse = payload;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static string ComputeHmacSha512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}