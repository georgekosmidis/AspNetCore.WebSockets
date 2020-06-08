using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RegistrationService.Web.Models.Request;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace RegistrationService.Web.IntegrationTests
{
    //Demo tests, leaves garbages in the persistence layer, needs cleaning
    // Since the persistene layer lifetime is limited (app lifetime), it is ok for now
    // More tests (negative scenarios) are necessary
    //Negative tests and testing the actual data are also needed, this is not proper TDD
    public class IntegrationTesting : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public IntegrationTesting(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/Licensing")]
        public async Task PostLicenseRequest_WhenAllFieldsCorrect_ShouldReturn201(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var licenseRequest = new LicenseRequestModel
            {
                CompanyName = "",
                ContactPerson = "George",
                Address = "Giesing",
                Email = "foo@bar.test",
                LicenseKey = "empty"
            };
            var content = new StringContent(JsonConvert.SerializeObject(licenseRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("api/Licensing")]
        public async Task GetLicenseRequest_WhenDataExists_ShouldReturnCorrectObject(string url)
        {

            // Arrange
            var client = _factory.CreateClient();
            var licenseRequest = new LicenseRequestModel
            {
                CompanyName = "",
                ContactPerson = "George",
                Address = "Giesing",
                Email = "foo@bar.test",
                LicenseKey = "empty"
            };
            var content = new StringContent(JsonConvert.SerializeObject(licenseRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);
            var resourse = response.Headers.Location;
            response = await client.GetAsync(resourse);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

        }

    }
}
