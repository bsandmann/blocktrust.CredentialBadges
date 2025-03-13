#################### ATTENTION
# This file must be run from the command line of the /Project folder
# (the folder that contains "blocktrust.CredentialBadges" and "DidPrismResolverClient")
#
# Example build command:
#   docker build -f blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder/Dockerfile -t credentialbadgesbuilder .
# ---------------------------------------------
# Stage 1: Build & Publish
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# 1) Copy the main .csproj files (plus any supporting .csproj)
#    so they can be restored individually (layer-cached)
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder/Blocktrust.CredentialBadges.Builder.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder/
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Core/Blocktrust.CredentialBadges.Core.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Core/
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.OpenBadges/Blocktrust.CredentialBadges.OpenBadges.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.OpenBadges/

# If the solution or sub-project references DidPrismResolverClient,
# we must copy it as well to avoid "Skipping project" errors:
COPY DidPrismResolverClient/DidPrismResolverClient.csproj DidPrismResolverClient/

# 2) Restore
RUN dotnet restore "blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder/Blocktrust.CredentialBadges.Builder.csproj"

# 3) Copy remaining source code
COPY blocktrust.CredentialBadges blocktrust.CredentialBadges
COPY DidPrismResolverClient DidPrismResolverClient

# 4) Build
WORKDIR /src/blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder
RUN dotnet build "Blocktrust.CredentialBadges.Builder.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ---------------------------------------------
# Stage 2: Publish
# ---------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Builder

RUN dotnet publish "Blocktrust.CredentialBadges.Builder.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ---------------------------------------------
# Stage 3: Final runtime image
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# If you need a specific non-root user, uncomment & adjust accordingly:
# ARG APP_UID=1000
# USER $APP_UID

WORKDIR /app
COPY --from=publish /app/publish .

# Expose the same ports you use in your Kestrel configuration
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Blocktrust.CredentialBadges.Builder.dll"]
