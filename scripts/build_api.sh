#!/bin/bash

# Выполнение команды в контейнере back
echo "Выполнение команды 'dotnet build' в контейнере back..."
docker exec -e BUILD_API=true back dotnet build

# Проверка статуса выполнения предыдущей команды
if [ $? -eq 0 ]; then
  echo "Команда 'dotnet build' успешно выполнена в контейнере back."
else
  echo "Ошибка при выполнении команды 'dotnet build' в контейнере back."
  exit 1
fi

# Выполнение команды в контейнере front_
echo "Выполнение команды 'npm run build_api' в контейнере front_..."
docker exec front_ npm run build_api

# Проверка статуса выполнения предыдущей команды
if [ $? -eq 0 ]; then
  echo "Команда 'npm run build_api' успешно выполнена в контейнере front_."
else
  echo "Ошибка при выполнении команды 'npm run build_api' в контейнере front_."
  exit 1
fi

echo "Все команды успешно выполнены."