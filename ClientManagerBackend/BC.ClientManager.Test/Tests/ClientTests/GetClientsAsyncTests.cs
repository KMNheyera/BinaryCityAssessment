using BC.ClientManager.BL.Dto;

namespace BC.ClientManager.Test.Tests.ClientTests
{
    public class GetClientsAsyncTests : ClientManagerTestBase
    {
        [Fact]
        public async Task GetClientsAsync_ShouldReturnClients()
        {
            // Arrange
            var getTableDataDto = new GetTableDataDto
            {
                PageNumber = 1,
                PageSize = 10
            };
            // Act
            var response = await GetClientsAsync(getTableDataDto);
            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Payload);
        }
    }
}
