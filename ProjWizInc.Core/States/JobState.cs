using ProjWizInc.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States {
    internal class JobState {
        public string Id { get; private set; }
        public ResourceType RewardType { get; private set; }
        public int Progress { get; set; }
        public int TicksRequired {  get; private set; }
        public long Payout {  get; private set; }
        public JobState(string id, ResourceType rewardType, int ticksRequired, long payout   ) {
            Id = id;
            RewardType = rewardType;
            TicksRequired = ticksRequired;
            Payout = payout;
        }

    }
}
