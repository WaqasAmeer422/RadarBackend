# 1. Use SDK 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 2. Copy the Solution file
COPY VisualSoft.Surveillance.Radar.sln ./

# 3. Find and copy all .csproj files while keeping their folder structure
# This command finds every .csproj and puts it in the right spot
COPY **/*.csproj ./
RUN dotnet restore

# 4. Copy everything else
COPY . .

# 5. Move into the actual API project folder to publish
# Note the double folder name based on your screenshot
WORKDIR "/src/VisualSoft.Surveillance.Radar.Api/VisualSoft.Surveillance.Radar.Api"
RUN dotnet publish -c Release -o /app/out

# 6. Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "VisualSoft.Surveillance.Radar.Api.dll"]