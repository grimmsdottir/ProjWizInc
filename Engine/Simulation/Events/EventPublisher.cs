
using ProjWizInc.Engine.State.Records;
using ProjWizInc.Engine.State.Records.Interfaces;

namespace ProjWizInc.Engine.Simulation.Events {
    public class EventPublisher {
        private readonly IPublishEvents _publisher;
        //holds an interface reader to just about every record
        private readonly IReadEconomy _economy;
        //snapshots for every record
        private readonly EconomyRecord _economySnapshot;
        public EventPublisher(
            IPublishEvents publisher,
            IReadEconomy economy
            ) {
            _publisher = publisher;
            _economy = economy;
            _economySnapshot = new EconomyRecord(economy.GetResourceCount());
            Snapshot();
        }
        public void Snapshot() {
            _economy.CopyTo( _economySnapshot );
        }
        public void PublishDelta(EventHub eventHub) {
            //place all our count based/aggregatable events here 
        }
    }
}
