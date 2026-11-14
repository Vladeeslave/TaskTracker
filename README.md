# TaskTracker

![CI](https://github.com/Vladeeslave/TaskTracker/actions/workflows/ci.yml/badge.svg)

Навчальний проєкт з дисципліни «Конструювання програмного забезпечення».
Варіант № N: облік робочих завдань команди.

## Що це і для чого

Програма веде облік завдань команди: кожне завдання має стан
(`New`, `InProgress`, `Review`, `Done`, `Cancelled`) і записи часу,
з яких складається загальна кількість витрачених годин.

## Склад рішення

- `TaskTracker.Core` — логіка предметної області (бібліотека класів)
- `TaskTracker.App` — консольний застосунок (введення, виведення)

## Вимоги

.NET SDK 8.0 або новіший, Git.

## Складання та запуск

```
dotnet build
dotnet run --project TaskTracker.App
```

Очікуваний вивід:

```
Завдання #1
Стан: New
Записів часу: 2
Годин усього: 3.75
```

## Стандарт кодування

Правила описано у файлі `CODING_STANDARD.md` і закріплено в `.editorconfig`.
Перевірка: `dotnet format --verify-no-changes`

## Автор

Прізвище Ім'я, група

## Конвеєр CI

Кожен push у main і feature/** та кожен pull request у main запускає GitHub Actions
(файл .github/workflows/ci.yml) на чистій машині ubuntu з .NET 8.

| Крок | Команда | Поріг |
|------|---------|-------|
| Формат | dotnet format --verify-no-changes | без змін |
| Складання | dotnet build -c Release | 0 попереджень, 0 помилок |
| Тести | dotnet test -c Release | 100 % зелених |
| Покриття | /p:Threshold=60 | не менше 60 % рядків TaskTracker.Core |

Зламана збірка — найвищий пріоритет: поки main червона, нові задачі не починаються.

## Версія

Поточна версія 0.8.0 задана властивістю VersionPrefix у Directory.Build.props
і друкується застосунком першим рядком.

## Перевірка локально

    dotnet format --verify-no-changes
    dotnet build -c Release
    dotnet test
