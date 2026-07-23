using ProjWizInc.Engine.State.Records;
namespace ProjWizInc.Engine.State {
    public class RecordRoot{
        public EconomyRecord Economy;
        public JobRecord Job;
        public TimeRecord Time;
        public RecordRoot() {
            Economy = new EconomyRecord();
            Job = new JobRecord();
            Time = new TimeRecord();
        }
        public RecordRoot(
            EconomyRecord economy,
            JobRecord job,
            TimeRecord time
            ) {
            Economy = economy;
            Job = job;
            Time = time;
        }
    }
}
