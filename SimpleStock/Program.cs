using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleStock.Data;
using SimpleStock.TUI;
using SimpleStock.TUI.States;
using Terminal.Gui;

namespace SimpleStock
{
    public class Program
    {
        private static void Main()
        {
            Application.Init();
            var provider = BuildServices();

            // Create DB
            var dbf = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using (var db = dbf.CreateDbContext())
                db.Database.EnsureCreated();

            // Enter the default state (showing a list of all items)
            provider.GetRequiredService<ShowItemList>().Enter();
        }

        private static IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

            // Add DB stuff
            services.AddDbContextFactory<ApplicationDbContext>(options => {
                options.UseSqlite("Data Source=database.sqlite;");
            });

            // Add UI states
            services.AddTransient<ShowItemList>();
            services.AddTransient<CreateItemModal>();
            services.AddTransient<AddStockModal>();
            services.AddTransient<RemoveStockModal>();
            services.AddTransient<DuplicateItemModal>();
            services.AddTransient<OptionsModal>();
            services.AddTransient<ShowLog>();

            // Add the UI layout
            services.AddSingleton<PrimaryUi>();

            var provider = services.BuildServiceProvider();
            services.AddSingleton(provider);

            return provider;
        }
    }
}
