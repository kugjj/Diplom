using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WarehouseApp.Models;


namespace WarehouseApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarehouseController(AppDbContext context) => _context = context;

        // 1. Список товаров
        [HttpGet("список")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // 2. Добавить товар
        [HttpPost("добавить")]
        public async Task<IActionResult> AddProduct([FromBody] Product newProduct)
        {
            if (newProduct == null) return BadRequest("Данные не получены");

            try
            {
                var existing = string.IsNullOrEmpty(newProduct.Sku)
                    ? null
                    : await _context.Products.FirstOrDefaultAsync(p => p.Sku == newProduct.Sku);

                if (existing != null)
                {
                    // Товар есть — обновляем количество
                    var oldQty = existing.CurrentStock;
                    existing.CurrentStock += newProduct.CurrentStock;

                    // Обновляем цену только если она реально изменилась
                    if (newProduct.Price > 0 && existing.Price != newProduct.Price)
                    {
                        existing.Price = newProduct.Price;
                    }

                    await _context.SaveChangesAsync();

                    _context.SystemLogs.Add(new SystemLog
                    {
                        ActionDescription = $"Пополнение: {existing.ProductName} " +
                                            $"({oldQty} + {newProduct.CurrentStock} = {existing.CurrentStock} шт.)",
                        ActionTime = DateTime.Now
                    });
                    await _context.SaveChangesAsync();

                    return Ok(new { message = $"Остаток обновлён: теперь {existing.CurrentStock} шт." });
                }
                else
                {
                    // Новый товар
                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync();

                    _context.SystemLogs.Add(new SystemLog
                    {
                        ActionDescription = $"Добавлен новый товар: {newProduct.ProductName}",
                        ActionTime = DateTime.Now
                    });
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Новый товар успешно добавлен" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка БД: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 3. Анализ дефицита (исправлено под русские колонки процедуры)
        [HttpGet("отчет-дефицит")]
        public async Task<IActionResult> GetDeficitReport()
        {
            try
            {
                var data = await _context.Set<DeficitDto>().FromSqlRaw("EXEC sp_GetReorderReport")
                    .ToListAsync();
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Ошибка процедуры: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 4. Инвентаризация: загрузка данных
        [HttpGet("инвентаризация")]
        public async Task<IActionResult> GetInventoryData()
        {
            var data = await _context.Products
                .Select(p => new { p.ProductId, p.ProductName, p.Sku, SystemQty = p.CurrentStock })
                .ToListAsync();
            return Ok(data);
        }

        // 5. Инвентаризация: сохранение (без ошибок FK)
        [HttpPost("инвентаризация")]
        public async Task<IActionResult> SubmitInventory([FromBody] InventorySubmitDto dto)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return BadRequest("Нет данных для сохранения");

            try
            {
                // Создаем акт с безопасными значениями (если FK не позволяют null)
                var act = new InventoryAct
                {
                    CheckDate = DateTime.Now,
                    WarehouseId = 1, // Заглушка. Если в БД нет ID=1, замени на существующий или сделай поле nullable
                    EmployeeId = 1
                };
                _context.InventoryActs.Add(act);
                await _context.SaveChangesAsync();

                foreach (var item in dto.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        _context.InventoryDetails.Add(new InventoryDetail
                        {
                            ActId = act.ActId,
                            ProductId = item.ProductId,
                            SystemQty = product.CurrentStock,
                            FactQty = item.FactQty
                        });

                        if (product.CurrentStock != item.FactQty)
                        {
                            product.CurrentStock = item.FactQty;
                            _context.SystemLogs.Add(new SystemLog
                            {


                                ActionDescription = $"Инвентаризация: {product.ProductName} ({product.CurrentStock} → {item.FactQty})",
                                ActionTime = DateTime.Now
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(new { message = "Инвентаризация сохранена" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Ошибка БД: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // 6. Создание заказа
        [HttpPost("создать-заказ")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            _context.SystemLogs.Add(new SystemLog
            {
                ActionDescription = $"Заказ поставщику: {request.ProductName}, кол-во: {request.Quantity}",
                ActionTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ успешно отправлен" });
        }

        // 7. Журнал аудита
        [HttpGet("логи")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.SystemLogs
                .OrderByDescending(l => l.ActionTime)
                .Take(100)
                .Select(l => new { l.LogId, l.ActionDescription, l.ActionTime })
                .ToListAsync();
            return Ok(logs);
        }

        [HttpGet("история-цен")]
        public async Task<IActionResult> GetPriceHistory([FromQuery] int? productId)
        {
            var query = _context.PriceHistories
                .Include(h => h.Product)
                .AsQueryable();
            if (productId.HasValue)
                query = query.Where(h => h.ProductId == productId);
            var data = await query
    .OrderByDescending(h => h.ChangeDate)      
    .Take(100)
    .Select(h => new {
        ProductName = h.Product != null ? h.Product.ProductName : "Неизвестно",
        Sku = h.Product != null ? h.Product.Sku : "-",
        h.OldPrice,
        h.NewPrice,
        h.ChangeDate
    }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("статистика")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                // Первый результат процедуры — общая статистика
                var stats = await _context.Database
                    .SqlQueryRaw<WarehouseStatsDto>("EXEC sp_GetWarehouseStats")
                    .ToListAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // Получить список поставщиков
        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
            .Select(s => new { s.SupplierId, s.SupplierName, s.ContactPhone })
            .ToListAsync();
            return Ok(suppliers);
        }

        // Добавить нового поставщика
        [HttpPost("add-supplier")]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier newSupplier)
        {
            if (newSupplier == null || string.IsNullOrEmpty(newSupplier.SupplierName))
                return BadRequest("Укажите название поставщика");

            // Проверяем дубликат
            var exists = await _context.Suppliers
            .AnyAsync(s => s.SupplierName.ToLower() == newSupplier.SupplierName.ToLower());
            if (exists)
                return BadRequest("Поставщик с таким названием уже существует");

            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();

            _context.SystemLogs.Add(new SystemLog
            {
                ActionDescription = $"Добавлен новый поставщик: {newSupplier.SupplierName}",
                ActionTime = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return Ok(new { message = "Поставщик добавлен", supplierId = newSupplier.SupplierId });
        }

    }

    [Microsoft.EntityFrameworkCore.Keyless]
    public class WarehouseStatsDto
    {
        public int ВсегоТоваров { get; set; }
        public int ВсегоЕдиниц { get; set; }
        public decimal ОбщаяСтоимость { get; set; }
        public int ТоваровНетНаСкладе { get; set; }
        public int ТоваровМало { get; set; }
    }

    // ================= DTO КЛАССЫ =================
    [Microsoft.EntityFrameworkCore.Keyless]
    public class DeficitDto
    {
        [Column("Товар")]
        public string Товар { get; set; } = string.Empty;

        [Column("Артикул")]
        public string Артикул { get; set; } = string.Empty;

        [Column("Текущий остаток")]
        public int Текущий_остаток { get; set; }

        [Column("Нужно закупить")]
        public int Нужно_закупить { get; set; }

        [Column("Рекомендуемый поставщик")]
        public string? Рекомендуемый_поставщик { get; set; }

        [Column("Примерная сумма закупки")]
        public decimal Примерная_сумма_закупки { get; set; }
    }

    public class InventoryItemDto
    {
        public int ProductId { get; set; }
        public int FactQty { get; set; }
    }

    public class InventorySubmitDto
    {
        public List<InventoryItemDto>? Items { get; set; } = new();
    }

    public class OrderRequestDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}