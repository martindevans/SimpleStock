using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using SimpleStock.Data;
using SimpleStock.Data.Model;
using SimpleStock.Extensions;
using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class ShowItemList
    {
        private readonly PrimaryUi _ui;
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _services;

        private List<ItemWrapper> _items;
        private readonly ListView _itemList;

        private readonly Toplevel _top;
        private readonly Window _primaryWindow;
        private readonly Window _secondaryWindow;
        private readonly TextField _search;

        public ShowItemList(PrimaryUi ui, ApplicationDbContext db, IServiceProvider services)
        {
            _ui = ui;
            _db = db;
            _services = services;

            _items = new List<ItemWrapper>();

            var label = new Label(0, 0, "Filter: ");
            _search = new TextField(".*") {
                X = Pos.Right(label),
                Y = Pos.Top(label),
                Width = Dim.Fill(),
                Height = 1,
            };

            _top = new Toplevel
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                MenuBar = new MenuBar {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = 1,
                    Menus = _ui.ConfigureMenubar(
                        new MenuBarItem("_Parts List",
                            new MenuItem[] {
                                new("_Create New", "", CreateItemModal),
                                new("_Refresh", "", UpdateFilter),
                            }
                        )
                    ),
                }
            };

            _primaryWindow = new Window("Primary") {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Percent(75),
                Title = $"Title"
            };
            
            _primaryWindow.Add(label);

            _search.TextChanged += _ => UpdateFilter();
            _primaryWindow.Add(_search);

            _itemList = new ListView(_items) {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            _itemList.SetSource(_items);
            _itemList.SelectedItemChanged += a => SelectionChanged(a.Value as ItemWrapper);
            _primaryWindow.Add(_itemList);

            _secondaryWindow = new Window("Secondary") {
                X = 0,
                Y = Pos.Percent(75) + 1,
                Width = Dim.Fill(),
                Height = Dim.Percent(25)
            };

            _top.Add(_primaryWindow, _secondaryWindow, _top.MenuBar);
        }

        public void Enter()
        {
            SelectionChanged(null);
            UpdateFilter();

            _ui.RunModal(_top);
        }

        private void UpdateFilter()
        {
            var filterStr = _search.Text.ToString();

            _items = (from item in _db.Items.AsQueryable()
                      orderby item.Manufacturer, item.ProductNumber
                      select item)
                     .AsEnumerable()
                     .Select(a => new ItemWrapper(_db, a))
                     .ToArray()
                     .Where(a => string.IsNullOrEmpty(filterStr) || Regex.IsMatch(a.ToString(), filterStr))
                     .ToList();

            _itemList.SetSource(_items);

            _primaryWindow.Title = $"Showing {_items.Count} Items";
        }

        private void SelectionChanged(ItemWrapper? item)
        {
            _secondaryWindow.Clear();

            if (item == null)
            {
                _secondaryWindow.Title = "No Details";
                return;
            }

            SetSecondaryTitle(item);

            var openDatasheet = new Button("_Open Datasheet") {
                X = 0,
                Y = 0,
            };
            openDatasheet.Clicked += () => OpenDatasheet(item);
            _secondaryWindow.Add(openDatasheet);

            var add = new Button("_Add") {
                X = Pos.Right(openDatasheet),
                Y = 0,
            };
            add.Clicked += () => AddStock(item);
            _secondaryWindow.Add(add);

            var remove = new Button("_Remove") {
                X = Pos.Right(add),
                Y = 0,
            };
            remove.Clicked += () => RemoveStock(item);
            _secondaryWindow.Add(remove);
            
            _secondaryWindow.Add(new Label(item.Item.Description) {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            });
        }

        private void SetSecondaryTitle(ItemWrapper item)
        {
            var quantity = item.Stock;
            var quantityStr = quantity == 0
                            ? "OUT OF STOCK"
                            : $"Quantity:{quantity}";

            _secondaryWindow.Title = $"{item.Item.Name} ({item.Item.Manufacturer}:{item.Item.ProductNumber}) - {quantityStr}";
        }

        private static void OpenDatasheet(ItemWrapper item)
        {
            Process.Start(new ProcessStartInfo(item.Item.DatasheetUrl) { UseShellExecute = true });
        }

        private void AddStock(ItemWrapper item)
        {
            var add = _services.GetRequiredService<AddStockModal>();
            var result = add.Enter();

            if (result.HasValue)
            {
                _db.Add(new Transaction {
                    Date = DateTime.UtcNow,
                    Delta = result.Value.Count,
                    Message = result.Value.Message,
                    StockItemId = item.Item.ID
                });
                _db.SaveChanges();

                SelectionChanged(item);
            }
        }

        private void RemoveStock(ItemWrapper item)
        {
            var remove = _services.GetRequiredService<RemoveStockModal>();
            var result = remove.Enter();

            if (result.HasValue)
            {
                _db.Add(new Transaction {
                    Date = DateTime.UtcNow,
                    Delta = -result.Value.Count,
                    Message = result.Value.Message,
                    StockItemId = item.Item.ID
                });
                _db.SaveChanges();

                SelectionChanged(item);
            }
        }

        private void CreateItemModal()
        {
            var result = _services.GetRequiredService<CreateItemModal>().Enter();
            if (result == null)
                return;

            var duplicate = (from item in _db.Items
                             where item.Manufacturer == result.Value.Manufacturer
                             where item.ProductNumber == result.Value.ProductNumber
                             select item).SingleOrDefault();

            if (duplicate != null)
            {
                _services.GetRequiredService<DuplicateItemModal>().Enter(duplicate);
            }
            else
            {
                _db.Add(new StockItem {
                    DatasheetUrl = result.Value.DatasheetUrl,
                    Description = result.Value.Description,
                    Manufacturer = result.Value.Manufacturer,
                    Name = result.Value.Name,
                    ProductNumber = result.Value.ProductNumber
                });
                _db.SaveChanges();

                UpdateFilter();
            }
        }

        private class ItemWrapper
        {
            private readonly ApplicationDbContext _db;
            public StockItem Item { get; }

            public int Stock =>
                (from tsx in _db.Transactions
                 where tsx.StockItemId == Item.ID
                 select tsx.Delta).Sum();

            public ItemWrapper(ApplicationDbContext db, StockItem item)
            {
                _db = db;
                Item = item;
            }

            public override string ToString()
            {
                return $"{Item.Manufacturer.SliceExact(20)} | {Item.ProductNumber.SliceExact(20)} | {Stock.ToString().SliceExact(5)} | {Item.Name}";
            }
        }
    }
}
