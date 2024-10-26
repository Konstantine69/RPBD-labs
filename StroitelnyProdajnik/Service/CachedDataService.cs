using Microsoft.Extensions.Caching.Memory;
using StroitelnyProdajnik.Data;
using StroitelnyProdajnik.Models;

namespace StroitelnyProdajnik.Service
{
    public class CachedDataService
    {
        private readonly BuilderPodContext _context;
        private readonly IMemoryCache _cache;
        private const int RowCount = 20;

        public CachedDataService(BuilderPodContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IEnumerable<BuildingMaterial> GetBuildingMaterials()
        {
            if (!_cache.TryGetValue("BuildingMaterials", out IEnumerable<BuildingMaterial> buildingMaterials))
            {
                buildingMaterials = _context.BuildingMaterials.Take(RowCount).ToList();
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
                constructionObjects = _context.ConstructionObjects.Take(RowCount).ToList();
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
                objectMaterials = _context.ObjectMaterials.Take(RowCount).ToList();
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
                objectWorks = _context.ObjectWorks.Take(RowCount).ToList();
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

        public IEnumerable<ViewFullConstructionObjectInfo> GetFullConstructionObjectInfo()
        {
            if (!_cache.TryGetValue("FullConstructionObjectInfo", out IEnumerable<ViewFullConstructionObjectInfo> fullConstructionObjectInfo))
            {
                fullConstructionObjectInfo = _context.ViewFullConstructionObjectInfos.Take(RowCount).ToList();
                _cache.Set("FullConstructionObjectInfo", fullConstructionObjectInfo, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2 * 22 + 240)
                });
            }
            return fullConstructionObjectInfo;
        }
    }
}
