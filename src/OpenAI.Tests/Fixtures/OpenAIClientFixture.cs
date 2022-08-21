using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI.Clients;
using System;
using System.Net.Http;
using Xunit.Abstractions;

namespace OpenAI.Tests
{
    public class OpenAIClientFixture
    {
        public ITestOutputHelper Output;
        public OpenAIClient Client;

        public OpenAIClientFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            Client = new OpenAIClient(
                serviceProvider
                    .GetService<ILoggerFactory>()
                    .CreateLogger<OpenAIClient>(),
                new HttpClient
                {
                    BaseAddress = new Uri("https://api.openai.com"),
                },
                new OpenAIClientOptions
                {
                    BaseUrl = "https://api.openai.com",
                    ApiVersion = "v1",
                    ApiKey = "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"
                });
        }
    }
}