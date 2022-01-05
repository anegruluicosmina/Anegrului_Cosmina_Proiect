using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreModel.Data;
using StoreModel.Models;
using Anegrului_Cosmina_Proiect.Models.LibraryViewModels;
using Anegrului_Cosmina_Proiect.Models;

namespace StoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly LibraryContext _context;

        public PublishersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            return await _context.Publishers.Include(i => i.PublishedCards)
                .ThenInclude(i => i.Card)
                .ThenInclude(i => i.Orders)
                .ThenInclude(i => i.Customer)
                .AsNoTracking()
                .OrderBy(i => i.PublisherName)
                .ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers
                .Include(i => i.PublishedCards)
                .ThenInclude(i => i.Card)
                .Where(p => p.ID == id)
                .FirstOrDefaultAsync();

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {
            if (id != publisher.ID)
            {
                return BadRequest();
            }

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> PutPublisher(SavePublisher savePublisher)
        {
            if (savePublisher.Publisher == null)
            {
                return NotFound();
            }
            var publisherToUpdate = await _context.Publishers
                .Include(i => i.PublishedCards)
                .ThenInclude(i => i.Card)
                .FirstOrDefaultAsync(m => m.ID == savePublisher.Publisher.ID);

            if (await TryUpdateModelAsync<Publisher>(publisherToUpdate, "", i => i.PublisherName, i => i.Adress))
            {
                UpdatePublishedCards(savePublisher.SelectedCards, publisherToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {

                    ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists, " + ex.Message);
                }
                return Ok();
            }
            UpdatePublishedCards(savePublisher.SelectedCards, publisherToUpdate);
            PopulatePublishedCardData(publisherToUpdate);
            return NoContent();
        }

        private void UpdatePublishedCards(string[] selectedCards, Publisher publisherToUpdate)
        {
            if (selectedCards == null)
            {
                publisherToUpdate.PublishedCards = new List<PublishedCard>();
                return;
            }
            var selectedCardsHS = new HashSet<string>(selectedCards);
            var publishedCards = new HashSet<int>
            (publisherToUpdate.PublishedCards.Select(c => c.Card.ID));
            foreach (var card in _context.Cards)
            {
                if (selectedCardsHS.Contains(card.ID.ToString()))
                {
                    if (!publishedCards.Contains(card.ID))
                    {
                        publisherToUpdate.PublishedCards.Add(new PublishedCard
                        {
                            PublisherID = publisherToUpdate.ID,
                            CardID = card.ID
                        });
                    }
                }
                else
                {
                    if (publishedCards.Contains(card.ID))
                    {
                        PublishedCard cardToRemove = publisherToUpdate.PublishedCards.FirstOrDefault(i
                       => i.CardID == card.ID);
                        _context.Remove(cardToRemove);
                    }
                }
            }
        }

        private List<PublishedCardData> PopulatePublishedCardData(Publisher publisher)
        {
            var allCards = _context.Cards;
            var publisherCards = new HashSet<int>(publisher.PublishedCards.Select(c => c.CardID));
            var viewModel = new List<PublishedCardData>();
            foreach (var card in allCards)
            {
                viewModel.Add(new PublishedCardData
                {
                    CardID = card.ID,
                    Title = card.Title,
                    IsPublished = publisherCards.Contains(card.ID)
                });
            }
            return viewModel;
        }


        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.ID }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.ID == id);
        }
    }
}
