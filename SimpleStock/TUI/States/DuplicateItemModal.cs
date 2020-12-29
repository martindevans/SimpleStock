using SimpleStock.Data.Model;
using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class DuplicateItemModal
    {
        private readonly PrimaryUi _ui;

        public DuplicateItemModal(PrimaryUi ui)
        {
            _ui = ui;
        }

        public void Enter(StockItem item)
        {
            var ok = new Button(3, 3, "Ok");
            ok.Clicked += Application.RequestStop;

            var dialog = new Dialog("Item Already Exists!", 77, 7, ok);

            var entry = new Label("An item with a duplicate manufacturer and product code already exists") {
                X = 1, 
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };
            dialog.Add(entry);

            _ui.RunModal(dialog);
        }
    }
}
