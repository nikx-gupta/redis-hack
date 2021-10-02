FROM mcr.microsoft.com/dotnet/sdk:5.0.201 as build

RUN mkdir -p src
WORKDIR /src
COPY src .

WORKDIR /
COPY ServiceRedis.sln .

RUN dotnet restore && dotnet publish -o /publish
FROM mcr.microsoft.com/dotnet/aspnet:5.0 as final
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT [ "dotnet", "Hack.Service.RedisDb.dll" ]


