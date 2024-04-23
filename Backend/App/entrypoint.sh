#!/bin/bash

touch data/database.db
# Check if migrations have already been applied
if sqlite3 data/database.db ".tables" | grep "__EFMigrationsHistory"; then
  echo "Migrations already applied."
else
  echo "Applying migrations..."
  sqlite3 data/database.db < migrations.sql
fi

# Start the ASP.NET Core application
dotnet Gemeinschaftsgipfel.dll
