FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blocktrust.CredentialBadges.Builder/Blocktrust.CredentialBadges.Builder.csproj", "Blocktrust.CredentialBadges.Builder/"]
COPY ["Blocktrust.CredentialBadges.Core/Blocktrust.CredentialBadges.Core.csproj", "Blocktrust.CredentialBadges.Core/"]
COPY ["Blocktrust.CredentialBadges.OpenBadges/Blocktrust.CredentialBadges.OpenBadges.csproj", "Blocktrust.CredentialBadges.OpenBadges/"]
RUN dotnet restore "Blocktrust.CredentialBadges.Builder/Blocktrust.CredentialBadges.Builder.csproj"
COPY . .
WORKDIR "/src/Blocktrust.CredentialBadges.Builder"
RUN dotnet build "Blocktrust.CredentialBadges.Builder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Blocktrust.CredentialBadges.Builder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blocktrust.CredentialBadges.Builder.dll"]
