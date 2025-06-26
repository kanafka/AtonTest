# User Management API

**Web API на .NET 9** для управления пользователями с CRUD-операциями и Swagger UI.

## 🎯 Задача для стажёра (C#-разработка)

Реализовать Web API сервис, выполняющий CRUD над сущностью **User**. Доступ к методам через Swagger UI. Для хранения данных можно использовать in-memory или PostgreSQL.

### Функциональные требования:

1. **Create**: создать пользователя с полями

    * `Login`, `Password`, `Name`, `Gender`, `Birthday`, `Admin`
    * Доступно только администраторам.
2. **Update**:

    * Изменить `Name`, `Gender`, `Birthday` (admin или сам активный пользователь).
    * Изменить `Password` (admin или сам активный пользователь).
    * Изменить `Login` (admin или сам активный пользователь; уникальность).
3. **Read**:

    * Список всех активных пользователей, отсортированных по `CreatedOn` (admin).
    * Информация по логину: `Name`, `Gender`, `Birthday`, `IsActive` (admin).
    * Аутентификация по `Login` + `Password` (сам пользователь, если активен).
    * Пользователи старше заданного возраста (admin).
4. **Delete**:

    * Полное или мягкое удаление по `Login` (мягкое проставляет `RevokedOn`, `RevokedBy`) (admin).
5. **Restore**: очистка полей `RevokedOn`, `RevokedBy` (admin).

### Поля сущности `User`:

```csharp
public Guid Id;
public string Login;    // только латиница и цифры
public string Password; // только латиница и цифры
public string Name;     // латиница и кириллица
public Gender Gender;   // 0=женщина,1=мужчина,2=неизвестно
public DateTime? Birthday;
public bool Admin;
public DateTime CreatedOn;
public string CreatedBy;
public DateTime ModifiedOn;
public string ModifiedBy;
public DateTime? RevokedOn;
public string? RevokedBy;
public bool IsActive => !RevokedOn.HasValue;
```

## ⚙️ Запуск проекта

1. **Через Docker**

   ```bash
   docker-compose up -d
   ```




2. **Открыть Swagger UI**
   - http://localhost:5000/swagger
   - https://localhost:5001/swagger


затем открыть [http://localhost:5001/swagger](http://localhost:5000/swagger)

## 📁 Структура проекта

```
Domain/         # Сущности и интерфейсы
Application/    # Сервисная логика и DTO
Infrastructure/ # Реализация DbContext, репозиториев, миграций
API/            # ASP.NET Core Web API, контроллеры, Swagger
```

---


