FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RedditAnalyzer/*.csproj ./RedditAnalyzer/
RUN dotnet restore ./RedditAnalyzer/RedditAnalyzer.csproj

COPY . .
WORKDIR /src/RedditAnalyzer

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RedditAnalyzer.dll"]