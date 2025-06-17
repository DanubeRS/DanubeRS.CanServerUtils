FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /app
COPY ["cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj", "cli/DanubeRS.CANServer.CLI/"]
COPY ["lib/DanubeRS.CANServer.Lib/DanubeRS.CANServer.Lib.csproj", "lib/DanubeRS.CANServer.Lib/"]
RUN dotnet restore "./cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj" -a $TARGETARCH
COPY . ./
RUN dotnet build "./cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DanubeRS.CANServer.CLI.dll"]
