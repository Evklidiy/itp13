using AutoPartsWarehouse.Models;
using System;
using System.Linq;

namespace AutoPartsWarehouse.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Parts.Any())
            {
                return;   // База уже заполнена
            }

            // 1. Поставщики (10 шт)
            var suppliers = new Supplier[]
            {
                new Supplier { Name = "АвтоЕвроОпт", Contacts = "Москва, МКАД 32км, +7(495)000-11-11", Rating = 5 },
                new Supplier { Name = "Japan Parts Direct", Contacts = "Владивосток, Портовая 5, jp-parts@mail.ru", Rating = 5 },
                new Supplier { Name = "Гермес-Авто", Contacts = "СПб, Лиговский 20, info@germes.ru", Rating = 4 },
                new Supplier { Name = "ТехноРесурс", Contacts = "Екатеринбург, Ленина 45", Rating = 3 },
                new Supplier { Name = "China Car Parts", Contacts = "Пекин/Москва, +86-10-9999", Rating = 3 },
                new Supplier { Name = "BMW Group Rus", Contacts = "Официальный дистрибьютор", Rating = 5 },
                new Supplier { Name = "Масла и Химия", Contacts = "Нижний Новгород, Складской пр-д 1", Rating = 4 },
                new Supplier { Name = "Корея Моторс", Contacts = "Москва, Варшавское ш.", Rating = 4 },
                new Supplier { Name = "ШинСервисОпт", Contacts = "Казань, ул. Восстания", Rating = 4 },
                new Supplier { Name = "ИП Петров (Разборка)", Contacts = "Местный поставщик", Rating = 2 }
            };
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            // 2. Запчасти (30 шт - разные категории)
            var parts = new Part[]
            {
                // Фильтры
                new Part { Article = "OF-1001", Name = "Фильтр масляный", Manufacturer = "Mann-Filter", CompatibleModels = "Toyota Camry, Corolla", PurchasePrice = 500, SalePrice = 900 },
                new Part { Article = "AF-3030", Name = "Фильтр воздушный", Manufacturer = "Filtron", CompatibleModels = "Ford Focus 3", PurchasePrice = 400, SalePrice = 800 },
                new Part { Article = "CF-202", Name = "Фильтр салонный угольный", Manufacturer = "Bosch", CompatibleModels = "VW Polo, Skoda Rapid", PurchasePrice = 600, SalePrice = 1200 },
                new Part { Article = "FF-55", Name = "Фильтр топливный", Manufacturer = "Knecht", CompatibleModels = "Kia Rio, Solaris", PurchasePrice = 800, SalePrice = 1500 },
                
                // Тормозная система
                new Part { Article = "BP-2020", Name = "Колодки передние", Manufacturer = "Brembo", CompatibleModels = "BMW X5, X6", PurchasePrice = 3500, SalePrice = 5500 },
                new Part { Article = "BD-111", Name = "Диск тормозной (шт)", Manufacturer = "TRW", CompatibleModels = "Audi A6, A4", PurchasePrice = 4000, SalePrice = 6500 },
                new Part { Article = "BP-Rio", Name = "Колодки задние", Manufacturer = "Sangsin", CompatibleModels = "Kia Rio 3/4", PurchasePrice = 1200, SalePrice = 2000 },
                
                // Двигатель и трансмиссия
                new Part { Article = "TIM-900", Name = "Комплект ГРМ", Manufacturer = "Continental", CompatibleModels = "VW, Skoda 1.6 MPI", PurchasePrice = 4500, SalePrice = 8000 },
                new Part { Article = "SP-5500", Name = "Свеча зажигания (Иридий)", Manufacturer = "NGK", CompatibleModels = "Mazda 6, CX-5", PurchasePrice = 800, SalePrice = 1600 },
                new Part { Article = "CL-001", Name = "Комплект сцепления", Manufacturer = "Luk", CompatibleModels = "Ford Focus 2/3", PurchasePrice = 8000, SalePrice = 14000 },
                new Part { Article = "WP-12", Name = "Помпа водяная", Manufacturer = "Dolz", CompatibleModels = "Renault Logan, Sandero", PurchasePrice = 1500, SalePrice = 2800 },
                
                // Подвеска
                new Part { Article = "SA-300", Name = "Амортизатор передний", Manufacturer = "KYB", CompatibleModels = "Toyota RAV4", PurchasePrice = 3000, SalePrice = 5200 },
                new Part { Article = "LB-55", Name = "Стойка стабилизатора", Manufacturer = "Lemforder", CompatibleModels = "Universal", PurchasePrice = 800, SalePrice = 1500 },
                new Part { Article = "WB-99", Name = "Подшипник ступицы", Manufacturer = "SKF", CompatibleModels = "Lada Vesta", PurchasePrice = 2000, SalePrice = 3500 },
                
                // Масла и жидкости
                new Part { Article = "OIL-5W40", Name = "Масло Shell Helix 5W-40 (4л)", Manufacturer = "Shell", CompatibleModels = "Универсальное", PurchasePrice = 2500, SalePrice = 4200 },
                new Part { Article = "OIL-5W30", Name = "Масло Mobil 1 5W-30 (4л)", Manufacturer = "Mobil", CompatibleModels = "Универсальное", PurchasePrice = 3200, SalePrice = 5500 },
                new Part { Article = "AF-RED", Name = "Антифриз G12 (5л)", Manufacturer = "Felix", CompatibleModels = "Универсальное", PurchasePrice = 800, SalePrice = 1300 },
                
                // Электрика
                new Part { Article = "BAT-60", Name = "Аккумулятор 60Ah", Manufacturer = "Varta", CompatibleModels = "Универсальное", PurchasePrice = 5000, SalePrice = 8500 },
                new Part { Article = "H7-LED", Name = "Лампа H7 LED (пара)", Manufacturer = "Philips", CompatibleModels = "Универсальное", PurchasePrice = 2500, SalePrice = 4500 },
                new Part { Article = "GEN-100", Name = "Генератор 100А", Manufacturer = "Valeo", CompatibleModels = "Renault Duster", PurchasePrice = 12000, SalePrice = 21000 },
                
                // Кузов
                new Part { Article = "WIP-600", Name = "Щетка стеклоочистителя 600мм", Manufacturer = "Bosch", CompatibleModels = "Универсальное", PurchasePrice = 600, SalePrice = 1100 },
                new Part { Article = "MIR-L", Name = "Зеркало левое в сборе", Manufacturer = "TYC", CompatibleModels = "Solaris 2017+", PurchasePrice = 3500, SalePrice = 6000 }
            };
            context.Parts.AddRange(parts);
            context.SaveChanges();

            // 3. Склад (для каждого товара создаем запись)
            // Логика: создаем дефицит для некоторых позиций (чтобы отчет работал)
            var stocks = new Stock[parts.Length];
            var random = new Random();

            for (int i = 0; i < parts.Length; i++)
            {
                int qty = random.Next(0, 100); // Случайное кол-во
                int min = random.Next(5, 20);  // Случайный минимум

                // Искусственно делаем дефицит для первых 3 товаров
                if (i < 3) { qty = 2; min = 10; }

                stocks[i] = new Stock
                {
                    PartId = parts[i].Id,
                    Quantity = qty,
                    Location = $"Ряд {random.Next(1, 10)}-{random.Next(1, 50)}",
                    MinQuantity = min
                };
            }
            context.Stocks.AddRange(stocks);
            context.SaveChanges();

            // 4. История поставок (5-7 штук в прошлом)
            for (int i = 0; i < 7; i++)
            {
                var supply = new Supply
                {
                    SupplierId = suppliers[random.Next(suppliers.Length)].Id,
                    Date = DateTime.Now.AddDays(-random.Next(10, 60)), // 10-60 дней назад
                    Status = "Получена"
                };
                context.Supplies.Add(supply);
                context.SaveChanges();

                // Добавляем 3-5 позиций в каждую поставку
                int positionsCount = random.Next(3, 6);
                for (int j = 0; j < positionsCount; j++)
                {
                    var part = parts[random.Next(parts.Length)];
                    context.SupplyPositions.Add(new SupplyPosition
                    {
                        SupplyId = supply.Id,
                        PartId = part.Id,
                        Quantity = random.Next(10, 50),
                        PurchasePrice = part.PurchasePrice
                    });
                }
            }
            context.SaveChanges();

            // 5. История продаж (15 штук для красивых графиков)
            var clients = new[] { "Иванов И.И.", "ООО Вектор", "Сервис-Плюс", "Петров А.С.", "Таксопарк №1", "Сидоров В.В." };
            var cars = new[] { "Ford Focus", "Toyota Camry", "Kia Rio", "BMW X5", "Lada Vesta", "Skoda Octavia" };

            for (int i = 0; i < 15; i++)
            {
                var sale = new Sale
                {
                    ClientName = clients[random.Next(clients.Length)],
                    CarDetails = cars[random.Next(cars.Length)],
                    Date = DateTime.Now.AddDays(-random.Next(1, 30)) // В течение последнего месяца
                };
                context.Sales.Add(sale);
                context.SaveChanges();

                // 1-4 товара в чеке
                int itemsCount = random.Next(1, 5);
                for (int j = 0; j < itemsCount; j++)
                {
                    var part = parts[random.Next(parts.Length)];
                    context.SalePositions.Add(new SalePosition
                    {
                        SaleId = sale.Id,
                        PartId = part.Id,
                        Quantity = random.Next(1, 4),
                        Price = part.SalePrice // Продаем по цене прайса
                    });
                }
            }
            context.SaveChanges();
        }
    }
}