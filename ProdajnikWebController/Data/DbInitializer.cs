using System;
using System.Linq;
using ProdajnikWebController.Models;
using ProdajnikWebController.Data;

namespace ProdajnikWebController.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ProdajnikContext db)
        {
            db.Database.EnsureCreated();

            if (db.BuildingMaterials.Any() || db.ConstructionObjects.Any() || db.Customers.Any())
            {
                return; // Если данные уже существуют, инициализация не требуется
            }

            Random randObj = new(1);

            // Заполнение таблицы материалов
            string[] materialNames = { "Цемент", "Кирпич", "Арматура", "Щебень", "Песок", "Бетон", "Изоляция", "Кровельные материалы" };
            foreach (var name in materialNames)
            {
                db.BuildingMaterials.Add(new BuildingMaterial
                {
                    MaterialName = name,
                    Manufacturer = "Производитель " + name,
                    PurchaseVolume = randObj.Next(1, 1000),
                    CertificateNumber = "Сертификат_" + Guid.NewGuid().ToString().Substring(0, 8),
                    CertificateDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-randObj.Next(1, 5))),
                    Photo = "photo_" + name + ".jpg"
                });
            }
            db.SaveChanges();

            // Заполнение заказчиков
            string[] customerNames = { "Компания А", "Компания Б", "Компания В", "Компания Г", "Компания Д" };
            foreach (var customerName in customerNames)
            {
                db.Customers.Add(new Customer
                {
                    OrganizationName = customerName,
                    City = "Город_" + customerName.Last(),
                    Address = "Адрес " + customerName,
                    PhoneNumber = "+7 123 456-78-90"
                });
            }
            db.SaveChanges();

            // Заполнение типов работ
            string[] licenseNumbers = { "Лицензия A", "Лицензия B", "Лицензия C" };
            foreach (var licenseNumber in licenseNumbers)
            {
                db.WorkTypes.Add(new WorkType
                {
                    LicenseNumber = licenseNumber,
                    LicenseDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-randObj.Next(1, 3))),
                    LicenseExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddYears(randObj.Next(1, 3))),
                    ClassifierCode = "Код_" + licenseNumber.Last()
                });
            }
            db.SaveChanges();

            // Заполнение объектов строительства
            var customers = db.Customers.ToList();
            for (int i = 0; i < 10; i++)
            {
                db.ConstructionObjects.Add(new ConstructionObject
                {
                    ObjectName = "Объект_" + i,
                    CustomerId = customers[randObj.Next(customers.Count)].CustomerId,
                    GeneralContractor = "Генеральный подрядчик_" + i,
                    ContractDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(100, 300))),
                    WorkList = "Список работ_" + i,
                    DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(randObj.Next(100, 200))),
                    CommissioningDate = DateOnly.FromDateTime(DateTime.Now.AddDays(randObj.Next(200, 300))),
                    Photo = "photo_object_" + i + ".jpg"
                });
            }
            db.SaveChanges();

            // Заполнение таблицы ObjectMaterial
            var materials = db.BuildingMaterials.ToList();
            var constructionObjects = db.ConstructionObjects.ToList();
            for (int i = 0; i < 20; i++)
            {
                db.ObjectMaterials.Add(new ObjectMaterial
                {
                    ObjectId = constructionObjects[randObj.Next(constructionObjects.Count)].ObjectId,
                    MaterialId = materials[randObj.Next(materials.Count)].MaterialId
                });
            }
            db.SaveChanges();

            // Заполнение таблицы ObjectWork
            var workTypes = db.WorkTypes.ToList();
            for (int i = 0; i < 20; i++)
            {
                db.ObjectWorks.Add(new ObjectWork
                {
                    ObjectId = constructionObjects[randObj.Next(constructionObjects.Count)].ObjectId,
                    WorkTypeId = workTypes[randObj.Next(workTypes.Count)].WorkTypeId
                });
            }
            db.SaveChanges();
        }
    }
}
