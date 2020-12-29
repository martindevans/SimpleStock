using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleStock.Data;
using SimpleStock.TUI.States;
using Terminal.Gui;

namespace SimpleStock.TUI
{
    public class PrimaryUi
    {
        private readonly IDbContextFactory<ApplicationDbContext> _db;
        private readonly IServiceProvider _services;

        public PrimaryUi(IDbContextFactory<ApplicationDbContext> db, IServiceProvider services)
        {
            _db = db;
            _services = services;
        }

        public MenuBarItem[] ConfigureMenubar(params MenuBarItem[] items)
        {
            var i = new List<MenuBarItem> {
                new("_File", new[] {
                    new MenuItem("_Save", "", Save),
                    new MenuItem("_Options", "", OptionsModal),
                    new MenuItem("_Close", "", Application.RequestStop),
                }),
                new("_Window", new[] {
                    new MenuItem("_Stock List", "", OpenStockList),
                    new MenuItem("_Stock Log", "", OpenStockLog),
                }),
            };

            i.AddRange(items);

            i.Add(
                new("_Help", new[] {
                    new MenuItem("_About", "", AboutModal)
                })
            );

            return i.ToArray();
        }

        private void Save()
        {
            using (var db = _db.CreateDbContext())
                db.SaveChanges();
        }

        private void OpenStockList()
        {
            Application.RequestStop();
            _services.GetRequiredService<ShowItemList>().Enter();
        }

        private void OpenStockLog()
        {
            Application.RequestStop();
            _services.GetRequiredService<ShowLog>().Enter();
        }

        public void RunModal(Toplevel modal)
        {
            Application.Run(modal);
        }

        private void AboutModal()
        {
            var ok = new Button("Ok") {
                X = Pos.Center()
            };
            ok.Clicked += Application.RequestStop;

            var dialog = new Dialog("About", 40, 15, ok);

            var y = 0;
            void L(string text, TextAlignment alignment = TextAlignment.Centered)
            {
                var label = new Label {
                    X = 0,
                    Y = ++y,
                    Width = Dim.Fill(),
                    Height = 1,
                    TextAlignment = alignment,
                    Text = text
                };
                dialog.Add(label);
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version ?? new System.Version("0.0.0.0");
            L($"Simple Stock® v{version.Major}.{version.Minor} b{version.Build}r{version.Revision}");
            L("© 2020 EvanSoft Corporation");
            L("All Rights Reserved.");
            L("");

            var dotnetVersion = Environment.Version;
            L($"Common Language Runtime v{dotnetVersion.Major}.{dotnetVersion.Minor} b{dotnetVersion.Build}");
            L("© 2020 MicroSoft Corporation");
            L("All Rights Reserved.");
            L("");

            var os = Environment.OSVersion;
            var bits = Environment.Is64BitOperatingSystem ? "64" : "32";
            L($"{os.Platform}({bits}) v{os.Version.Major}.{os.Version.Minor} b{os.Version.Build}");
            L($"Page Size: {Environment.SystemPageSize}");

            RunModal(dialog);
        }

        private void OptionsModal()
        {
            _services.GetRequiredService<OptionsModal>().Enter();
        }
    }
}
