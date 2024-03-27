FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
RUN dotnet tool install --global dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools

WORKDIR /App

COPY ["Kompetenzgipfel.csproj", "."]
RUN dotnet restore "Kompetenzgipfel.csproj" 

COPY . ./

WORKDIR .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && \
    apt-get install -y sqlite3

WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 8080

RUN sqlite3   database.db < migrations.sql
ENTRYPOINT ["dotnet", "Kompetenzgipfel.dll"]