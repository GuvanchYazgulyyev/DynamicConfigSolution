## Temel ASP.NET runtime imajı
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#EXPOSE 5000
#
## SDK içeren build imajı
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#
## Projeyi kopyala ve bağımlılıkları yükle
#COPY ConfigWebApi/ConfigWebApi.csproj ./ConfigWebApi/
#RUN dotnet restore "./ConfigWebApi/ConfigWebApi.csproj"
#
## Kalan tüm dosyaları kopyala
#COPY . .
#
## Çalışma dizinini belirle
#WORKDIR "/src/ConfigWebApi"
#
## Projeyi build et
#RUN dotnet build -c Release -o /app/build
#
## Yayın (Publish) et
#FROM build AS publish
#RUN dotnet publish -c Release -o /app/publish
#
## Final image
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#
## Uygulamayı başlat
#ENTRYPOINT ["dotnet", "ConfigWebApi.dll"]
#



# Temel imajı belirle
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Uygulama dosyalarını kopyala ve build et
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ConfigWebApi/ConfigWebApi.csproj", "ConfigWebApi/"]
RUN dotnet restore "ConfigWebApi/ConfigWebApi.csproj"
COPY . .
WORKDIR "/src/ConfigWebApi"
RUN dotnet build "ConfigWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigWebApi.dll"]




