using System.Threading.Tasks;
using Xunit.Abstractions;

namespace OpenAI.Tests
{
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
             var modelResponse = await _fixture.Client.GetModelsAsync();
        }
    }
}