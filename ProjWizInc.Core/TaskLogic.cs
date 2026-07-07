using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core {
    public class TaskLogic {
        public TaskState CurrentTask = new TaskState("Chop Wood",2);
        public void Update(double fixedDeltaTime) {
            CurrentTask.Progress += fixedDeltaTime;
            if (CurrentTask.Progress >= CurrentTask.Duration) {
                CurrentTask.Duration -= CurrentTask.Progress;
                CurrentTask.Gold += 10;
            }
        }
    }
}
