using BC.ClientManager.BL.Dto;

namespace BC.ClientManager.Test.Tests.ClientTests
{
    public class UpdateClientAsyncTest : ClientManagerTestBase
    {
        [Fact]
        public async Task UpdateClientAsync_ShouldReturnSuccess_WhenClientExists()
        {
            // Arrange
            var createResponse = await CreateClientAsync("Test Client");
            Assert.True(createResponse.Success);
            var clientId = createResponse.Payload!.ClientId;
            var updateDto = new UpdateClientDto
            {
                ClientId = clientId,
                NewName = "Updated Test Client"
            };
            // Act
            var updateResponse = await UpdateClientAsync(updateDto);
            // Assert
            Assert.True(updateResponse.Success);
            // Verify the update
            var getResponse = await GetClientByIdAsync(clientId);
            Assert.True(getResponse.Success);
            Assert.NotNull(getResponse.Payload);
            Assert.Equal("Updated Test Client", getResponse.Payload.Name);
        }
    }
}
