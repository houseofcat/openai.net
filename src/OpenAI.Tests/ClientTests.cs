using OpenAI.Clients.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace OpenAI.Tests;

public class ClientTests : IClassFixture<OpenAIClientFixture>
{
    private readonly OpenAIClientFixture _fixture;

    public ClientTests(
        OpenAIClientFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.Output = output;
    }

    [Fact]
    public async Task GetModelsAsync()
    {
        // Act
        var modelsResponse = await _fixture.Client.GetModelsAsync();

        // Assert
        Assert.NotNull(modelsResponse);
        Assert.Equal("list", modelsResponse.Object);
        Assert.NotEmpty(modelsResponse.Models);
    }

    [Fact]
    public async Task GetModelAsync()
    {
        // Arrange
        var modelId = "davinci-instruct-beta";

        // Act
        var modelResponse = await _fixture.Client.GetModelAsync(modelId);

        // Assert
        Assert.NotNull(modelResponse);
        Assert.Equal("model", modelResponse.Object);
        Assert.Equal(modelId, modelResponse.Root);
        Assert.NotEmpty(modelResponse.Permissions);
    }

    [Fact]
    public async Task PostCreateCompletionAsync()
    {
        // Arrange
        var testPrompt = "Say this is a test";
        var modelId = "text-davinci-002";
        var request = new OpenAICompletionRequest
        {
            ModelId = modelId,
            MaxTokens = 6,
            Temperature = 0,
            TopP = 1,
            NCompletions = 1,
            Stream = false,
            LogProbabilities = null,
            Echo = true
        };

        request.Prompts.Add(testPrompt);
        request.Stops = new List<string>
        {
            "\n"
        };

        // Act
        var response = await _fixture.Client.PostCompletionAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("text_completion", response.Object);
        Assert.Equal(modelId, response.ModelId);
        Assert.NotEmpty(response.Choices);
        Assert.Contains(testPrompt, response.Choices.ElementAt(0).Text);
        Assert.NotNull(response.Usage);
    }
}