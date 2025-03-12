FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blocktrust.CredentialBadges.DocsWeb/Blocktrust.CredentialBadges.DocsWeb.csproj", "Blocktrust.CredentialBadges.DocsWeb/"]
RUN dotnet restore "Blocktrust.CredentialBadges.DocsWeb/Blocktrust.CredentialBadges.DocsWeb.csproj"
COPY . .
WORKDIR "/src/Blocktrust.CredentialBadges.DocsWeb"
RUN dotnet build "Blocktrust.CredentialBadges.DocsWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Blocktrust.CredentialBadges.DocsWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blocktrust.CredentialBadges.DocsWeb.dll"]
