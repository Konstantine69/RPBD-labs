using Microsoft.Extensions.Caching.Memory;
using ProdajnikWebController.Models;
using ProdajnikWebController.Data;
using ProdajnikWebController.Models;
using Microsoft.EntityFrameworkCore;

namespace ProdajnikWebController.Service
{
    public class CachedDataService
    {
        private readonly ProdajnikContext _context;
        private readonly IMemoryCache _cache;
        private const int RowCount = 20;

        public CachedDataService(ProdajnikContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IEnumerable<BuildingMaterial> GetBuildingMaterials()
        {
            if (!_cache.TryGetValue("BuildingMaterials", out IEnumerable<BuildingMaterial> buildingMaterials))
            {
                buildingMaterials = _context.BuildingMaterials
                    .Take(RowCount)
                    .ToList();
                _cache.Set("BuildingMaterials", buildingMaterials, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return buildingMaterials;
        }

        public IEnumerable<ConstructionObject> GetConstructionObjects()
        {
            if (!_cache.TryGetValue("ConstructionObjects", out IEnumerable<ConstructionObject> constructionObjects))
            {
                constructionObjects = _context.ConstructionObjects
                    .Include(c => c.Customer)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("ConstructionObjects", constructionObjects, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return constructionObjects;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            if (!_cache.TryGetValue("Customers", out IEnumerable<Customer> customers))
            {
                customers = _context.Customers.Take(RowCount).ToList();
                _cache.Set("Customers", customers, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return customers;
        }

        public IEnumerable<ObjectMaterial> GetObjectMaterials()
        {
            if (!_cache.TryGetValue("ObjectMaterials", out IEnumerable<ObjectMaterial> objectMaterials))
            {
                objectMaterials = _context.ObjectMaterials
                    .Include(c => c.Material)
                    .Include(c => c.Object)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("ObjectMaterials", objectMaterials, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return objectMaterials;
        }

        public IEnumerable<ObjectWork> GetObjectWorks()
        {
            if (!_cache.TryGetValue("ObjectWorks", out IEnumerable<ObjectWork> objectWorks))
            {
                objectWorks = _context.ObjectWorks
                    .Include(c => c.Object)
                    .Include(c => c.WorkType)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("ObjectWorks", objectWorks, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return objectWorks;
        }

        public IEnumerable<WorkType> GetWorkTypes()
        {
            if (!_cache.TryGetValue("WorkTypes", out IEnumerable<WorkType> workTypes))
            {
                workTypes = _context.WorkTypes.Take(RowCount).ToList();
                _cache.Set("WorkTypes", workTypes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return workTypes;
        }
    }
}
