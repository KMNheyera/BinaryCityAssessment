namespace BC.ClientManager.Test.Tests.ClientTests
{
    public class GetClientByIdAsyncTests : ClientManagerTestBase
    {
        [Fact]
        public async Task GetClientByIdAsync_Should_Return_Client_When_Valid_Id()
        {
            //n
            Guid id = Guid.NewGuid();


            // Arrange
            var createResponse = await CreateClientAsync($"test-{id}");
            var clientId = createResponse.Payload!.ClientId;
            // Act
            var response = await GetClientByIdAsync(clientId);
            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.NotNull(response.Payload);
            Assert.Equal(clientId, response.Payload.ClientId);
            Assert.Equal($"test-{id}", response.Payload.Name);
        }
    }
}
