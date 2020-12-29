using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class CreateItemModal
    {
        private readonly PrimaryUi _ui;

        public CreateItemModal(PrimaryUi ui)
        {
            _ui = ui;
        }

        public Result? Enter()
        {
            var okpressed = false;

            var ok = new Button(3, 7, "Ok");
            ok.Clicked += () => {
                Application.RequestStop();
                okpressed = true;
            };

            var cancel = new Button(10, 7, "Cancel");
            cancel.Clicked += Application.RequestStop;

            var nameLabel = new Label("Name: ") { X = 3, Y = 1 };
            var manuLabel = new Label("Manufacturer: ")
            {
                X = Pos.Left(nameLabel),
                Y = Pos.Top(nameLabel) + 1
            };
            var codeLabel = new Label("Product Code: ")
            {
                X = Pos.Left(manuLabel),
                Y = Pos.Top(manuLabel) + 1
            };
            var dataLabel = new Label("Datasheet: ")
            {
                X = Pos.Left(codeLabel),
                Y = Pos.Top(codeLabel) + 1
            };
            var descLabel = new Label("Description: ")
            {
                X = Pos.Left(dataLabel),
                Y = Pos.Top(dataLabel) + 1
            };

            var nameText = new TextField("")
            {
                X = Pos.Right(manuLabel),
                Y = Pos.Top(nameLabel),
                Width = 48
            };
            var manuText = new TextField("")
            {
                X = Pos.Left(nameText),
                Y = Pos.Top(manuLabel),
                Width = Dim.Width(nameText)
            };
            var codeText = new TextField("")
            {
                X = Pos.Left(nameText),
                Y = Pos.Top(codeLabel),
                Width = Dim.Width(nameText)
            };
            var dataText = new TextField("")
            {
                X = Pos.Left(nameText),
                Y = Pos.Top(dataLabel),
                Width = Dim.Width(nameText)
            };
            var descText = new TextField("")
            {
                X = Pos.Left(nameText),
                Y = Pos.Top(descLabel),
                Width = Dim.Width(nameText)
            };

            var dialog = new Dialog("Create New Item", 70, 11, ok, cancel) {
                nameLabel, manuLabel, codeLabel, dataLabel, descLabel,
                nameText, manuText, codeText, dataText, descText
            };

            _ui.RunModal(dialog);

            if (!okpressed)
                return null;

            return new Result(
                dataText.Text.ToString()!,
                descText.Text.ToString()!,
                manuText.Text.ToString()!,
                nameText.Text.ToString()!,
                codeText.Text.ToString()!
            );
        }

        public readonly struct Result
        {
            public string DatasheetUrl { get; }
            public string Description { get; }
            public string Manufacturer { get; }
            public string Name { get; }
            public string ProductNumber { get; }

            public Result(string datasheetUrl, string description, string manufacturer, string name, string productNumber)
            {
                DatasheetUrl = datasheetUrl;
                Description = description;
                Manufacturer = manufacturer;
                Name = name;
                ProductNumber = productNumber;
            }
        }
    }
}
