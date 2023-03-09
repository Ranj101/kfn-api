FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY src/KfnApi/KfnApi.csproj KfnApi/
RUN dotnet restore KfnApi/KfnApi.csproj

COPY src .

WORKDIR /src/KfnApi

RUN dotnet build KfnApi.csproj --no-restore -c Release

FROM build as publish
RUN dotnet publish KfnApi.csproj --no-build -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet","KfnApi.dll" ]
