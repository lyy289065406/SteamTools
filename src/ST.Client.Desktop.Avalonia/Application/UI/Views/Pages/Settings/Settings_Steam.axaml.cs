using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using System.Application.UI.ViewModels;

namespace System.Application.UI.Views.Pages
{
    public class Settings_Steam : UserControl
    {
        public Settings_Steam()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}