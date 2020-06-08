FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["RegistrationService.Web/RegistrationService.Web.csproj", "RegistrationService.Web/"]
COPY ["RegistrationService.Infrastructure/RegistrationService.Infrastructure.csproj", "RegistrationService.Infrastructure/"]
COPY ["RegistrationService.SharedKernel/RegistrationService.SharedKernel.csproj", "RegistrationService.SharedKernel/"]
COPY ["RegistrationService.Core/RegistrationService.Core.csproj", "RegistrationService.Core/"]
RUN dotnet restore "RegistrationService.Web/RegistrationService.Web.csproj"
COPY . .
WORKDIR "/src/RegistrationService.Web"
RUN dotnet build "RegistrationService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegistrationService.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegistrationService.Web.dll"]