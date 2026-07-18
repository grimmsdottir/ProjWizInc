using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels {
    public partial class MainWindowViewModel : ViewModelBase {
        public string Greeting { get; } = "Welcome to Project Wizard Incremental";

        private readonly CoreContext _context;

        [ObservableProperty]
        private string _timeDisplay = "Time: 0";

        public ObservableCollection<ResourceVM> Resources { get; } = new ObservableCollection<ResourceVM>();
        public ObservableCollection<JobVM> Jobs { get; } = new ObservableCollection<JobVM>();
        //so apparently we need 2 constructors, because the design-time view model needs to be able to instantiate
        //without parameters,but the runtime view model needs the context. So we provide a default constructor
        //that calls the other one with a built context.
        public MainWindowViewModel() : this(Bootstrapper.BuildContext()) { }
        public MainWindowViewModel(CoreContext context) {
            _context = context;

            if (Avalonia.Controls.Design.IsDesignMode) {
                return;
            }

            // 1. Hydrate dynamic Resource UI elements based on definitions
            int resourceCount = _context.GetResourceCount();
            for (int i = 0; i < resourceCount; i++) {
                ResourceDefinition def = _context.GetResourceDefinition(i);
                ResourceVM resourceVM = new ResourceVM(i, def.DisplayName);
                Resources.Add(resourceVM);
            }

            // 2. Hydrate dynamic Job elements based on definitions
            int jobCount = _context.GetJobCount();
            for (int i = 0; i < jobCount; i++) {
                JobDefinition def = _context.GetJobDefinition(i);
                ToggleJobCommand command = new ToggleJobCommand(this, i);
                JobVM jobVM = new JobVM(i, def.DisplayName, command);
                Jobs.Add(jobVM);
            }

            // Subscribe to render frame updates
            _context.Subscribe<UpdateRenderEvent>(OnRenderPulse);
        }

        public void ToggleJob(int jobId) {
            _context.ToggleJob(jobId);
        }

        private void OnRenderPulse(UpdateRenderEvent e) {
            Dispatcher.UIThread.Post(UpdateVisualElements);
        }

        private void UpdateVisualElements() {
            // Update Time Track
            long elapsedSeconds = _context.GetTimeState().TimeElapsed;
            TimeDisplay = "Time: " + elapsedSeconds.ToString();

            // Update Resource Amounts
            for (int i = 0; i < Resources.Count; i++) {
                ResourceVM resourceVM = Resources[i];
                BigNum amount = _context.GetResourceAmount(resourceVM.Id);
                resourceVM.AmountDisplay = amount.ToString();
            }

            // Update Job execution states and progress bars
            JobState jobState = _context.GetJobState();
            int activeId = jobState.ActiveJobId;

            for (int i = 0; i < Jobs.Count; i++) {
                JobVM jobVM = Jobs[i];
                if (jobVM.Id == activeId) {
                    jobVM.StatusText = "Stop";

                    if (jobState.JobTicksRequired != null) {
                        double currentTicks = 0.0;
                        double requiredTicks = 1.0;

                        // Safely parse BigNum components into double coordinates for the UI Progress Bar
                        if (!jobState.Ticks.IsLarge) {
                            currentTicks = jobState.Ticks.Small;
                        } else {
                            currentTicks = jobState.Ticks.Man * Math.Pow(10, jobState.Ticks.Exp);
                        }

                        if (!jobState.JobTicksRequired.RequiredTicks.IsLarge) {
                            requiredTicks = jobState.JobTicksRequired.RequiredTicks.Small;
                        } else {
                            requiredTicks = jobState.JobTicksRequired.RequiredTicks.Man * Math.Pow(10, jobState.JobTicksRequired.RequiredTicks.Exp);
                        }

                        double ratio = currentTicks / requiredTicks;
                        if (ratio > 1.0) ratio = 1.0;
                        if (ratio < 0.0) ratio = 0.0;

                        jobVM.ProgressValue = ratio;
                        jobVM.ProgressText = ((int)(ratio * 100)).ToString() + "%";
                    } else {
                        jobVM.ProgressValue = 0.0;
                        jobVM.ProgressText = "0%";
                    }
                } else {
                    jobVM.StatusText = "Start";
                    jobVM.ProgressValue = 0.0;
                    jobVM.ProgressText = "0%";
                }
            }
        }
    }
}