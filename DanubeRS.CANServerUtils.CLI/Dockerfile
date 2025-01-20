FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DanubeRS.CANServerUtils.CLI/DanubeRS.CANServerUtils.CLI.csproj", "DanubeRS.CANServerUtils.CLI/"]
COPY ["DanubeRS.CanServerUtils.Library/DanubeRS.CanServerUtils.Library.csproj", "DanubeRS.CanServerUtils.Library/"]
RUN dotnet restore "DanubeRS.CANServerUtils.CLI/DanubeRS.CANServerUtils.CLI.csproj"
COPY . .
WORKDIR "/src/DanubeRS.CANServerUtils.CLI"
RUN dotnet build "DanubeRS.CANServerUtils.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DanubeRS.CANServerUtils.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DanubeRS.CANServerUtils.CLI.dll"]
