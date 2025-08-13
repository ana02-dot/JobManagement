using System.Text.RegularExpressions;
using JobManagement.Application.Interfaces;
using JobManagement.Application.Services;
using Microsoft.Extensions.Logging;

namespace JobManagement.Infrastructure.Services;

public class GeorgianPersonalNumberVerificationService : IPersonalNumberVerificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeorgianPersonalNumberVerificationService> _logger;

    public GeorgianPersonalNumberVerificationService(HttpClient httpClient, ILogger<GeorgianPersonalNumberVerificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PersonalNumberVerificationResult> VerifyPersonalNumberAsync(string personalNumber)
    {
        try
        {
                // Validate format first (11 digits)
                if (!IsValidPersonalNumberFormat(personalNumber))
                {
                    return new PersonalNumberVerificationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Personal number must be 11 digits"
                    };
                }

                // For demo purposes, we'll simulate the API call
                // In production, you would integrate with the actual Georgian Civil Registry API
                // Example: https://rs.ge/api/verify-citizen
                
                var mockResponse = await SimulateGeorgianApiCall(personalNumber);
                
                if (mockResponse.IsValid)
                {
                    return new PersonalNumberVerificationResult
                    {
                        IsValid = true,
                        FirstName = mockResponse.FirstName,
                        LastName = mockResponse.LastName
                    };
                }

                return new PersonalNumberVerificationResult
                {
                    IsValid = false,
                    ErrorMessage = "Personal number not found in Georgian Civil Registry"
                };
        }
        catch (Exception ex)
        {
                _logger.LogError(ex, "Error verifying personal number: {PersonalNumber}", personalNumber);
                return new PersonalNumberVerificationResult
                {
                    IsValid = false,
                    ErrorMessage = "Service temporarily unavailable"
                }; 
        }
    }

        private bool IsValidPersonalNumberFormat(string personalNumber)
        {
            return !string.IsNullOrWhiteSpace(personalNumber) && 
                   Regex.IsMatch(personalNumber, @"^\d{11}$");
        }

        private async Task<MockGeorgianApiResponse> SimulateGeorgianApiCall(string personalNumber)
        {
            // Simulate API delay
            await Task.Delay(500);

            // Mock response based on personal number patterns
            // In production, this would be a real API call
            var mockData = new Dictionary<string, (string FirstName, string LastName)>
            {
                { "12345678901", ("გიორგი", "მელაძე") }, // Giorgi Meladze
                { "98765432109", ("ნინო", "კვარაცხელია") }, // Nino Kvaratskhelia  
                { "11111111111", ("ანა", "ჩაჩუა") }, // Ana Chachua
                { "22222222222", ("დავით", "ღვინიაშვილი") } // Davit Ghviniashvili
            };

            if (mockData.TryGetValue(personalNumber, out var person))
            {
                return new MockGeorgianApiResponse
                {
                    IsValid = true,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                };
            }

            return new MockGeorgianApiResponse { IsValid = false };
        }

        private class MockGeorgianApiResponse
        {
            public bool IsValid { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
        }
}