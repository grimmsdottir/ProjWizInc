using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.State.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Bootstrapper {
    /*
    * Interface Reader Pipeline:
    * Create an interface IReadThing, place getter contracts in IReadThing, implement getters in ThingRecord
    * How to use:
    * Create concrete ThingRecord, then use IReadThing readThing = ThingRecord. Automatically upcasted
    * Inject readThing into whatever needs it.
    * 
    * Mutator Delegate Pipeline:
    * Create mutators in ThingRecord
    * Create ThingMutators class, and create the mutator delegates with the same signature as the ThingRecord mutators
    * Declare public readonly delegates
    * Create constructor that sets the delegates
    * Create concrete ThingRecord
    * Declare thingMutator = new ThingMutators(thing.Mutators)
    * Inject thingMutator into however needs it
    */
    public class EconomyMutators {
        public delegate void AdjustResourceDelegate(int id, BigNum amount);
        public delegate bool TryAdjustResourceDelegate(int id, BigNum amount);
        public readonly AdjustResourceDelegate Adjust;
        public readonly TryAdjustResourceDelegate TryAdjust;
        public EconomyMutators(AdjustResourceDelegate adjust, TryAdjustResourceDelegate tryAdjust) {
            Adjust = adjust;
            TryAdjust = tryAdjust;
        }
    }
    public static partial class Bootstrapper {
        public static EconomyRecord CreateEconomyRecord(BigNum[] resources, out EconomyMutators mutators) {
            EconomyRecord record = new EconomyRecord(resources);
            mutators = new EconomyMutators(record.AdjustResource, record.TryAdjustResource);
            return record;

        }
    }
}
