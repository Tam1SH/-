Чтобы выполнить миграцию, нужно внутри контейнера back, собранного в дебаге, выполнить:
dotnet ef database update

Чтобы сгенерировать клиент для апи, нужно выполнить скрипт в /scripts

При написании фронта я основывался на методологии FSD (https://feature-sliced.design/ru/docs)
А в бекенде - на clean architecture

# Для сборки:
## dev 
docker-compose -f docker-compose.base.yaml -f docker-compose.dev.yaml build

## prod
docker-compose -f docker-compose.base.yaml -f docker-compose.prod.yaml build

# Запуск

## Для запуска dev-сборки:
docker-compose -f docker-compose.base.yaml -f docker-compose.dev.yaml up

## Для запуска релизной сборки:
docker-compose -f docker-compose.base.yaml -f docker-compose.prod.yaml up -d