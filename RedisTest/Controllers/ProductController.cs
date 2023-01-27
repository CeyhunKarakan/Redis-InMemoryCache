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
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

                //options.SlidingExpiration = TimeSpan.FromSeconds(10);
                options.Priority = CacheItemPriority.High;

                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _memoryCache.Set("callback", $"{key}->{value} => reason:{reason}");
                });

                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);
            }

            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("time", out string timeCache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.callback = callback;
            ViewBag.Time = timeCache;   

            return View();
        }
    }
}
