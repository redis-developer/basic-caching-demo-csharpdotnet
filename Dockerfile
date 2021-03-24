FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 443

ENV PORT "5000"
ENV REDIS_ENDPOINT_URL "Redis server URI"
ENV REDIS_PASSWORD "Password to the server"

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "BasicRedisCacingExampleInDotNetCore/BasicRedisCacingExampleInDotNetCore.csproj"

WORKDIR "/src/BasicRedisCacingExampleInDotNetCore"
RUN dotnet build "BasicRedisCacingExampleInDotNetCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BasicRedisCacingExampleInDotNetCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/BasicRedisCacingExampleInDotNetCore/ClientApp/dist ./ClientApp/dist

ENTRYPOINT ["dotnet", "BasicRedisCacingExampleInDotNetCore.dll"]