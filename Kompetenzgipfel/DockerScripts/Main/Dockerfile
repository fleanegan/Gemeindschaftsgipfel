# This is a multi-stage build so we'll start with a Node image
FROM node:latest AS node_base
RUN echo "NODE Version:" && node --version
RUN echo "NPM Version:" && npm --version

# Now create a .NET image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
RUN dotnet tool install --global dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools

# Copy the node  into the .NET image
COPY --from=node_base . .

# The default app directory for Linux and .NET
# this is not the same as our project's /app
WORKDIR /App
RUN mkdir ./ClientApp

COPY ["Kompetenzgipfel.csproj", "."]
COPY ["ClientApp/package.json", "./ClientApp/"]
RUN dotnet restore "Kompetenzgipfel.csproj" 
WORKDIR ./ClientApp
RUN npm install
WORKDIR ..

# Copy everything - both frontend and backend
COPY . ./

# change into the server directory we need to run a few commands
WORKDIR .

# Build and publish a release
WORKDIR ./ClientApp
RUN npm run build
WORKDIR ..
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && \
    apt-get install -y sqlite3

WORKDIR /App
COPY --from=build-env /App/out .
COPY ./.env .
EXPOSE 8080

RUN sqlite3 database.db < migrations.sql
ENTRYPOINT ["dotnet", "Kompetenzgipfel.dll"]