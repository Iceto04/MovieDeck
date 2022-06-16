namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp;

    using AutoMapper.Configuration;

    using CommandLine;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MovieDeck.Data;
    using MovieDeck.Data.Common;
    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Data.Repositories;
    using MovieDeck.Data.Seeding;
    using MovieDeck.Services.Messaging;

    public static class Program
    {
        public static int Main(string[] args)
        {
            Scraping();

            return 0;
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            // Seed data on application startup
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;

                return Parser.Default.ParseArguments<SandboxOptions>(args).MapResult(
                    opts => SandboxCode(opts, serviceProvider).GetAwaiter().GetResult(),
                    _ => 255);
            }
        }

        private static void Scraping()
        {
            var config = Configuration.Default.WithDefaultLoader();

            var context = BrowsingContext.New(config);

            var document = context
                .OpenAsync($"https://www.imdb.com/title/tt4154756/")
                .GetAwaiter()
                .GetResult();

            var movieName = document
                //.QuerySelector("h1.sc-b73cd867-0")
                .QuerySelector("h1[data-testid=hero-title-block__title]")
                .TextContent;

            Console.WriteLine(movieName);

            var fullMoviePlotDoc = context
                    .OpenAsync($"https://www.imdb.com/title/tt4154756/plotsummary")
                    .GetAwaiter()
                    .GetResult();

            var fullMoviePlot = fullMoviePlotDoc.QuerySelectorAll(".ipl-zebra-list__item > p")[0].TextContent;
            Console.WriteLine(fullMoviePlot);

            var releaseDateAsString = document
                //.QuerySelector("div.sc-f65f65be-0.ktSkVi > ul > li:nth-child(1) > div > ul > li > a")
                .QuerySelector("li[data-testid=title-details-releasedate] li.ipc-inline-list__item > a")
                .TextContent;

            var releaseDateNotSplited = releaseDateAsString
                .Substring(0, releaseDateAsString.IndexOf("("))
                .Trim()
                .Replace(",", string.Empty);

            var releaseDateElements = releaseDateNotSplited.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var month = releaseDateElements[0];
            var day = int.Parse(releaseDateElements[1]);
            var year = int.Parse(releaseDateElements[2]);
            Console.WriteLine($"{day} {month} {year}");

            int monthNumber = DateTimeFormatInfo.InvariantInfo.MonthNames.ToList().IndexOf(month) + 1;

            var releaseDate = new DateTime(year, monthNumber, day);

            var runtimeAsString = document.QuerySelector("li[data-testid=title-techspec_runtime] > div").TextContent;

            //var runtimeHours = int.Parse(runtimeAsString
            //    .Substring(0, runtimeAsString.IndexOf("hours"))
            //    .Trim());

            var runtimeDataSplited = runtimeAsString
                .ToLower()
                .Replace("hours", string.Empty)
                .Replace("minutes", string.Empty)
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            var runtimeHours = runtimeDataSplited[0];
            var runtimeMinutes = runtimeDataSplited[1];

            var runtime = new TimeSpan(runtimeHours, runtimeMinutes, 0);

            var genres = document.QuerySelectorAll("div[data-testid=genres] > a > ul > li")
                .Select(x => x.TextContent);

            var directorsNames = document
                .QuerySelector("section[data-testid=title-cast] > ul > li > div > ul")
                .QuerySelectorAll("li > a");

            foreach (var director in directorsNames)
            {
                Console.WriteLine(director.TextContent);
            }

            var companiesDoc = context
                .OpenAsync("https://www.imdb.com/title/tt4154756/companycredits")
                .GetAwaiter()
                .GetResult();

            var companies = companiesDoc
                .QuerySelector("ul.simpleList")
                .QuerySelectorAll("li > a")
                .Select(x => x.TextContent)
                .ToArray();

            var actorsDoc = context
                .OpenAsync("https://www.imdb.com/title/tt4154756/fullcredits")
                .GetAwaiter()
                .GetResult();

            var actors = actorsDoc.QuerySelectorAll("table.cast_list > tbody > tr.odd, tr.even");

            foreach (var actor in actors)
            {
                var actorPhotoElement = actor.QuerySelector("td.primary_photo > a").GetAttribute("href");

                var actorPhotoDoc = context.OpenAsync($"https://www.imdb.com/{actorPhotoElement}")
                    .GetAwaiter()
                    .GetResult();

                var actorPhotoUrl = actorPhotoDoc.QuerySelector("#name-poster").GetAttribute("src");

                Console.WriteLine(actorPhotoUrl);

                var actorName = actor.QuerySelector("td:not([class]) > a").TextContent.Trim();
                Console.WriteLine(actorName);

                var characters = actor.QuerySelectorAll("td.character > a").Select(x => x.TextContent).ToList();
                Console.WriteLine(string.Join(", ", characters));
            }
        }

        private static async Task<int> SandboxCode(SandboxOptions options, IServiceProvider serviceProvider)
        {
            var sw = Stopwatch.StartNew();

            Console.WriteLine(sw.Elapsed);
            return await Task.FromResult(0);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(configuration);

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .UseLoggerFactory(new LoggerFactory()));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
        }
    }
}
