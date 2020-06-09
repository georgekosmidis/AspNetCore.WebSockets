using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using RegistrationService.Web;
using RegistrationService.Web.Models.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RegistrationService.Core.IntegrationTests
{
    //Demo test, leaves garbages in the persistence layer, needs cleaning
    // Since the persistene layer lifetime is limited (app lifetime), it is ok for now
    // More tests (negative scenarios) are necessary
    public class IntegrationTesting
    {
        [Theory]
        [InlineData("/api/Licensing")]
        public async Task POSTLicenseRequestGETSignedLicense_WhenNewLicenseRequest_ShouldReturnignedLicense(string path)
        {
            //Arrange
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Development");
            var server = new TestServer(builder);

            //Arrange - WebSocket
            var wsClient = server.CreateWebSocketClient();
            var wsUri = new UriBuilder(server.BaseAddress)
            {
                Scheme = "ws",
                Path = "ws"
            }.Uri;

            //Arrange - API Call 
            var webClient = server.CreateClient();
            var licenseRequestContent = new StringContent(
                JsonConvert.SerializeObject(
                    new LicenseRequestModel
                    {
                        CompanyName = "",
                        ContactPerson = "George",
                        Address = "Giesing",
                        Email = "foo@bar.test",
                        LicenseKey = "empty"
                    }
                ), Encoding.UTF8, "application/json");

            //Act - API Call
            var licenseResourse = (await webClient.PostAsync(path, licenseRequestContent)).Headers.Location;

            //Act - WebSocket
            var websocket = await wsClient.ConnectAsync(wsUri, CancellationToken.None);
            var buffer = new byte[4096];
            var recvMessage = new List<byte>();
            var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            using (var cts = new CancellationTokenSource(1000))
            {
                //receive license request, "sign" it and send it back
                var recvResult = await websocket.ReceiveAsync(segment, cts.Token);
                recvMessage.AddRange(new ArraySegment<byte>(buffer, 0, recvResult.Count));

                //orchestrate "generator" response
                // alter george to george2 for Assert
                var sendText = Encoding.UTF8.GetString(recvMessage.ToArray())
                    .Replace("George", "George2");
                var sendBytes = Encoding.UTF8.GetBytes(sendText);
                await websocket.SendAsync(sendBytes, WebSocketMessageType.Text, true, cts.Token);
            }

            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            //Act - API Call
            var response = await webClient.GetAsync(licenseResourse);
            var body = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("George2", body);

        }

    }
}
