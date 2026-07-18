using System;
using System.Windows.Input;

namespace AvaloniaApplication1.ViewModels {
    public class ToggleJobCommand : ICommand {
        private readonly MainWindowViewModel _parent;
        private readonly int _jobId;

        public event EventHandler? CanExecuteChanged;

        public ToggleJobCommand(MainWindowViewModel parent, int jobId) {
            _parent = parent;
            _jobId = jobId;
        }

        public bool CanExecute(object? parameter) {
            return true;
        }

        public void Execute(object? parameter) {
            _parent.ToggleJob(_jobId);
        }
    }
}