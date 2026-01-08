using AutoPartsWarehouse.Models;
using System;
using System.Linq;

namespace AutoPartsWarehouse.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Проверяем, создана ли база данных
            context.Database.EnsureCreated();

            // Если в таблице запчастей уже есть данные, инициализатор прерывается
            if (context.Parts.Any())
            {
                return;   // База уже заполнена
            }

            // 1. Добавляем Запчасти
            var parts = new Part[]
            {
                new Part { Article = "OF-1001", Name = "Фильтр масляный", Manufacturer = "Mann-Filter", CompatibleModels = "Toyota Camry, Corolla", PurchasePrice = 500, SalePrice = 900 },
                new Part { Article = "BP-2020", Name = "Тормозные колодки (передние)", Manufacturer = "Brembo", CompatibleModels = "BMW X5, X6", PurchasePrice = 3500, SalePrice = 5500 },
                new Part { Article = "SP-5500", Name = "Свеча зажигания", Manufacturer = "NGK", CompatibleModels = "KIA Rio, Hyundai Solaris", PurchasePrice = 300, SalePrice = 600 },
                new Part { Article = "AF-3030", Name = "Фильтр воздушный", Manufacturer = "Filtron", CompatibleModels = "Ford Focus 3", PurchasePrice = 400, SalePrice = 800 },
                new Part { Article = "TIM-900", Name = "Ремень ГРМ", Manufacturer = "Continental", CompatibleModels = "VW Polo, Skoda Rapid", PurchasePrice = 1200, SalePrice = 2100 },
                new Part { Article = "OIL-5W40", Name = "Масло моторное 5W-40 (4л)", Manufacturer = "Shell", CompatibleModels = "Универсальное", PurchasePrice = 2500, SalePrice = 4200 },
            };

            context.Parts.AddRange(parts);
            context.SaveChanges(); // Сохраняем, чтобы получить ID запчастей

            // 2. Добавляем Поставщиков
            var suppliers = new Supplier[]
            {
                new Supplier { Name = "ООО АвтоОпт", Contacts = "Москва, ул. Ленина 1, +7(495)123-45-67", Rating = 5 },
                new Supplier { Name = "Japan Parts Direct", Contacts = "Владивосток, Портовая 5, jp-parts@mail.ru", Rating = 4 },
                new Supplier { Name = "ЕвроДеталь", Contacts = "Санкт-Петербург, euro@detal.ru", Rating = 3 }
            };

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            // 3. Добавляем Складские остатки (обязательно для каждой запчасти!)
            // Логика: связываем по ID запчасти, который она получила после сохранения
            var stocks = new Stock[]
            {
                new Stock { PartId = parts[0].Id, Quantity = 50, Location = "Стеллаж А-1", MinQuantity = 10 },
                new Stock { PartId = parts[1].Id, Quantity = 4, Location = "Стеллаж Б-3", MinQuantity = 5 },  // Дефицит!
                new Stock { PartId = parts[2].Id, Quantity = 100, Location = "Ящик 12", MinQuantity = 20 },
                new Stock { PartId = parts[3].Id, Quantity = 15, Location = "Стеллаж А-2", MinQuantity = 5 },
                new Stock { PartId = parts[4].Id, Quantity = 2, Location = "Склад 2", MinQuantity = 5 },      // Дефицит!
                new Stock { PartId = parts[5].Id, Quantity = 20, Location = "Зона масел", MinQuantity = 5 },
            };

            context.Stocks.AddRange(stocks);
            context.SaveChanges();

            // 4. Добавляем одну тестовую поставку (история)
            var supply = new Supply
            {
                SupplierId = suppliers[0].Id,
                Date = DateTime.Now.AddDays(-5),
                Status = "Получена"
            };
            context.Supplies.Add(supply);
            context.SaveChanges();

            var supplyPos = new SupplyPosition
            {
                SupplyId = supply.Id,
                PartId = parts[0].Id,
                Quantity = 10,
                PurchasePrice = 500
            };
            context.SupplyPositions.Add(supplyPos);

            // 5. Добавляем одну тестовую продажу
            var sale = new Sale
            {
                ClientName = "Иван Иванов",
                CarDetails = "Toyota Camry",
                Date = DateTime.Now.AddDays(-1)
            };
            context.Sales.Add(sale);
            context.SaveChanges();

            var salePos = new SalePosition
            {
                SaleId = sale.Id,
                PartId = parts[0].Id, // Фильтр
                Quantity = 1,
                Price = 900
            };
            context.SalePositions.Add(salePos);

            context.SaveChanges();
        }
    }
}