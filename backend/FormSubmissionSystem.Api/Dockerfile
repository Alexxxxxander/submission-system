FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FormSubmissionSystem.Domain/*.csproj ./FormSubmissionSystem.Domain/
COPY FormSubmissionSystem.Application/*.csproj ./FormSubmissionSystem.Application/
COPY FormSubmissionSystem.Infrastructure/*.csproj ./FormSubmissionSystem.Infrastructure/
COPY FormSubmissionSystem.Api/*.csproj ./FormSubmissionSystem.Api/
RUN dotnet restore FormSubmissionSystem.Api/FormSubmissionSystem.Api.csproj

COPY . .
RUN dotnet build FormSubmissionSystem.Api/FormSubmissionSystem.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FormSubmissionSystem.Api/FormSubmissionSystem.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5000

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FormSubmissionSystem.Api.dll"]


