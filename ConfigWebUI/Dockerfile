FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ConfigWebUI/ConfigWebUI.csproj", "ConfigWebUI/"]
RUN dotnet restore "ConfigWebUI/ConfigWebUI.csproj"
COPY . .
WORKDIR "/src/ConfigWebUI"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigWebUI.dll"]
