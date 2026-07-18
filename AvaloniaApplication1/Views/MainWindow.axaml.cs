using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjWizInc.Core.Managers;

namespace AvaloniaApplication1.Views {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        // Manual XAML loader fallback to bypass IDE source-generator compilation issues
        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}