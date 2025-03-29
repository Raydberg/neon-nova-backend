# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["NeonNovaApp/NeonNovaApp.csproj", "NeonNovaApp/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Intrastructure/Intrastructure.csproj", "Intrastructure/"]
RUN dotnet restore "NeonNovaApp/NeonNovaApp.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/NeonNovaApp"
RUN dotnet build "NeonNovaApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NeonNovaApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "NeonNovaApp.dll"]