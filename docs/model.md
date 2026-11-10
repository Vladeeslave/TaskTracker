# Класова модель варіанта TaskTracker (ЛР 4)
 
Документ — наряд на роботи (`WorkOrder`), позиція — рядок наряду (`WorkOrderLine`),
учасник — замовник (`Customer`), об’єкт обліку — проєкт (`Project`, тип із ЛР 1).
 
## Сутності
 
| Сутність | Роль | Незмінний | Інваріант |
|---|---|---|---|
| WorkOrderLine | позиція документа | так, record | WorkCode не порожній; Hours > 0; HourlyRate >= 0 |
| WorkOrder | документ зі станом | ні, агрегат | Id > 0; CustomerId > 0; рядки додаються лише у стані Draft |
| Customer | довідник учасників | так, record | Id > 0; FullName не порожнє; Email містить @ |
| Project | довідник об’єктів обліку | ні (PlannedHours, IsActive змінюються) | Code не порожній; PlannedHours >= 0 |
 
## Стани наряду
 
| Зі стану | До стану | Умова | Метод |
|---|---|---|---|
| Draft | Planned | є хоча б один рядок | Plan |
| Draft | Cancelled | завжди | Cancel |
| Planned | InProgress | завжди (роботи розпочато) | Start |
| Planned | Cancelled | завжди (до початку робіт) | Cancel |
| InProgress | Done | завжди (роботи виконано) | Complete |
 
Done і Cancelled — кінцеві стани: з них переходів немає.
