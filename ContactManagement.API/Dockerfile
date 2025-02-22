# Use the official .NET image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["ContactManagement.sln", "."]
COPY ["ContactManagement.API/ContactManagement.API.csproj", "ContactManagement.API/"]
COPY ["ContactManagement.DAL/ContactManagement.DAL.csproj", "ContactManagement.DAL/"]
COPY ["ContactManagement.BLL/ContactManagement.BLL.csproj", "ContactManagement.BLL/"]
COPY ["ContactManagement.Models/ContactManagement.Models.csproj", "ContactManagement.Models/"]
RUN dotnet restore "ContactManagement.API/ContactManagement.API.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src/ContactManagement.API"
RUN dotnet build "ContactManagement.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ContactManagement.API.csproj" -c Release -o /app/publish

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ContactManagement.API.dll"]