using System;
using System.Collections.Generic;

namespace Flying_Postman
{
    /// <summary>
    /// The entry class for this project. The projects purpose is to solve the 
    /// TSP for CAB201 using various provided algorithms.
    /// 
    /// Author Perdana Bailey May 2019
    /// </summary>
    class Program
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            // Ensure the arguments are viable
            if (((args.Length > 3) && (args[3] == ("-o") || args[3] == "bonus" || (args.Length == 6 && args[5] == "bonus"))) || args.Length == 3)
            {
                // Attempt to parse the files
                Console.WriteLine("Reading input from {0}", args[0]);
                List<Station> stations = Station.FileParse(args[0]);
                Plane planeSpec = Plane.FileParse(args[1]);

                // Attempt to parse the time
                TimeSpan startTime;
                try
                {
                    startTime = TimeSpan.Parse(args[2]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse starting time. Ensure it is in the correct format.");
                    Console.WriteLine("For example: 20:00");
                    throw;
                }

                // Check if saving itinerary
                if (args.Length > 3 && args[3] == "-o")
                {
                    // Check if bonus level
                    if (args.Length == 6 && args[5] == "bonus")
                    {
                        // Test can write to file and begin the tour
                        Tour.CheckItinFile(args[4]);
                        Tour.BeginTour(stations, planeSpec, startTime, args[4], true);
                        Console.WriteLine("Saving itinerary to {0}", args[4]);
                    }
                    else
                    {
                        // Test can write to file and begin the tour
                        Tour.CheckItinFile(args[4]);
                        Tour.BeginTour(stations, planeSpec, startTime, args[4]);
                        Console.WriteLine("Saving itinerary to {0}", args[4]);
                    }
                }
                else
                {
                    // Check if bonus level
                    if (args.Length == 4 && args[3] == "bonus")
                    {
                        Tour.BeginTour(stations, planeSpec, startTime, true);
                    }
                    else
                    {
                        Tour.BeginTour(stations, planeSpec, startTime);
                    }
                }
            }
            else
            {
                // Cleanly throw errors
                Console.WriteLine("Incorrect number of arguments or incorrect arguments.");
                Console.WriteLine("The format is: <station file> <plane file> <time> -o <itinerary file>");
                throw new ArgumentException();
            }

        }
    } // end of Program class
}
