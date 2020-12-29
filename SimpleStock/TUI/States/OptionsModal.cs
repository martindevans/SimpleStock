using Terminal.Gui;

namespace SimpleStock.TUI.States
{
    public class OptionsModal
    {
        private readonly PrimaryUi _ui;

        public OptionsModal(PrimaryUi ui)
        {
            _ui = ui;
        }

        public void Enter()
        {
            var ok = new Button(3, 14, "Ok");
            ok.Clicked += Application.RequestStop;

            var dialog = new Dialog("Options", 60, 18, ok);

            _ui.RunModal(dialog);
        }
    }
}
