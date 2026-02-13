namespace BC.ClientManager.Test.Tests.ClientTests
{
    public class CreateClientAsyncTests : ClientManagerTestBase
    {
        [Fact]
        public async Task CreateClientAsync_ShouldReturnSuccess_WhenValidNameProvided()
        {
            // Arrange
            var clientName = "Test Client";
            // Act
            var response = await CreateClientAsync(clientName);
            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.NotNull(response.Payload);
            Assert.Equal(clientName, response.Payload.Name);
        }
    }
}
