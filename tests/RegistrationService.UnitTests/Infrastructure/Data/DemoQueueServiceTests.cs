using RegistrationService.Core.Messages;
using RegistrationService.Infrastructure.Implemantations;
using Xunit;

namespace RegistrationService.Infrastructure.Data.Tests
{
    public class DemoQueueServiceTests
    {
        //Just a demo, more tests are needed
        [Fact]
        public void BasicTest()
        {
            //Arrange
            var quequeService = new LicenseQueueService();

            //Act
            quequeService.Enqueue(new LicenseMessage
            {
                LicenseKey = "George"
            });
            var result = quequeService.TryDequeue(out var licenseMessage);

            //Assert
            Assert.True(result);
            Assert.Equal("George", licenseMessage.LicenseKey);

            //Act
            result = quequeService.TryDequeue(out var licenseMessage2);

            //Assert
            Assert.False(result);
            Assert.Null(licenseMessage2);
        }
    }
}
