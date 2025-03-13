#################### ATTENTION
# This file has to be run from the command line of the /Project folder
# (the folder that contains "blocktrust.CredentialBadges" and "DidPrismResolverClient")
#
# Example build command:
#   docker build -f blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web/Dockerfile \
#       -t credentialbadgesweb .
#
# Explanation:
#   We copy each .csproj individually to preserve folder structure in the Docker cache,
#   then restore, then copy the rest of the source files.

# ---------------------------------------------
# Stage 1: Build & Publish
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# 1) Copy *.csproj files first, preserving subfolder structure
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web/Blocktrust.CredentialBadges.Web.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web/
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Core/Blocktrust.CredentialBadges.Core.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Core/
COPY blocktrust.CredentialBadges/Blocktrust.CredentialBadges.OpenBadges/Blocktrust.CredentialBadges.OpenBadges.csproj \
     blocktrust.CredentialBadges/Blocktrust.CredentialBadges.OpenBadges/
COPY DidPrismResolverClient/DidPrismResolverClient.csproj DidPrismResolverClient/

# 2) Restore using the main *.csproj
RUN dotnet restore "blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web/Blocktrust.CredentialBadges.Web.csproj"

# 3) Copy remaining source code
COPY blocktrust.CredentialBadges blocktrust.CredentialBadges
COPY DidPrismResolverClient DidPrismResolverClient

# 4) Build
WORKDIR /src/blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web
RUN dotnet build "Blocktrust.CredentialBadges.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ---------------------------------------------
# Stage 2: Publish
# ---------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/blocktrust.CredentialBadges/Blocktrust.CredentialBadges.Web
RUN dotnet publish "Blocktrust.CredentialBadges.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ---------------------------------------------
# Stage 3: Final runtime image
# ---------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Optional: You can switch user if you have a specific UID in your environment:
# ARG APP_UID=1000
# USER $APP_UID

WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports if needed (these match your Kestrel config in Program.cs)
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Blocktrust.CredentialBadges.Web.dll"]
