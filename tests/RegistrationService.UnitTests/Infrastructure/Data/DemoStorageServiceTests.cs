using RegistrationService.Core.Entities;
using RegistrationService.Infrastructure.Implementations;
using Xunit;

namespace RegistrationService.Infrastructure.Tests
{
    public class DemoStorageServiceTests
    {
        //Just a demo
        [Fact]
        public void BasicTest()
        {
            //Arrange
            var storageService = new LicenseStorageService();
            var licenseDataModel = new LicenseDataModel
            {
                ContactPerson = "George"
            };

            //Act
            storageService.AddOrUpdate(licenseDataModel);
            var result = storageService.Get(licenseDataModel.Id);

            //Assert
            Assert.Equal("George", result.ContactPerson);

            //Arrange
            licenseDataModel.ContactPerson = "George2";
            storageService.AddOrUpdate(licenseDataModel);
            result = storageService.Get(licenseDataModel.Id);

            //Assert
            Assert.Equal("George2", result.ContactPerson);

        }
    }
}
