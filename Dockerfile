# Stage 1: stripe-cli image (for tunneling webhooks)
FROM stripe/stripe-cli:latest AS stripecli

# Stage 2: build .NET app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["NeonNovaApp/NeonNovaApp.csproj", "NeonNovaApp/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Intrastructure/Intrastructure.csproj", "Intrastructure/"]
RUN dotnet restore "NeonNovaApp/NeonNovaApp.csproj"
COPY . .
WORKDIR "/src/NeonNovaApp"
RUN dotnet publish "NeonNovaApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: final image combining stripe-cli and .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy .NET app
COPY --from=build /app/publish .

# Copy stripe-cli binary into final image
COPY --from=stripecli /usr/local/bin/stripe /usr/local/bin/stripe

# Expose your app port (HTTP internal)
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

# Entrypoint: run stripe CLI in background and then the .NET app
ENTRYPOINT ["/bin/sh", "-c", \
  "stripe listen \
    --api-key $STRIPE_SECRET_KEY \
    --forward-to http://localhost:${PORT:-8080}/api/checkout/webhook & \" dotnet NeonNovaApp.dll"]
