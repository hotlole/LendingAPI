using ClosedXML.Excel;
using Landing.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
/// <summary>
/// Контроллер для генерации административных отчетов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Генерирует Excel-отчет за указанный месяц.
    /// </summary>
    /// <remarks>
    /// Создает Excel-файл с двумя листами:
    /// - "Пользователи": содержит ФИО, email, дату рождения и начисленные баллы за указанный месяц.
    /// - "Посещения": таблица, где строки — это мероприятия, а столбцы — пользователи. В ячейке "+" если пользователь посетил мероприятие.
    /// </remarks>
    /// <param name="month">Месяц (1-12), за который нужен отчет.</param>
    /// <param name="year">Год, за который нужен отчет.</param>
    /// <returns>Файл Excel с данными отчета.</returns>
    /// <response code="200">Успешная генерация и возврат Excel-файла.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Доступ запрещён. Необходима роль администратора.</response>
    [HttpGet("monthly-report")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GenerateMonthlyReport([FromQuery] int month, [FromQuery] int year)
    {
        var users = await _context.Users
            .Include(u => u.PointsTransactions)
            .Include(u => u.Attendances)
            .ToListAsync();

        var events = await _context.Events.ToListAsync();

        using var workbook = new XLWorkbook();

        // Лист 1: Информация о пользователях
        var sheet1 = workbook.Worksheets.Add("Пользователи");
        sheet1.Cell(1, 1).Value = "ФИО";
        sheet1.Cell(1, 2).Value = "Email";
        sheet1.Cell(1, 3).Value = "Дата рождения";
        sheet1.Cell(1, 4).Value = "Баллы за месяц";

        // Стили заголовка
        var header1 = sheet1.Range("A1:D1");
        header1.Style.Font.Bold = true;
        header1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        header1.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var user in users)
        {
            var monthlyPoints = user.PointsTransactions
                .Where(p => p.CreatedAt.Month == month && p.CreatedAt.Year == year)
                .Sum(p => p.Points);

            var fullName = !string.IsNullOrWhiteSpace(user.FullName)
                ? user.FullName
                : $"{user.LastName} {user.FirstName} {(string.IsNullOrWhiteSpace(user.MiddleName) 
                ? "" : user.MiddleName)}".Trim();
            sheet1.Cell(row, 1).Value = fullName;
            sheet1.Cell(row, 2).Value = user.Email;
            sheet1.Cell(row, 3).Value = user.BirthDate?.ToShortDateString();
            sheet1.Cell(row, 4).Value = monthlyPoints;

            row++;
        }

        sheet1.Columns().AdjustToContents(); // автоширина

        // Лист 2: Посещения мероприятий
        var sheet2 = workbook.Worksheets.Add("Посещения");

        sheet2.Cell(1, 1).Value = "Мероприятие";

        for (int i = 0; i < users.Count; i++)
        {
            var user = users[i];
            var fullName = !string.IsNullOrWhiteSpace(user.FullName)
                ? user.FullName
                : $"{user.LastName} {user.FirstName} {(string.IsNullOrWhiteSpace(user.MiddleName) ? "" : user.MiddleName)}".Trim();

            sheet2.Cell(1, i + 2).Value = fullName;
        }


        var header2 = sheet2.Range(1, 1, 1, users.Count + 1);
        header2.Style.Font.Bold = true;
        header2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        header2.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < events.Count; i++)
        {
            var ev = events[i];
            sheet2.Cell(i + 2, 1).Value = ev.Title;

            for (int j = 0; j < users.Count; j++)
            {
                var user = users[j];
                var attendance = user.Attendances.FirstOrDefault(a => a.EventId == ev.Id);

                var cell = sheet2.Cell(i + 2, j + 2);
                if (attendance != null && attendance.IsConfirmed)
                {
                    cell.Value = "+";
                    cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                }
                else
                {
                    cell.Value = "-";
                    cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                }

            }
        }

        sheet2.Columns().AdjustToContents(); // автоширина



        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"MonthlyReport_{month}_{year}.xlsx";
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
