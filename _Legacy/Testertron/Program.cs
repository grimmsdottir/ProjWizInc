using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using System;
using System.Threading;

namespace Testertron {
    internal class Program {
        private static CoreContext _context = null!;
        private static bool _running = true;

        public static void Main(string[] args) {
            // 1. Boot the core context (This loads the split JSON files from bin/Debug/net8.0/defs/)
            _context = Bootstrapper.BuildContext();

            // 2. Subscribe to the render frame (the 30 PPS pulse) to redraw the screen
            _context.Subscribe<UpdateRenderEvent>(OnRenderPulse);

            // 3. Start the asynchronous background tick loops
            _context.Start();

            // Hide the terminal cursor to prevent blinking visual artifacts
            Console.CursorVisible = false;
            Console.Clear();

            // 4. Main Non-Blocking Input Loop
            while (_running) {
                // Check if a key was pressed without blocking the thread
                if (Console.KeyAvailable) {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    ProcessInput(keyInfo);
                }

                // Yield the CPU slightly (sleep 50ms) to keep CPU core usage near 0%
                Thread.Sleep(50);
            }

            // Cleanup on termination
            _context.Unsubscribe<UpdateRenderEvent>(OnRenderPulse);
            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine("Headless engine terminated cleanly.");
        }

        private static void OnRenderPulse(UpdateRenderEvent e) {
            // Resets the draw cursor to the top-left corner instead of clearing the screen.
            // This achieves standard, flicker-free rendering inside the console.
            Console.SetCursorPosition(0, 0);

            long time = _context.GetTimeState().TimeElapsed;
            Console.WriteLine("==================================================");
            Console.WriteLine("         PROJECT WIZARD INCREMENTAL - TERMINAL    ");
            Console.WriteLine("==================================================");
            Console.WriteLine(" Time Elapsed: " + time.ToString() + "s");
            Console.WriteLine("--------------------------------------------------");

            // Render Resources
            Console.WriteLine(" INVENTORY:");
            int resourceCount = _context.GetResourceCount();
            for (int i = 0; i < resourceCount; i++) {
                ResourceDefinition def = _context.GetResourceDefinition(i);
                BigNum amount = _context.GetResourceAmount(i);
                Console.WriteLine("  - " + def.DisplayName + ": " + amount.ToString());
            }
            Console.WriteLine("--------------------------------------------------");

            // Render Jobs
            Console.WriteLine(" AVAILABLE TASKS:");
            int jobCount = _context.GetJobCount();
            JobState jobState = _context.GetJobState();
            int activeJobId = jobState.ActiveJobId;

            for (int i = 0; i < jobCount; i++) {
                JobDefinition def = _context.GetJobDefinition(i);
                string status = "Idle  ";
                string progress = "0%  ";

                if (i == activeJobId) {
                    status = "ACTIVE";
                    if (jobState.JobTicksRequired != null) {
                        double current = 0.0;
                        double required = 1.0;

                        // Parse BigNum limits into doubles for display percentage
                        if (!jobState.Ticks.IsLarge) {
                            current = jobState.Ticks.Small;
                        } else {
                            current = jobState.Ticks.Man * Math.Pow(10, jobState.Ticks.Exp);
                        }

                        if (!jobState.JobTicksRequired.RequiredTicks.IsLarge) {
                            required = jobState.JobTicksRequired.RequiredTicks.Small;
                        } else {
                            required = jobState.JobTicksRequired.RequiredTicks.Man * Math.Pow(10, jobState.JobTicksRequired.RequiredTicks.Exp);
                        }

                        double ratio = current / required;
                        progress = ((int)(ratio * 100)).ToString() + "%   ";
                    }
                }

                Console.WriteLine("  [" + (i + 1).ToString() + "] " + def.DisplayName.PadRight(15) + " (" + status + ") - Progress: " + progress);
            }
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(" Controls: Press [1], [2], [3] to Toggle Jobs.    ");
            Console.WriteLine("           Press [Q] to Quit.                     ");
            Console.WriteLine("==================================================");
        }

        private static void ProcessInput(ConsoleKeyInfo keyInfo) {
            char keyChar = keyInfo.KeyChar;

            if (keyChar == 'q' || keyChar == 'Q') {
                _running = false;
            } else if (keyChar == '1') {
                _context.ToggleJob(0);
            } else if (keyChar == '2') {
                _context.ToggleJob(1);
            } else if (keyChar == '3') {
                _context.ToggleJob(2);
            }
        }
    }
}