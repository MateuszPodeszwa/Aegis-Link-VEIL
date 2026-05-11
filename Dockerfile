FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Aegis-Link-VEIL.sln ./
COPY Aegis-Link-VEIL/Aegis-Link-VEIL.csproj Aegis-Link-VEIL/
COPY AegisLink.Server/AegisLink.Server.csproj AegisLink.Server/
COPY AegisLink.Shared/AegisLink.Shared.csproj AegisLink.Shared/
RUN dotnet restore Aegis-Link-VEIL.sln

COPY . .

RUN dotnet publish Aegis-Link-VEIL/Aegis-Link-VEIL.csproj -c Release -o /app/publish/client
RUN dotnet publish AegisLink.Server/AegisLink.Server.csproj -c Release -o /app/publish/server

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish/server ./
RUN rm -rf /app/wwwroot
COPY --from=build /app/publish/client/wwwroot /app/wwwroot

EXPOSE 10000
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet AegisLink.Server.dll"]
