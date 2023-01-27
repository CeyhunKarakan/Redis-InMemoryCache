using InMemoryApp.Web.Models;
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

            Product product = new Product{
                Id = 1,
                Name = "Apple Watch",
                Price = 200
            };

            _memoryCache.Set<Product>("product:1", product);


            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("time", out string timeCache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Time = timeCache;
            ViewBag.callback = callback;

            ViewBag.product = _memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
