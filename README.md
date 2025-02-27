﻿# 🚆 TrainCloud.Microservices.Email

`TrainCloud.Microservices.Email` is the central email sender service for TrainCloud. 
All services which require sending emails send a Google Cloud Pub/Sub message with the email content as shown in the example below.

Services using tis service:
* TrainCloud.Microservices.Identity
* TrainCloud.Microservices.CarLists
* TrainCloud.Microservices.Disturbances
* TrainCloud.Microservices.

Sender Address is always `mail@traincloud.net` production or `mail@traincloud.dev` in development

## Status

### GitHub Actions
[![SonarQube](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/sonarqube.yml) 
[![NuGet](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/nuget.yml/badge.svg)](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/nuget.yml) 
[![Build and deploy to Cloud Run](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/google-cloudrun-docker.yml/badge.svg)](https://github.com/traincloud-net/TrainCloud.Microservices.Email/actions/workflows/google-cloudrun-docker.yml)

### SonarQube
[![Bugs](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=bugs&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email)
[![Code Smells](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=code_smells&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Duplicated Lines (%)](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=duplicated_lines_density&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Lines of Code](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=ncloc&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Maintainability Rating](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=sqale_rating&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Reliability Rating](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=reliability_rating&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Security Hotspots](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=security_hotspots&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Security Rating](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=security_rating&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Technical Debt](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=sqale_index&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email) 
[![Vulnerabilities](https://sonarqube.traincloud.net/api/project_badges/measure?project=TrainCloud.Microservices.Email&metric=vulnerabilities&token=sqb_dc5ad7da8a3ecc2534a63b9596277c6986f68742)](https://sonarqube.traincloud.net/dashboard?id=TrainCloud.Microservices.Email)


## Send an email 

### Add dependencies

```xml
<configuration>
		<packageSources>
				<add key="TrainCloud" value="https://nuget.pkg.github.com/traincloud-net/index.json" />
		</packageSources>
</configuration>
```

```bash
dotnet add package TrainCloud.Microservices.Email.Messages
dotnet add package TrainCloud.Microservices.Core
```

### Add MessageBusPublisher
```csharp
webApplicationBuilder.Services.AddTrainCloudMessageBusPublisher();
```

### Use the service as required
```csharp
[inject]
protected IMessageBusPublisherService MessageBusPublisherService  { get; init; }
```

```csharp
string topicId = Configuration.GetValue<string>("MessageBus:Topics:Email")!;
SendMailMessage busMessage = new()
{
    To = new List<string> { "mail@example.com" },
    Subject = $"TrainCloud ...",
    Body = $"Hello World!",
    IsHtml = true,
};
await MessageBusPublisherService.SendMessageAsync(topicId, busMessage);
```