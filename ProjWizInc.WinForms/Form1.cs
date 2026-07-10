using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States;
using System.Diagnostics;

namespace ProjWizInc.WinForms
{
    public partial class Form1 : Form {
        private readonly ContextManager _context;

        public Form1() {
            InitializeComponent();
            _context = ContextManager.Instance;
            _context.Subscribe<UpdateRenderEvent>(UpdateUI);
            _context.Start();
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
        }
        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            _context.Unsubscribe<UpdateRenderEvent>(UpdateUI);
        }
        private void UpdateUI(UpdateRenderEvent e) {
            // Check if we need to marshal to the UI thread
            if (labelTimer.InvokeRequired) {
                labelTimer.BeginInvoke(() => UpdateUI(e));
                return;
            }
            long time = 0;
            labelTimer.Text = "Time: " + time;
        }
    }
}
