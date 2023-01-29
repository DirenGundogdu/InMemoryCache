using InMemoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace InMemoryApp.Controllers
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
            // First Way
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("Date")))
            //{
            //    _memoryCache.Set<string>("Date", DateTime.Now.ToString());

            //}

            //Second Way
            //if (_memoryCache.TryGetValue("Date", out string datecache))
            //{
            //    _memoryCache.Set<string>("Date", DateTime.Now.ToString());

            //}



            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            //Cache time finish delete cache
            //options.AbsoluteExpiration = DateTime.Now.AddSeconds(30); 

            // When you use SlidingExpiration, use AbsoluteExpiration so that old data is not always left.
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1); 

            //If used before time runs out, cache will continue.
            options.SlidingExpiration = TimeSpan.FromSeconds(10);

            //When the memory is full, it deletes with priority. Low, Normal, High, NeverRemove
            options.Priority = CacheItemPriority.Normal;

            options.RegisterPostEvictionCallback((key, value, reason, state) => {
                _memoryCache.Set("callback", $"{key}->{value} => reason:{reason}");
            });


            _memoryCache.Set<string>("Date", DateTime.Now.ToString(), options);


            Product product = new Product { Id=1, Name="Pencil", Price=6};

            _memoryCache.Set<Product>("product:1", product);

            return View();
        }

        public IActionResult Show()
        {
            // Delete Cache
            //_memoryCache.Remove("Date");

            //Key empty create Cache
            //_memoryCache.GetOrCreate<string>("Date", entry => { 
            //    return DateTime.Now.ToString();
            //});

            _memoryCache.TryGetValue("Date", out string datecache);
            ViewBag.Date = datecache;

            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Callback = callback;

            ViewBag.product = _memoryCache.Get<Product>("product:1");

            //ViewBag.date =  _memoryCache.Get<String>("Date");
            return View();
        }
    }
}
