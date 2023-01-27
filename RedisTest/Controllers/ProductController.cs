using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace RedisTest.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if (!_memoryCache.TryGetValue("time", out string timeCache))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

                options.SlidingExpiration = TimeSpan.FromSeconds(10);
                options.Priority = CacheItemPriority.High;

                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);
            }

            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("time", out string timeCache);
            ViewBag.Time = timeCache;   

            return View();
        }
    }
}
