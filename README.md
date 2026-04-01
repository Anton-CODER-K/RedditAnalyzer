# Reddit Analyzer API

API для аналізу постів з Reddit за ключовими словами.
Дозволяє отримати список постів з певних сабредітів та визначити, чи містять вони медіа (зображення або відео).

## Features

- Отримання постів з Reddit
- Фільтрація за ключовими словами
- Визначення наявності медіа (HasMedia)
- Підтримка кількох сабредітів

## Technologies

- C#
- ASP.NET Core Web API
- HttpClient
- JSON (System.Text.Json

## Run locally

1. Клонувати репозиторій:
   git clone <your-repo-link>

2. Відкрити в Visual Studio

3. Запустити проєкт:
   dotnet run

4. Відкрити Swagger:
   https://localhost:xxxx/swagger

## Example Request

POST /analyze

{
  "limit": 10,
  "items": [
    {
      "subreddit": "aww",
      "keywords": ["cat", "dog"]
    }
  ]
}

## Example Response

{
  "/r/aww": [
    {
      "title": "This is my cat",
      "hasMedia": true
    },
    {
      "title": "Some post",
      "hasMedia": false
    }
  ]
}

## How it works

- API отримує список сабредітів
- Завантажує пости через Reddit API
- Фільтрує пости за ключовими словами
- Визначає, чи містить пост медіа:
  - зображення (i.redd.it)
  - відео (v.redd.it)
  - gallery

## Run with Docker

docker build -t reddit-analyzer .
docker run -p 8080:80 reddit-analyzer

Open:
http://localhost:8080/swagger
