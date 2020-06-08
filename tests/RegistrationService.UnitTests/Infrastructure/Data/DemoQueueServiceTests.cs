using RegistrationService.Core.Entities;
using RegistrationService.Services;
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
            var quequeService = new DemoQueueService<LicenseMessage>();

            //Act
            quequeService.Enqueue(new LicenseMessage
            {
                ContactPerson = "George"
            });
            var result = quequeService.TryDequeue(out var licenseMessage);

            //Assert
            Assert.True(result);
            Assert.Equal("George", licenseMessage.ContactPerson);

            //Act
            result = quequeService.TryDequeue(out var licenseMessage2);

            //Assert
            Assert.False(result);
            Assert.Null(licenseMessage2);
        }
    }
}
