using IDistributedCacheRedisApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IDistributedCacheRedisApp.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;

        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();

            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(25);

            //_distributedCache.SetString("name","Diren",cacheEntryOptions);
            //await _distributedCache.SetString("surname", "Gündoğdu", cacheEntryOptions);

            Product product = new Product { Id=1, Name="Pc", Price=45000 };

            string jsonproduct = JsonConvert.SerializeObject(product);

            //await _distributedCache.SetStringAsync("product:1",jsonproduct,cacheEntryOptions);

            Byte[] bytes = Encoding.UTF8.GetBytes(jsonproduct);

            _distributedCache.Set("product:1", bytes);



            return View();
        }

        public IActionResult Show(){

            //string name = _distributedCache.GetString("name");
            //string surname = _distributedCache.GetString("surname");
            //ViewBag.Name = name;

            //string jsonproduct = _distributedCache.GetString("product:1");

            Byte[] bytes = _distributedCache.Get("product:1");

            string jsonproduct = Encoding.UTF8.GetString(bytes); 

            Product product = JsonConvert.DeserializeObject<Product>(jsonproduct);

            ViewBag.Product = product;


            return View();
        }

        public IActionResult Delete()
        {
            _distributedCache.Remove("name");

            return View();
        }
    
        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/car.jpg");

            byte[] imagebyte = System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("image", imagebyte);

            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] imagebyte = _distributedCache.Get("image");

            return File(imagebyte, "image/jpg");
        }
    }
}
