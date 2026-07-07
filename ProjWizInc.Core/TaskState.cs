using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core
{
    public struct TaskState {
        public string Name { get; set; }
        public double Progress { get; set; }
        public double Duration { get; set; }
        public double Gold {  get; set; }
        public TaskState(string name, double duration) {
            Name = name;
            Duration = duration;
            Progress = 0;
            Gold = 0;
        }
    }
}
