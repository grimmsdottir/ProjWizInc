using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace AvaloniaApplication1.ViewModels {
    public class JobVM : ViewModelBase {
        private string _progressText;
        private double _progressValue;
        private string _statusText;

        public int Id { get; }
        public string Name { get; }

        public string ProgressText {
            get { return _progressText; }
            set {
                if (_progressText != value) {
                    _progressText = value;
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        public double ProgressValue {
            get { return _progressValue; }
            set {
                if (_progressValue != value) {
                    _progressValue = value;
                    OnPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        public string StatusText {
            get { return _statusText; }
            set {
                if (_statusText != value) {
                    _statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public ICommand ToggleCommand { get; }

        public JobVM(int id, string name, ICommand toggleCommand) {
            Id = id;
            Name = name;
            _progressText = "0%";
            _progressValue = 0.0;
            _statusText = "Start";
            ToggleCommand = toggleCommand;
        }
    }
}
