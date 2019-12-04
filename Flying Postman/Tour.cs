using System;
using System.Collections.Generic;
using System.IO;

namespace Flying_Postman
{
    /// <summary>
    /// The tour class, everything related with the tour object and itinerary go here.
    /// This includes creation of Tour object, calculating the time between stations, 
    /// calculating the various different distances, calculating the itinerary information,
    /// dealing with displaying the itinerary and saving the itinerary.
    /// This class also has the wrapper function for the different levels of the tour.
    /// 
    /// Author Perdana Bailey May 2019
    /// </summary>
    public class Tour
    {
        public TimeSpan time;
        public double length;
        public TimeSpan elapsedTime;
        public List<object> tour;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlyingPostman.Tour"/> class.
        /// </summary>
        /// <param name="timeLength">Length of tour as a TimeSpan</param>
        /// <param name="distanceLength">Distance of tour as a decimal</param>
        /// <param name="algoTime">Length of algorithm time as a TimeSpan</param>
        /// <param name="itinerary">List of objects which hold the itinerary data</param>
        public Tour(TimeSpan timeLength, double distanceLength, TimeSpan algoTime, List<object> itinerary)
        {
            time = timeLength;
            length = distanceLength;
            elapsedTime = algoTime;
            tour = itinerary;
        }

        /// <summary>
        /// Calculates the complete tour distance.
        /// </summary>
        /// <returns>The tour distance as a decimal</returns>
        /// <param name="tour">The tour as a list of stations</param>
        public static double CalcTourDistance(List<Station> tour)
        {
            // Initialise the distances list for overall tour distance
            double distance = 0;

            // Calculate overall tour distance
            for (int i = 0; i < tour.Count - 1; i++)
            {
                // Add distance between stations
                distance += Station.CalcDistance(tour[i], tour[i + 1]);
            }

            return distance;

        }

        /// <summary>
        /// Calculates the distance between each station.
        /// </summary>
        /// <returns>A list of the distances as a decimal between the stations.</returns>
        /// <param name="tour">The tour as a list of stations</param>
        public static List<double> CalcItinDistance(List<Station> tour)
        {
            // Initialise the distances list for overall tour distance
            List<double> distances = new List<double>();

            // Calculate overall tour distance
            for (int i = 0; i < tour.Count - 1; i++)
            {
                // add distance between stations one at a time to distances list
                distances.Add(Station.CalcDistance(tour[i], tour[i + 1]));
            }

            return distances;
        }

        /// <summary>
        /// Calculates the timings for the Tour and make the tour object.
        /// </summary>
        /// <returns>The itinerary as a Tour object</returns>
        /// <param name="tour">The tour as a list of stations</param>
        /// <param name="planeSpec">The plane specs as a plane.</param>
        /// <param name="timeNow">Current time as a TimeSpan</param>
        /// <param name="algoTime">Length of algorithm time as a TimeSpan</param>
        public static Tour CalcItinTimes(List<Station> tour, Plane planeSpec, TimeSpan timeNow, TimeSpan algoTime)
        {
            // Set some basic variables
            Tour itinerary;
            List<object> itineraryObj = new List<object>();
            List<double> distances = CalcItinDistance(tour);
            double finalDistance = CalcTourDistance(tour);
            TimeSpan tourTime = new TimeSpan(0);
            TimeSpan startTime = timeNow;
            TimeSpan overRange = new TimeSpan(0);

            // Loop through each station pair
            for (int i = 0; i < tour.Count - 1; i++)
            {
                // Calculate the timing
                double distance = distances[i];
                TimeSpan endTime = Plane.CalcTime(startTime, distance, planeSpec);

                // Check if its within fuel range
                if (overRange + (endTime - startTime) > planeSpec.range)
                {
                    // Add the object with refuel to the itinerary list and update current values
                    itineraryObj.Add(new { refuel = "refuel", time = planeSpec.refuelTime });
                    tourTime += TimeSpan.FromMinutes(planeSpec.refuelTime);
                    startTime += TimeSpan.FromMinutes(planeSpec.refuelTime);
                    endTime = Plane.CalcTime(startTime, distance, planeSpec);
                    overRange = new TimeSpan(0);
                }
                // Add the object with leg info to the itinerary list and update current values
                itineraryObj.Add(new { startStation = tour[i], endStation = tour[i + 1], startT = startTime, endT = endTime });
                tourTime += (endTime - startTime);
                overRange += (endTime - startTime);
                startTime = endTime;
            }

            // Create and return a tour based on the timings
            itinerary = new Tour(tourTime, finalDistance, algoTime, itineraryObj);
            return itinerary;
        }

        /// <summary>
        /// Displays the itinerary.
        /// </summary>
        /// <param name="level">The level the itinerary data was completed at as an int.</param>
        /// <param name="tour">The tour information as a tour.</param>
        public static void DisplayItinerary(int level, Tour tour)
        {
            // Print the header
            Console.WriteLine("Optimising tour length: Level {0}...", level);
            Console.WriteLine("Elapsed time: {0:0.000} seconds", tour.elapsedTime.TotalSeconds);
            // Check days or not to display
            if (tour.time.Days == 0)
            {
                Console.WriteLine("Tour time: {0} ", tour.time.ToString("h' hours 'mm' minutes'"));
            }
            else
            {
                Console.WriteLine("Tour time: {0} ", tour.time.ToString("d' days 'h' hours 'mm' minutes'"));
            }
            Console.WriteLine("Tour length: {0}", tour.length.ToString("N4"));

            // Loop through each leg of the journey
            foreach (var data in tour.tour)
            {
                // Use this little hack to differentiate between a normal leg and a refuel leg
                try
                {
                    // Convert the object to the respected data types and print them correctly
                    Station startStation = (Station)data.GetType().GetProperty("startStation").GetValue(data, null);
                    Station endStation = (Station)data.GetType().GetProperty("endStation").GetValue(data, null);
                    TimeSpan startTime = (TimeSpan)data.GetType().GetProperty("startT").GetValue(data, null);
                    TimeSpan endTime = (TimeSpan)data.GetType().GetProperty("endT").GetValue(data, null);
                    Console.WriteLine("{0} \t -> \t {1}\t{2}\t{3}",
                        startStation.name,
                        endStation.name,
                        startTime.ToString("hh':'mm"),
                        endTime.ToString("hh':'mm"));
                }
                catch (Exception)
                {
                    // Convert the object to the respected data types and print them correctly
                    int time = (int)data.GetType().GetProperty("time").GetValue(data, null);
                    Console.WriteLine("*** refuel {0} minutes ***", time);
                }

            }
        }

        /// <summary>
        /// Saves the itinerary.
        /// </summary>
        /// <param name="level">The level the itinerary data was completed at as an int.</param>
        /// <param name="tour">The tour information as a tour.</param>
        /// <param name="filePath">Filepath of file to save to</param>
        public static void SaveItinerary(int level, Tour tour, string filePath)
        {
            // Initialise the writing to file and write the header
            StreamWriter file = new StreamWriter(filePath);
            file.WriteLine("Optimising tour length: Level {0}...", level);
            file.WriteLine("Elapsed time: {0:0.000} seconds", tour.elapsedTime.TotalSeconds);
            // Check days or not to display
            if (tour.time.Days == 0)
            {
                file.WriteLine("Tour time: {0} ", tour.time.ToString("h' hours 'mm' minutes'"));
            }
            else
            {
                file.WriteLine("Tour time: {0} ", tour.time.ToString("d' days 'h' hours 'mm' minutes'"));
            }
            file.WriteLine("Tour length: {0}", tour.length.ToString("N3"));

            // Loop through each leg of the journey
            foreach (var data in tour.tour)
            {
                // Use this little hack to differentiate between a normal leg and a refuel leg
                try
                {
                    // Convert the object to the respected data types and write them correctly
                    Station startStation = (Station)data.GetType().GetProperty("startStation").GetValue(data, null);
                    Station endStation = (Station)data.GetType().GetProperty("endStation").GetValue(data, null);
                    TimeSpan startTime = (TimeSpan)data.GetType().GetProperty("startT").GetValue(data, null);
                    TimeSpan endTime = (TimeSpan)data.GetType().GetProperty("endT").GetValue(data, null);
                    file.WriteLine("{0} \t -> \t {1}\t{2}\t{3}",
                        startStation.name,
                        endStation.name,
                        startTime.ToString("hh':'mm"),
                        endTime.ToString("hh':'mm"));
                }
                catch (Exception)
                {
                    // Convert the object to the respected data types and write them correctly
                    int time = (int)data.GetType().GetProperty("time").GetValue(data, null);
                    file.WriteLine("*** refuel {0} minutes ***", time);
                }

            }
            file.Close();
        }

        /// <summary>
        /// Checks the itinerary file is writeable.
        /// </summary>
        /// <param name="filePath">Filepath of file to save to</param>
        public static void CheckItinFile(string filePath)
        {
            // Test that its possible to write to the file and throw errors where needed
            try
            {
                StreamWriter file = new StreamWriter(filePath);
                file.Close();
            }
            catch (Exception e) when (e is DirectoryNotFoundException)
            {
                Console.WriteLine("Directory to save itinerary file not found. Ensure its a valid file path.");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Itinerary file cannot be read. Ensure the file has no special permissions.");
                throw;
            }
        }

        /// <summary>
        /// Tour wrapper Level 1-4 without saving.
        /// </summary>
        /// <param name="stations">List of stations</param>
        /// <param name="planeSpec">The plane specs</param>
        /// <param name="startTime">The tour starting time</param>
        public static void BeginTour(List<Station> stations, Plane planeSpec, TimeSpan startTime)
        {
            // If the amount of stations is 12 just brute force
            if (stations.Count <= 12)
            {
                // Time it and calculate the itinerary and display it
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<Station> l3Tour = Levels.LevelThree(stations);
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
                Tour tour = Tour.CalcItinTimes(l3Tour, planeSpec, startTime, elapsed);
                Tour.DisplayItinerary(3, tour);
            }
            else
            {
                // Do the prereq tours then time the final level and calculate the itinerary and display it
                List<Station> l1Tour = Levels.LevelOne(stations);
                List<Station> l2Tour = Levels.LevelTwo(l1Tour);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<Station> l4Tour = Levels.LevelFour(l2Tour);
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
                Tour tour = Tour.CalcItinTimes(l4Tour, planeSpec, startTime, elapsed);
                Tour.DisplayItinerary(4, tour);
            }
        }

        /// <summary>
        /// Tour wrapper Level 1-4 with saving.
        /// </summary>
        /// <param name="stations">List of stations</param>
        /// <param name="planeSpec">The plane specs</param>
        /// <param name="startTime">The tour starting time</param>
        /// <param name="filePath">Filepath of file to save to</param>
        public static void BeginTour(List<Station> stations, Plane planeSpec, TimeSpan startTime, string filePath)
        {
            if (stations.Count <= 12)
            {
                // Time it and calculate the itinerary and display it and save it
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<Station> l3Tour = Levels.LevelThree(stations);
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
                Tour tour = Tour.CalcItinTimes(l3Tour, planeSpec, startTime, elapsed);
                Tour.DisplayItinerary(3, tour);
                Tour.SaveItinerary(3, tour, filePath);
            }
            else
            {
                // Do the prereq tours then time the final level and calculate the itinerary and display it and save it
                List<Station> l1Tour = Levels.LevelOne(stations);
                List<Station> l2Tour = Levels.LevelTwo(l1Tour);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<Station> l4Tour = Levels.LevelFour(l2Tour);
                watch.Stop();
                TimeSpan elapsed = watch.Elapsed;
                Tour tour = Tour.CalcItinTimes(l4Tour, planeSpec, startTime, elapsed);
                Tour.DisplayItinerary(4, tour);
                Tour.SaveItinerary(4, tour, filePath);
            }
        }

        /// <summary>
        /// Tour wrapper Level 1-4 plus the bonus without saving.
        /// </summary>
        /// <param name="stations">List of stations</param>
        /// <param name="planeSpec">The plane specs</param>
        /// <param name="startTime">The tour starting time</param>
        /// <param name="level5">If set to <c>true</c> level5.</param>
        public static void BeginTour(List<Station> stations, Plane planeSpec, TimeSpan startTime, bool level5)
        {
            // Do the prereq tours then time the bonus level and calculate the itinerary and display it
            List<Station> l1Tour = Levels.LevelOne(stations);
            List<Station> l2Tour = Levels.LevelTwo(l1Tour);
            List<Station> l4Tour = Levels.LevelFour(l2Tour);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<Station> bonusTour = Levels.BonusLevel(l4Tour, planeSpec, startTime);
            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;
            Tour tour = Tour.CalcItinTimes(bonusTour, planeSpec, startTime, elapsed);
            Tour.DisplayItinerary(5, tour);
        }

        /// <summary>
        /// Tour wrapper Level 1-4 plus the bonus with saving.
        /// </summary>
        /// <param name="stations">List of stations</param>
        /// <param name="planeSpec">The plane specs</param>
        /// <param name="startTime">The tour starting time</param>
        /// <param name="filePath">Filepath of file to save to</param>
        /// <param name="level5">If set to <c>true</c> level5.</param>
        public static void BeginTour(List<Station> stations, Plane planeSpec, TimeSpan startTime, string filePath, bool level5)
        {
            // Do the prereq tours then time the bonus level and calculate the itinerary and display it and save it
            List<Station> l1Tour = Levels.LevelOne(stations);
            List<Station> l2Tour = Levels.LevelTwo(l1Tour);
            List<Station> l4Tour = Levels.LevelFour(l2Tour);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<Station> bonusTour = Levels.BonusLevel(l4Tour, planeSpec, startTime);
            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;
            Tour tour = Tour.CalcItinTimes(bonusTour, planeSpec, startTime, elapsed);
            Tour.DisplayItinerary(5, tour);
            Tour.SaveItinerary(5, tour, filePath);
        }
    } // end of Tour class
}
