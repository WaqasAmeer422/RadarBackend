# 1. Use SDK 10.0 for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 2. Copy the Solution and all specific Project files
COPY VisualSoft.Surveillance.Radar.sln ./
COPY VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api.csproj ./VisualSoft.Surveillance.Radar.Api/
COPY VisualSoft.Surveillance.Radar.Data/VisualSoft.Surveillance.Radar.Data.csproj ./VisualSoft.Surveillance.Radar.Data/
COPY VisualSoft.Surveillance.Radar.Domain/VisualSoft.Surveillance.Radar.Domain.csproj ./VisualSoft.Surveillance.Radar.Domain/
COPY VisualSoft.Surveillance.Radar.Services/VisualSoft.Surveillance.Radar.Services.csproj ./VisualSoft.Surveillance.Radar.Services/

# 3. Restore dependencies
RUN dotnet restore

# 4. Copy all source files
COPY . .

# 5. Build and Publish the API project
WORKDIR "/src/VisualSoft.Surveillance.Radar.Api"
RUN dotnet publish -c Release -o /app/out

# 6. Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Use the DLL name found in your Api folder
ENTRYPOINT ["dotnet", "VisualSoft.Surveillance.Radar.Api.dll"]