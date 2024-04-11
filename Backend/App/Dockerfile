FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
RUN dotnet tool install --global dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools

WORKDIR /App

COPY ["Gemeinschaftsgipfel.csproj", "."]
RUN dotnet restore "Gemeinschaftsgipfel.csproj" 

COPY . ./

WORKDIR .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && \
    apt-get install -y sqlite3

WORKDIR /App
COPY --from=build-env /App/out .
COPY entrypoint.sh .

# Set execute permission on entrypoint script
RUN chmod +x entrypoint.sh

EXPOSE 8080

# Use entrypoint script to handle migration check and execution
ENTRYPOINT ["./entrypoint.sh"]
