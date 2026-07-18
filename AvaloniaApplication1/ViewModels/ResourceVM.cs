using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels {
    public class ResourceVM : ViewModelBase {
        private string _amountDisplay;

        public string Name { get; }
        public int Id { get; }

        public string AmountDisplay {
            get {
                return _amountDisplay;
            }
            set {
                if (_amountDisplay != value) {
                    _amountDisplay = value;
                    OnPropertyChanged(nameof(AmountDisplay));
                }
            }
        }

        public ResourceVM(int id, string name) {
            Id = id;
            Name = name;
            _amountDisplay = "0";
        }
    }
}
