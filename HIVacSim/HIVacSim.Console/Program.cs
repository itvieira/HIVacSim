// ----------------------------------------------------------------------------
// <copyright file="Program.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Console
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Console application showing how to use the HIVacSim model
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Creates an instance of the simulation and attach event handlers
            Simulation sim = new Simulation();
            sim.OnStartRun += sim_OnStartRun;
            sim.OnEndRun += sim_OnEndRun;

            // Update run-time settings
            sim.AutoSeed = true;    // Use auto-generated seed
            sim.Speed = 100;        // Run at maximum speed, no delay

            // Open and validate an existing scenario definition
            sim.OpenScenario(".","..\\..\\Data\\SingleGroupScenario.xml");
            if (sim.ValidateScenario() == true)
            {
                // Print out scenario details
                Console.WriteLine("HIVacSim Model [{0}]", sim.ShortFileName);
                Console.WriteLine(
                        "Scenario loaded successfully, population size: {0}.", 
                        sim.Data.Size);

                // Run simulation asynchronously
                sim.SimRun();

                // Waits for the simulation to complete
                while(sim.Status != ESimStatus.Completed)
                {
                    Thread.Sleep(10);
                }

                // Prints out total run-time
                Console.WriteLine(
                                   "{0}Elapsed time = {1} seconds", 
                                   Environment.NewLine, 
                                   sim.Time);

                // Process / export results
                // sim.Exportxxx
            }
            else
            {
                Console.WriteLine("Validation error: {0}", sim.ErrorLog);
            }

            // Close scenario file
            if (sim.IsFileOpen == true)
            {
                sim.CloseScenario();
            }

            Console.WriteLine("\nPress <ENTER> to exit.");
            Console.ReadLine();
        }

        static void sim_OnStartRun(object sender, EventArgs e)
        {
            Simulation sim = sender as Simulation;
            Console.Write("Run # {0,2} of {1} ... ", sim.Run, sim.Runs);
        }

        static void sim_OnEndRun(object sender, SimEventArgs e)
        {
            Console.WriteLine(e.Mensage);
        }
    }
}
