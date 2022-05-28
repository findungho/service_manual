#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServiceManual.Web/ServiceManual.Web.csproj", "ServiceManual.Web/"]
COPY ["ServiceManual.ApplicationCore/ServiceManual.ApplicationCore.csproj", "ServiceManual.ApplicationCore/"]
RUN dotnet restore "ServiceManual.Web/ServiceManual.Web.csproj"
COPY . .
WORKDIR "/src/ServiceManual.Web"
RUN dotnet build "ServiceManual.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceManual.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceManual.Web.dll"]