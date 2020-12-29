using SimpleStock.Data.Model;
using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class AddStockModal
    {
        private readonly PrimaryUi _ui;

        public AddStockModal(PrimaryUi ui)
        {
            _ui = ui;
        }

        public Result? Enter()
        {
            var okpressed = false;

            var ok = new Button(3, 4, "Ok");
            ok.Clicked += () => {
                Application.RequestStop();
                okpressed = true;
            };

            var cancel = new Button(10, 4, "Cancel");
            cancel.Clicked += Application.RequestStop;

            var dialog = new Dialog("Add Items", 70, 8, ok, cancel);

            var countLabel = new Label("Count: ") { X = 3, Y = 1 };
            var messageLabel = new Label("Message: ")
            {
                X = Pos.Left(countLabel),
                Y = Pos.Top(countLabel) + 1
            };
            var countTxt = new TextField("")
            {
                X = Pos.Right(messageLabel),
                Y = Pos.Top(countLabel),
                Width = Dim.Fill() - 3
            };
            var messageTxt = new TextField("")
            {
                X = Pos.Left(countTxt),
                Y = Pos.Top(messageLabel),
                Width = Dim.Width(countTxt)
            };

            dialog.Add(countLabel, messageLabel, countTxt, messageTxt);

            _ui.RunModal(dialog);

            if (okpressed && ushort.TryParse(countTxt.Text.ToString(), out var parsed))
            {
                return new Result {
                    Count = parsed,
                    Message = messageTxt.Text.ToString() ?? ""
                };
            }

            return null;
        }

        public struct Result
        {
            public int Count;
            public string Message;
        }
    }
}
