using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;

namespace AvaloniaApplication1.ViewModels {
    public partial class MainWindowViewModel : ViewModelBase {
        public string Greeting { get; } = "Welcome to Avalonia!";
        private readonly ContextManager _context;
        [ObservableProperty]
        private string _timeDisplay = "Time: 0";
        public MainWindowViewModel() {
            if (Avalonia.Controls.Design.IsDesignMode) return;

            ContextManager.Instance.Subscribe<UpdateRenderEvent>(OnRenderPulse);
        }
        private void OnRenderPulse(UpdateRenderEvent e) {
            // Polling the core logic
            var time = ContextManager.Instance.GetTimeState().TimeElapsed;

            // Marshaling back to the UI thread
            Dispatcher.UIThread.Post(() =>
            {
                TimeDisplay = $"Time: {time}";
            });
        }
    }
}
