FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./

# Build and publish a release
RUN dotnet clean
RUN dotnet restore
RUN dotnet publish TrainCloud.Microservices.Email/TrainCloud.Microservices.Email.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "TrainCloud.Microservices.Email.dll"]
