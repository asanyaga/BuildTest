using System.Windows;
using System.Windows.Interactivity;

namespace Distributr.WPF.UI.Utility.FormBorderButtons
{
    public sealed class HideCloseButtonBehaiviour
    : Behavior<Window>
    {
        private CloseButtonHider hider;

        protected override void OnAttached()
        {
            this.hider = new CloseButtonHider(this.AssociatedObject);

            this.hider.Hide();

            base.OnAttached();
        }
        protected override void OnDetaching()
        {
            this.hider.Show();

            base.OnDetaching();
        }
    }
}
