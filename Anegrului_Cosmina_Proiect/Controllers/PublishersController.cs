using Anegrului_Cosmina_Proiect.Models;
using Anegrului_Cosmina_Proiect.Models.LibraryViewModels;
using StoreModel.Data;
using StoreModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anegrului_Cosmina_Proiect.Controllers
{
    //[Authorize(Policy = "OnlySales")]
    public class PublishersController : Controller
    {
        private readonly LibraryContext _context;
        private string _baseUrl = "https://localhost:44363/api/Publishers";
        private string _baseCardsUrl = "https://localhost:44363/api/Cards";

        public PublishersController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int? cardID)
        {

            var viewModel = new PublisherIndexData();
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);

            IEnumerable<Publisher> publishers = new List<Publisher>();
            if (response.IsSuccessStatusCode)
            {
                publishers = JsonConvert.DeserializeObject<List<Publisher>>(await response.Content.ReadAsStringAsync());
            }

            viewModel.Publishers = publishers;

            if (id != null)
            {
                ViewData["PublisherID"] = id.Value;
                 var publisher = viewModel.Publishers.Where(
                i => i.ID == id.Value).Single();
                viewModel.Cards = publisher.PublishedCards.Select(s => s.Card);
            }
            if (cardID != null)
            {
                ViewData["BoookID"] = cardID.Value;
                viewModel.Orders = viewModel.Cards.Where(
                x => x.ID == cardID).Single().Orders;
            }
            return View(viewModel);
        }

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
                var publisher = JsonConvert.DeserializeObject<Publisher>(
                await response.Content.ReadAsStringAsync());
                return View(publisher);
            }
            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PublisherName,Adress")] Publisher publisher)
        {
            if (!ModelState.IsValid) return View(publisher);
            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(publisher);
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

            return View(publisher);
        }

        // GET: Publishers/Edit/5
        [HttpGet]
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
                var publisher = JsonConvert.DeserializeObject<Publisher>(await response.Content.ReadAsStringAsync());
                ViewData["Cards"] = await PopulatePublishedBookData(publisher);
                return View(publisher);
            }
            return new NotFoundResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,PublisherName,Adress")] Publisher publisher, string[] selectedCards)
        {
            if (publisher == null)
            {
                return NotFound();
            }
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(new SavePublisher { 
                Publisher = publisher,
                SelectedCards = selectedCards
            });
            var respone = await client.PutAsync($"{_baseUrl}", new StringContent(json, Encoding.UTF8, "application/json"));

            if (respone.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));

            }
            ViewData["Cards"] = await PopulatePublishedBookData(publisher);
            return View();
        }

        private async Task<List<PublishedCardData>> PopulatePublishedBookData(Publisher publisher)
        {
            var client = new HttpClient();
            var response = new HttpResponseMessage();
            try
            {
                response = await client.GetAsync(_baseCardsUrl);
            }catch(Exception ex)
            {
                return null;
            }
            var allCards = new List<Card>();
            if (response.IsSuccessStatusCode)
            {
                allCards = JsonConvert.DeserializeObject<List<Card>>(await response.Content.ReadAsStringAsync());
            }
            var publisherBooks = new HashSet<int>(publisher.PublishedCards.Select(c => c.CardID));
            var viewModel = new List<PublishedCardData>();
            foreach (var card in allCards)
            {
                viewModel.Add(new PublishedCardData
                {
                    CardID = card.ID,
                    Title = card.Title,
                    IsPublished = publisherBooks.Contains(card.ID)
                });
            }
              return viewModel;
        }

        // GET: Publishers/Delete/5
        public async Task<IActionResult> Delete([Bind("ID")] Publisher publisher)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/{publisher.ID}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(publisher), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record: { ex.Message}");
            }
            return View(publisher);
        }

    }
}
