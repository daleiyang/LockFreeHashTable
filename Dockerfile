# Lock-free Build Stage
#FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS lock_free_webapi_build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS lock_free_webapi_build
WORKDIR /src
COPY ./WebApi WebApi
COPY ./CASHashTable CASHashTable
RUN dotnet restore "WebApi/WebApi.csproj"
RUN dotnet build "WebApi/WebApi.csproj" -c release -o /app/build/webapi
RUN dotnet publish "WebApi/WebApi.csproj" -c release -o /app/publish/webapi

# Lock-free Serve Stage
#FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS lock_free_webapi_serve
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS lock_free_webapi_serve
WORKDIR /app
COPY --from=lock_free_webapi_build /app/publish/webapi .
ENTRYPOINT ["dotnet", "WebApi.dll"]

# RPCServer Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS rpcserver_build
WORKDIR /src
COPY ./RPCServer RPCServer
RUN dotnet restore "RPCServer/RPCServer.csproj"
RUN dotnet build "RPCServer/RPCServer.csproj" -c release -o /app/build/rpcserver
RUN dotnet publish "RPCServer/RPCServer.csproj" -c release -o /app/publish/rpcserver

# RPCServer Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS rpcserver_serve
WORKDIR /app
COPY --from=rpcserver_build /app/publish/rpcserver .
ENTRYPOINT ["dotnet", "RPCServer.dll"]

# RPCClient Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS rpcclient_build
WORKDIR /src
COPY ./RPCClient RPCClient
RUN dotnet restore "RPCClient/RPCClient.csproj"
RUN dotnet build "RPCClient/RPCClient.csproj" -c release -o /app/build/rpcclient
RUN dotnet publish "RPCClient/RPCClient.csproj" -c release -o /app/publish/rpcclient

# RPCClient Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS rpcclient_serve
WORKDIR /app
COPY --from=rpcclient_build /app/publish/rpcclient .
ENTRYPOINT ["dotnet", "RPCClient.dll"]

# docker build --rm -t daleiyang/webapi:latest --target lock_free_webapi_serve .
# docker build --rm -t daleiyang/rpcserver:latest --target rpcserver_serve .
# docker build --rm -t daleiyang/rpcclient:latest --target rpcclient_serve .

# docker run --rm -p 5000:5000 -p 5001:5001 -e ASPNETCORE_HTTP_PORT=https://+:5001 -e ASPNETCORE_URLS=http://+:5000 daleiyang/webapi
# docker run -it --rm --name rabbitmq-container --network test -p 5672:5672 -p 15672:15672 rabbitmq:4.0-management-alpine
# docker run --rm --name rpcserver --network test daleiyang/rpcserver
# docker run --rm --name rpcclient --network test daleiyang/rpcclient

# docker network create test
# docker compose up --build
