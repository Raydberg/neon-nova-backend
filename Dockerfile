# ────────────────────────────────────────────────────────────────────────────
# Stage 1: stripe-cli image (para tunelizar webhooks)
FROM stripe/stripe-cli:latest AS stripecli

# ────────────────────────────────────────────────────────────────────────────
# Stage 2: Compila tu aplicación .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia y restaura proyectos
COPY ["NeonNovaApp/NeonNovaApp.csproj", "NeonNovaApp/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Intrastructure/Intrastructure.csproj", "Intrastructure/"]
RUN dotnet restore "NeonNovaApp/NeonNovaApp.csproj"

# Copia todo el código y publica en modo Release
COPY . .
WORKDIR "/src/NeonNovaApp"
RUN dotnet publish "NeonNovaApp.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ────────────────────────────────────────────────────────────────────────────
# Stage 3: Imagen final combinada
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# 1) Copia la app .NET ya compilada
COPY --from=build /app/publish .

# 2) Copia el binario de stripe-cli desde su ruta real
COPY --from=stripecli /bin/stripe /usr/local/bin/stripe

# Exponer el puerto interno HTTP
EXPOSE 8080

# Variables de entorno para Kestrel y producción
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

# ENTRYPOINT: arranca Stripe CLI en background y luego tu app .NET
ENTRYPOINT ["/bin/sh", "-c", "\
  stripe listen \
    --api-key $STRIPE_SECRET_KEY \
    --forward-to http://localhost:8080/api/checkout/webhook & \
  dotnet NeonNovaApp.dll\
"]
