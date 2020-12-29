using System.Collections.Generic;
using System.Linq;
using SimpleStock.Data;
using SimpleStock.Data.Model;
using SimpleStock.Extensions;
using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class ShowLog
    {
        private readonly PrimaryUi _ui;
        private readonly ApplicationDbContext _db;

        private readonly Toplevel _top;
        private readonly ListView _itemList;

        private List<ItemWrapper> _items;

        public ShowLog(PrimaryUi ui, ApplicationDbContext db)
        {
            _ui = ui;
            _db = db;
            _items = new List<ItemWrapper>();

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
                        new MenuBarItem("_Log",
                            new MenuItem[] {
                                new("_Refresh", "", Refresh)
                            }
                        )
                    ),
                }
            };

            _itemList = new ListView(_items) {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            _itemList.SetSource(_items);

            _top.Add(_itemList, _top.MenuBar);
        }

        public void Enter()
        {
            Refresh();

            _ui.RunModal(_top);
        }

        private void Refresh()
        {
            _items = (from item in _db.Transactions.AsQueryable()
                      orderby item.Date descending
                      select item)
                     .AsEnumerable()
                     .Select(a => new ItemWrapper(_db, a))
                     .ToList();

            _itemList.SetSource(_items);
        }

        private class ItemWrapper
        {
            private readonly ApplicationDbContext _db;
            private readonly Transaction _tsx;

            private StockItem? _item;
            private StockItem? Item
            {
                get
                {
                    if (_item == null)
                    {
                        _item = (from item in _db.Items
                                 where item.ID == _tsx.StockItemId
                                 select item).FirstOrDefault();
                    }
                    return _item;
                }
            }

            public ItemWrapper(ApplicationDbContext db, Transaction transaction)
            {
                _db = db;
                _tsx = transaction;
            }

            public override string ToString()
            {
                var name = $"{Item?.Name ?? "Unknown Item!"} ({Item?.ProductNumber})".SliceExact(20);

                return $"{name} | {_tsx.Date.ToShortDateString()} {_tsx.Date.ToShortTimeString()} | {_tsx.Delta.ToString().SliceExact(5)} | {_tsx.Message}";
            }
        }
    }
}
