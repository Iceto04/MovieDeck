namespace MovieDeck.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using MovieDeck.Data;
    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;

    [Area("Administration")]
    public class GenresController : AdministrationController
    {
        private readonly IDeletableEntityRepository<Genre> dataRepository;

        public GenresController(IDeletableEntityRepository<Genre> dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        // GET: Administration/Genres
        public async Task<IActionResult> Index()
        {
            return this.View(await this.dataRepository.AllWithDeleted().ToListAsync());
        }

        // GET: Administration/Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var genre = await this.dataRepository.AllWithDeleted()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return this.NotFound();
            }

            return this.View(genre);
        }

        // GET: Administration/Genres/Create
        public IActionResult Create()
        {
            return this.View();
        }

        // POST: Administration/Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Genre genre)
        {
            if (this.ModelState.IsValid)
            {
                await this.dataRepository.AddAsync(genre);
                await this.dataRepository.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(genre);
        }

        // GET: Administration/Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var genre = await this.dataRepository.AllWithDeleted().FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null)
            {
                return this.NotFound();
            }

            return this.View(genre);
        }

        // POST: Administration/Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Genre genre)
        {
            if (id != genre.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.dataRepository.Update(genre);
                    await this.dataRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.GenreExists(genre.Id))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(genre);
        }

        // GET: Administration/Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var genre = await this.dataRepository.All()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return this.NotFound();
            }

            return this.View(genre);
        }

        // POST: Administration/Genres/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await this.dataRepository.All().FirstOrDefaultAsync(x => x.Id == id);
            this.dataRepository.Delete(genre);
            await this.dataRepository.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        private bool GenreExists(int id)
        {
            return this.dataRepository.AllWithDeleted().Any(e => e.Id == id);
        }
    }
}
