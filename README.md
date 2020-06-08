# DotNetCore.WebSockets
This is a sample on how to work with .NET Core and WebSockets. The sample is about a software licensing process, where the end customer submits a license request to a RESTful API via POST, and retrieves it via GET. The License is signed by a 3rd party software that connects to the current solution via WebSockets. When connected, the solution sends the License Request and receives it back signed.

## Features

 - RESTful API
	 - Accepting a POST with a license request
	 - Accepting long polling with a GET request for retrieving a signed license
- WebSocket
	 - A a queue will be used to send License Requests one by one, when the WebSocket client is connected
	 - WebSocket will handle incoming requests from a middleware
- Testing
	- Basic Unit testing for demo persistence layers
	- Basic Integration for REST API and WebSocket
