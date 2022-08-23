using ElasticSearchExample.Entities;
using ElasticSearchExample.Interfaces;
using ElasticSearchExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IElasticSearchService _elasticSearchService;

        public HomeController(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }


        public async Task<IActionResult> Index()
        {

            await _elasticSearchService.CheckIndex("product");

            var model = await GetItems();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([ModelBinder] List<Product> products)
        {

            if (ModelState.IsValid)
            {
                await _elasticSearchService.InsertDocuments("product", products);
            }

            return RedirectToAction("Index");
        }

        private async Task<List<Product>> GetItems()
        {
            await Task.Delay(500);
            return await _elasticSearchService.GetDocuments("product");
        }
    }
}
