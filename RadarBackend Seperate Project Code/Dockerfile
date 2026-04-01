# 1. Use SDK 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 2. Copy the Solution file
COPY VisualSoft.Surveillance.Radar.sln ./

# 3. Copy each project file into its specific folder
# We must use the exact folder names from your screenshot
COPY VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api.csproj ./VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api/
COPY VisualSoft.Surveillance.Radar.Data/VisualSoft.Surveillance.Radar.Data.csproj ./VisualSoft.Surveillance.Radar.Data/
COPY VisualSoft.Surveillance.Radar.Domain/VisualSoft.Surveillance.Radar.Domain.csproj ./VisualSoft.Surveillance.Radar.Domain/
COPY VisualSoft.Surveillance.Radar.Services/VisualSoft.Surveillance.Radar.Services.csproj ./VisualSoft.Surveillance.Radar.Services/

# 4. Now restore using the Solution file specifically
RUN dotnet restore VisualSoft.Surveillance.Radar.sln

# 5. Copy everything else
COPY . .

# 6. Build and Publish
WORKDIR "/src/VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api"
RUN dotnet publish -c Release -o /app/out

# 7. Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "VisualSoft.Surveillance.Radar.Api.dll"]