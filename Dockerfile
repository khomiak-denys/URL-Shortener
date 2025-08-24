FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
WORKDIR /app 


COPY *.sln . 
COPY src/URL_Shortener.API/*csproj src/URL_Shortener.API/
COPY src/URL_Shortener.Application/*csproj src/URL_Shortener.Application/
COPY src/URL_Shortener.Infrastructure/*csproj src/URL_Shortener.Infrastructure/
COPY src/URL_Shortener.Domain/*csproj src/URL_Shortener.Domain/


RUN dotnet restore

COPY . .

RUN dotnet publish src/URL_Shortener.API/*csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app 


COPY --from=build /app/publish .


EXPOSE 8080


ENTRYPOINT ["dotnet", "URL_Shortener.API.dll"]