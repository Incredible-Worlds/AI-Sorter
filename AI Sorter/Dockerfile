FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release

FROM nginx:alpine
WORKDIR /var/www/web
COPY --from=build /app/bin/Release/net8.0/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80