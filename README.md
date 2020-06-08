# DotNetCore.WebSockets
This is a sample on how to work with .NET Core and WebSockets. The sample is about a software licensing process, where the end customer submits a license request to a RESTful API via POST, and retrieves it via GET. The License is signed by a 3rd party software that connects to the current solution via WebSockets. When connected, the solution sends the License Request and receives it back signed.

## Features

 - RESTful API
	 - Accepting POST requests with a [LicenseRequestModel](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Web/Models/Request/LicenseRequestModel.cs). As per REST principles, returns a [LicenseResponseModel](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Web/Models/Response/LicenseResponseModel.cs)) and a Location header to retrieve the resource.
	 - Accepting GET requests for the newly created resource that returns a [LicenseResponseModel](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Web/Models/Response/LicenseResponseModel.cs), which contains (or not yet) the Signed License. 
	 - The state of the License (signed or not yet) is singaled with the property [Status](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/ef6419e24474b3ee0c37d4ee10c97ed733f56abd/src/RegistrationService.Web/Models/Response/LicenseResponseModel.cs#L13).
- WebSocket
	 - A [queue](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Infrastructure/Data/DemoQueueService.cs) is used to send License Requests, one by one, when the WebSocket client is connected.
	 - A middleware ([LicenseSignatureGeneratorMiddleware](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Core/Middlewares/LicenseSignatureGeneratorMiddleware.cs)) is handling WebSocket connections.
	- The WebSocket middlware is dequeueing a message to send and upon receiving a valid message it's storing in using a [storage service](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Infrastructure/Data/DemoStorageService.cs)	 	 
	 - The WebSocket messages are exchanged with a [LicenseMessage](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Core/Entities/LicenseMessage.cs)
	 - Allowed Origins is in [AppSettings](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Web/appsettings.Development.json) (currently allowing all).
- Testing
	- Basic [Unit Testing](https://github.com/georgekosmidis/AspNetCore.WebSockets/tree/master/tests/RegistrationService.UnitTests/Infrastructure/Data) for demo persistence layers
	- Basic [Integration Testing](https://github.com/georgekosmidis/AspNetCore.WebSockets/tree/master/tests/RegistrationService.IntegrationTests) for REST API and WebSocket
- Azure
	- Using Azure DevOps to [Build](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/azure-pipelines-master.yml), [Test](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/azure-pipelines-test.yml) and [Deploy](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/azure-pipelines-release.yml). Azure DevOps Project is public and availble [here](https://dev.azure.com/georgekosmidis/AspNetCore.WebSockets).
	- Image is pushed in Azure Container Registry
	- App is runnig in Azure App Service a the address http://aspnetcore-websockets.azurewebsites.net/ (no SSL installed currently).
- Sample Page ([index.html](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/master/src/RegistrationService.Web/wwwroot/index.html))
	- Left column is an end customer sending a License Request (fields are prefilled with data)
	- Right column is the 3rd party app (a License Signature App) waiting to connect to the WebSocket
	- Usage
		- Click 'Submit Data' on the left (only one initially). Long polling starts (every 3 secs), waiting for a signature ([Status](https://github.com/georgekosmidis/AspNetCore.WebSockets/blob/ef6419e24474b3ee0c37d4ee10c97ed733f56abd/src/RegistrationService.Web/Models/Response/LicenseResponseModel.cs#L13): 30).
		- Click 'Connect' on the right. Upon connecting retrieves all messages awaiting.
		- Click 'Send' on the right (a signed license is already prefilled).
		- Long polling stops, signature arrived.

	
