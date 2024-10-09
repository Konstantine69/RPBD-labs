using StroitelPodryadchik.Data;
using StroitelPodryadchik.Models;
using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rpbd
{
    internal class Program
    {
        static StroitelPodryadchikContext context = new StroitelPodryadchikContext();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Выборка всех данных из таблицы (отношение 'один')");
                Console.WriteLine("2. Выборка данных из таблицы с фильтрацией (отношение 'один')");
                Console.WriteLine("3. Группировка данных с итогом (отношение 'многие')");
                Console.WriteLine("4. Выборка данных из двух таблиц ('один-ко-многим')");
                Console.WriteLine("5. Выборка данных из двух таблиц с фильтрацией ('один-ко-многим')");
                Console.WriteLine("6. Вставка данных в таблицу (отношение 'один')");
                Console.WriteLine("7. Вставка данных в таблицу (отношение 'многие')");
                Console.WriteLine("8. Удаление данных из таблицы (отношение 'один')");
                Console.WriteLine("9. Удаление данных из таблицы (отношение 'многие')");
                Console.WriteLine("10. Обновление данных с условием");
                Console.WriteLine("0. Выход");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        SelectAllBuildingMaterials();
                        break;
                    case "2":
                        SelectConstructionObjectsWithFilter();
                        break;
                    case "3":
                        GroupMaterialsByManufacturer();
                        break;
                    case "4":
                        SelectMaterialsWithConstructionObject();
                        break;
                    case "5":
                        SelectMaterialsWithObjectFilter();
                        break;
                    case "6":
                        InsertConstructionObject();
                        break;
                    case "7":
                        InsertObjectMaterial();
                        break;
                    case "8":
                        DeleteBuildingMaterial();
                        break;
                    case "9":
                        DeleteConstructionObject();
                        break;
                    case "10":
                        UpdateBuildingMaterial();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }
            }
        }

        // 1. Выборка всех данных из таблицы BuildingMaterials (отношение "один")
        static void SelectAllBuildingMaterials()
        {
            var materials = context.BuildingMaterials.ToList();
            foreach (var material in materials)
            {
                Console.WriteLine($"ID: {material.MaterialId}, Название: {material.MaterialName}, Производитель: {material.Manufacturer}");
            }
        }

        // 2. Выборка данных из таблицы ConstructionObjects с фильтрацией
        static void SelectConstructionObjectsWithFilter()
        {
            Console.WriteLine("Введите название объекта для фильтрации:");
            string objectName = Console.ReadLine();

            var objects = context.ConstructionObjects
                .Where(co => co.ObjectName.Contains(objectName))
                .ToList();

            foreach (var obj in objects)
            {
                Console.WriteLine($"ID: {obj.ObjectId}, Название: {obj.ObjectName}, Генподрядчик: {obj.GeneralContractor}");
            }
        }

        // 3. Группировка данных по бинарному представлению (отношение "многие")
        static void GroupMaterialsByManufacturer()
        {
            var materialGroups = context.BuildingMaterials
                .GroupBy(m => m.Manufacturer)
                .Select(g => new
                {
                    Manufacturer = g.Key,
                    MaterialNames = g.Select(m => m.MaterialName).ToList()
                })
                .ToList();

            foreach (var group in materialGroups)
            {
                Console.WriteLine($"Производитель: {group.Manufacturer}, Материалы: {string.Join(", ", group.MaterialNames)}");
            }
        }


       // 4. Выборка данных из двух таблиц Material и ConstructionObject (отношение "один-ко-многим")
        static void SelectMaterialsWithConstructionObject()
        {
            var query = context.ObjectMaterials
                .Select(om => new
                {
                    om.Material.MaterialName,
                    om.Object.ObjectName
                })
                .ToList();

            foreach (var item in query)
            {
                Console.WriteLine($"Материал: {item.MaterialName}, Объект: {item.ObjectName}");
            }
        }

        // 5. Выборка данных из двух таблиц с фильтрацией по названию объекта
        static void SelectMaterialsWithObjectFilter()
        {
            Console.WriteLine("Введите название объекта для фильтрации материалов:");
            string objectName = Console.ReadLine();

            var materials = context.ObjectMaterials
                .Where(om => om.Object.ObjectName.Contains(objectName))
                .Select(om => new
                {
                    om.Material.MaterialName,
                    om.Object.ObjectName
                })
                .ToList();

            foreach (var item in materials)
            {
                Console.WriteLine($"Материал: {item.MaterialName}, Объект: {item.ObjectName}");
            }
        }

        // 6. Вставка данных в таблицу ConstructionObjects (отношение "один")
        static void InsertConstructionObject()
        {
            string name = ReadNonEmptyString("Введите название нового объекта: ");
            int customerId = ReadInt("ID заказчика: ");
            string contractor = ReadNonEmptyString("Введите генподрядчика: ");
            DateOnly contractDate = ReadDateOnly("Введите дату заключения договора (ГГГГ-ММ-ДД): ");

            string workList = ReadNonEmptyString("Введите список работ : ");
            DateOnly deliveryDate = ReadDateOnly("Введите дату доставки (ГГГГ-ММ-ДД) : ");
            DateOnly commissioningDate = ReadDateOnly("Введите дату ввода в эксплуатацию (ГГГГ-ММ-ДД) : ");
            string? photo = ReadNonEmptyString("Введите путь к фото : ");

            ConstructionObject newObject = new ConstructionObject
            {
                ObjectName = name,
                CustomerId = customerId,
                GeneralContractor = contractor,
                ContractDate = contractDate,
                WorkList = workList,
                DeliveryDate = deliveryDate,
                CommissioningDate = commissioningDate,
                Photo = photo
            };

            context.ConstructionObjects.Add(newObject);
            context.SaveChanges();

            Console.WriteLine("Объект добавлен.");
        }


        // 7. Вставка данных в таблицу ObjectMaterials (отношение "многие")
        static void InsertObjectMaterial()
        {
            int objectId = ReadInt("ID объекта: ");
            int materialId = ReadInt("ID материала: ");

            // Check if the ObjectId exists in the ConstructionObjects table
            bool objectExists = context.ConstructionObjects.Any(co => co.ObjectId == objectId);

            if (!objectExists)
            {
                Console.WriteLine("Ошибка: Объект с таким ID не существует.");
                return;
            }

            ObjectMaterial newObjectMaterial = new ObjectMaterial
            {
                ObjectId = objectId,
                MaterialId = materialId
            };

            context.ObjectMaterials.Add(newObjectMaterial);
            context.SaveChanges();

            Console.WriteLine("Материал объекта добавлен.");
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите число.");
                }
            }
        }

        static DateOnly ReadDateOnly(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (DateOnly.TryParse(input, out DateOnly result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Некорректная дата. Пожалуйста, введите дату в формате ГГГГ-ММ-ДД.");
                }
            }
        }

        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Значение не может быть пустым. Попробуйте снова.");
                }
            }
        }


        // 8. Удаление данных из таблицы BuildingMaterials (отношение "один")
        static void DeleteBuildingMaterial()
        {
            Console.Write("Введите ID материала для удаления: ");
            int materialId = int.Parse(Console.ReadLine());

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var material = context.BuildingMaterials.Find(materialId);
                    if (material != null)
                    {
                        var materialToDelete = context.ObjectMaterials.Where(c => c.MaterialId == materialId).ToList();
                        
                        // Удаляем 
                        context.ObjectMaterials.RemoveRange(materialToDelete);
                        context.SaveChanges();
                        transaction.Commit();
                        Console.WriteLine("Связанные данные удалены.");

                        // Удаляем автомобили
                        context.BuildingMaterials.Remove(material);
                        context.SaveChanges();
                        Console.WriteLine("Материал удален.");
                    }
                    else
                    {
                        Console.WriteLine("Материал не найден.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }



        // 9. Удаление данных из таблицы ConstructionObjects (отношение "многие")
        static void DeleteConstructionObject()
        {
            Console.Write("Введите ID объекта для удаления: ");
            int objectId = int.Parse(Console.ReadLine());

            // Поиск объекта
            var obj = context.ConstructionObjects.Find(objectId);

            if (obj != null)
            {
                // Удаление связанных записей из таблицы ObjectWork
                var relatedWorks = context.ObjectWorks.Where(ow => ow.ObjectId == objectId).ToList();
                foreach (var work in relatedWorks)
                {
                    context.ObjectWorks.Remove(work);
                }

                // Удаление связанных записей из таблицы ObjectMaterials
                var relatedMaterials = context.ObjectMaterials.Where(om => om.ObjectId == objectId).ToList();
                foreach (var material in relatedMaterials)
                {
                    context.ObjectMaterials.Remove(material);
                }

                // Удаление самого объекта
                context.ConstructionObjects.Remove(obj);

                // Сохранение изменений
                context.SaveChanges();

                Console.WriteLine("Объект и все связанные данные удалены.");
            }
            else
            {
                Console.WriteLine("Объект не найден.");
            }
        }



        // 10. Обновление данных в таблице BuildingMaterials с условием
        static void UpdateBuildingMaterial()
        {
            Console.Write("Введите ID материала для обновления: ");
            int materialId = int.Parse(Console.ReadLine());

            var material = context.BuildingMaterials.Find(materialId);
            if (material != null)
            {
                Console.Write("Введите новое значение для поля 'Название материала': ");
                string newName = Console.ReadLine();

                material.MaterialName = newName;
                context.SaveChanges();
                Console.WriteLine("Данные материала обновлены.");
            }
            else
            {
                Console.WriteLine("Материал не найден.");
            }
        }
    }
}
