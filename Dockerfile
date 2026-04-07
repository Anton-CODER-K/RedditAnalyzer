# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RedditAnalyzer/*.csproj ./RedditAnalyzer/
RUN dotnet restore ./RedditAnalyzer/RedditAnalyzer.csproj

COPY . .
WORKDIR /src/RedditAnalyzer

RUN dotnet publish -c Release -o /app/publish


# ---------- FINAL ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# залежності для chromium
RUN apt-get update && apt-get install -y \
    wget curl gnupg ca-certificates \
    fonts-liberation \
    libasound2t64 \
    libxshmfence1 \
    libnss3 \
    libatk-bridge2.0-0 \
    libx11-xcb1 \
    libgtk-3-0 \
    libgbm1 \
    nodejs npm \
    && rm -rf /var/lib/apt/lists/*

# 🔥 ПРОСТО СТАВИМО (в правильний шлях)
RUN npm install -g playwright \
    && playwright install chromium

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "RedditAnalyzer.dll"]