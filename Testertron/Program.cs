using System;
using System.Threading;
using ProjWizInc.Engine.Simulation.Core;
using ProjWizInc.Engine.Simulation.Bootstrapper;
using ProjWizInc.Engine.Data.Configs;

namespace ProjWizInc.Engine.ConsoleApp {
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("========================================");
            Console.WriteLine("  ProjWizInc: Idle Game Engine Boot  ");
            Console.WriteLine("========================================");
            Console.WriteLine();

            Console.WriteLine("Initializing CoreContext...");

            // 1. Create the CoreContext.
            // We set isRaw = true (to read the physical JSON defs folder)
            // and isNew = true (to initialize a fresh, non-loaded game session)
            CoreContext context = Bootstrapper.CreateCoreContext(true, true);

            Console.WriteLine("CoreContext initialized successfully.");
            Console.WriteLine("Starting simulation loop... Press any key to exit.");
            Console.WriteLine();

            // 2. Start the HeartbeatManager and the async game loop
            context.Start();

            // 3. Poll and display the active tick counter at a throttled rate
            while (!Console.KeyAvailable) {
                long elapsedTicks = context.GetTicks();

                // \r clears the current line in the console for an in-place update
                Console.Write("\r[Active Simulation] Ticks Elapsed: " + elapsedTicks.ToString());

                // Poll 10 times a second to prevent heavy console write overhead
                Thread.Sleep(100);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Key pressed. Shutting down engine...");
            Console.WriteLine("Engine terminated.");
        }
    }
}