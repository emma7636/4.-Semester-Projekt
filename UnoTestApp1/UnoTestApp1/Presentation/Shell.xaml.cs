using Uno.Toolkit.UI;

namespace UnoTestApp1.Presentation
{
    public sealed partial class Shell : UserControl, IContentControlProvider
    {
        public Shell()
        {
            this.InitializeComponent();
        }

        public ContentControl ContentControl => Splash;
    }
}