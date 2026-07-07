using ProjWizInc.Core;
using System.Diagnostics;

namespace ProjWizInc.WinForms
{
    public partial class Form1 : Form {
        private GameLoopManager _loopManager;
        private TaskLogic _taskLogic;
        private System.Windows.Forms.Timer _uiFrameTimer;
        private Stopwatch _frameStopwatch;
        private const double _SPEED_MULT = 1;

        public Form1() {
            InitializeComponent();
        }
        private void InitializeGameArchitecture() {
            EventBroker eventBroker = new();
            _taskLogic = new TaskLogic();
            _loopManager = new GameLoopManager(_taskLogic,eventBroker);

            eventBroker.Subscribe("RenderFrameRequested", (data) => RefreshUiDisplay());

            _frameStopwatch = new Stopwatch();
            _frameStopwatch.Start();

            _uiFrameTimer = new System.Windows.Forms.Timer();
            _uiFrameTimer.Interval = 16;
            _uiFrameTimer.Tick += OnVisualFrameTick;
            _uiFrameTimer.Start();
        }
        private void OnVisualFrameTick(Object sender, EventArgs e) {
            double realSecondsPassed = _frameStopwatch.Elapsed.TotalSeconds;
            _frameStopwatch.Restart();
            _loopManager.Update(realSecondsPassed, _SPEED_MULT);
        }
        private void RefreshUiDisplay() {
            double goldCount = _taskLogic.CurrentTask.Gold;
            double currentProgress = _taskLogic.CurrentTask.Progress;
            double duration = _taskLogic.CurrentTask.Duration;

            labelGold.Text = $"Gold: {goldCount}";

            double progressPercent = currentProgress / duration * 100;
            if (progressPercent <0) { progressPercent = 0; }
            if (progressPercent > 100) { progressPercent = 100; }

            barProgress.Value = (int)progressPercent;
        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
