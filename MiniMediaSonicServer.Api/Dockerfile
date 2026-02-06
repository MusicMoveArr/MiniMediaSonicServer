FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Install ffmpeg, fpcalc and ca-certs
USER root
RUN apt-get update
RUN apt-get install -y ffmpeg libchromaprint-tools
RUN apt-get install -y ca-certificates
RUN update-ca-certificates

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MiniMediaSonicServer.Api/MiniMediaSonicServer.Api.csproj", "MiniMediaSonicServer.Api/"]
COPY ["MiniMediaSonicServer.Application/MiniMediaSonicServer.Application.csproj", "MiniMediaSonicServer.Application/"]
RUN dotnet restore "MiniMediaSonicServer.Api/MiniMediaSonicServer.Api.csproj"
COPY . .
WORKDIR "/src/MiniMediaSonicServer.Api"
RUN dotnet build "./MiniMediaSonicServer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MiniMediaSonicServer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniMediaSonicServer.Api.dll"]
