using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anegrului_Cosmina_Proiect.Controllers
{
    //[Authorize(Roles = "Employee")]
    public class CardsController : Controller
    {
        private string _baseUrl = "https://localhost:44363/api/Cards";

        public CardsController()
        {
        }

        // GET: Cards
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);

            IEnumerable<Card> cards = new List<Card>();
            if (response.IsSuccessStatusCode)
            {
                cards = JsonConvert.DeserializeObject<List<Card>>(await response.Content.ReadAsStringAsync());
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                cards = cards.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    cards = cards.OrderByDescending(b => b.Title);
                    break;
                case "Price":
                    cards = cards.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    cards = cards.OrderByDescending(b => b.Price);
                    break;
                default:
                    cards = cards.OrderBy(b => b.Title);
                    break;
            }
            int pageSize = 2;
            return View(PaginatedList<Card>.CreateAsync(cards.AsEnumerable(), pageNumber ?? 1, pageSize));
        }

        // GET: Cards/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var card = JsonConvert.DeserializeObject<Card>(
                await response.Content.ReadAsStringAsync());
                return View(card);
            }
            return NotFound();
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Designer,Price")] Card card)
        {
            if (!ModelState.IsValid) return View(card);
            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(card);
                var response = await client.PostAsync(_baseUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record:  { ex.Message} ");
            }
            return View(card);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var card = JsonConvert.DeserializeObject<Card>(
                await response.Content.ReadAsStringAsync());
                return View(card);
            }
            return new NotFoundResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("ID,Title,Designer,Price")] Card card)
        {
            if (!ModelState.IsValid) return View(card);
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(card);
            var response = await client.PutAsync($"{_baseUrl}/{card.ID}",
            new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(card);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var card = JsonConvert.DeserializeObject<Card>(await response.Content.ReadAsStringAsync());
                return View(card);
            }
            return new NotFoundResult();
        }

        // POST: Cards/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("ID")] Card card)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/{card.ID}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(card), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record: { ex.Message}");
            }
            return View(card);
        }
    }
}
