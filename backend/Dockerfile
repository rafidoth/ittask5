FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN apt-get update && apt-get install -y fonts-liberation
WORKDIR /App
COPY --from=build /App/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.dll"]