FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /App
ARG NuGetPackageSourceCredentials_TrainCloud

# Copy solution 
COPY nuget.config ./
COPY *.sln ./
COPY TrainCloud.Microservices.Email/ ./TrainCloud.Microservices.Email/
COPY TrainCloud.Microservices.Email.Messages/ ./TrainCloud.Microservices.Email.Messages/
COPY TrainCloud.Microservices.Email.Models/ ./TrainCloud.Microservices.Email.Models/

# Authorize for TrainCloud NuGet packages
ENV NuGetPackageSourceCredentials_TrainCloud ${NuGetPackageSourceCredentials_TrainCloud}

# Build and publish a release
RUN dotnet clean
RUN dotnet restore
RUN dotnet publish TrainCloud.Microservices.Email/TrainCloud.Microservices.Email.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App

COPY --from=build-env /App/out .

# Create a non-root user and group
RUN groupadd -r -g 10001 dotnetuser && \
    useradd -r -u 10001 -g dotnetuser dotnetuser && \
    chown -R dotnetuser:dotnetuser /App

# Switch to non-root user
USER dotnetuser

ENTRYPOINT ["dotnet", "TrainCloud.Microservices.Email.dll"]
