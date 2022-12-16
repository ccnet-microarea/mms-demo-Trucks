FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["mms-demo-Trucks/mms-demo-Trucks.csproj", "mms-demo-Trucks/"]
RUN dotnet restore "mms-demo-Trucks/mms-demo-Trucks.csproj"
COPY . .
WORKDIR "/src/mms-demo-Trucks"
RUN dotnet build "mms-demo-Trucks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mms-demo-Trucks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mms-demo-Trucks.dll"]
