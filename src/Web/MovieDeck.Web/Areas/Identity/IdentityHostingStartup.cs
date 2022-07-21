using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MovieDeck.Web.Areas.Identity.IdentityHostingStartup))]

namespace MovieDeck.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
