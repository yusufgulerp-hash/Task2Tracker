FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Task2Tracker.slnx .

COPY Task2Tracker.WebAPI/Task2Tracker.WebAPI.csproj Task2Tracker.WebAPI/
COPY Task2Tracker.Application/Task2Tracker.Application.csproj Task2Tracker.Application/
COPY Task2Tracker.Infrastructure/Task2Tracker.Infrastructure.csproj Task2Tracker.Infrastructure/
COPY Task2Tracker.Domain/Task2Tracker.Domain.csproj Task2Tracker.Domain/

RUN dotnet restore Task2Tracker.slnx

COPY . .

RUN dotnet build Task2Tracker.WebAPI/Task2Tracker.WebAPI.csproj -c Release -o /app/build

RUN dotnet publish Task2Tracker.WebAPI/Task2Tracker.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Task2Tracker.WebAPI.dll"]