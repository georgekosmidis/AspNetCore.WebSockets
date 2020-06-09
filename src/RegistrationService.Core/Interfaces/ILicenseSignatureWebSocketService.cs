using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RegistrationService.Core.Interfaces
{
    public interface ILicenseSignatureWebSocketService
    {
        Task AcceptConnections(HttpContext context);
    }
}
