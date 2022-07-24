namespace MovieDeck.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Companies;

    public class CompaniesController : BaseController
    {
        private readonly ICompaniesService companiesService;

        public CompaniesController(
            ICompaniesService companiesService)
        {
            this.companiesService = companiesService;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            await this.companiesService.CreateAsync(input);

            return this.Redirect("/");
        }
    }
}
