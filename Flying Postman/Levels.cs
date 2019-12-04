using System.Collections.Generic;
using System;

namespace Flying_Postman
{
    /// <summary>
    /// This is the levels class, everything related to the TSP algorithms go here.
    /// Levels 1-4 plus the bonus level.
    /// 
    /// Author Perdana Bailey May 2019
    /// </summary>
    public class Levels
    {
        /// <summary>
        /// Calculates level one TSP algorithm
        /// </summary>
        /// <returns>The tour as a list of stations</returns>
        /// <param name="stations">List of Stations</param>
        public static List<Station> LevelOne(List<Station> stations)
        {

            // Seed the final station list with both POs
            List<Station> mainTour = new List<Station>
            {
                stations[0],
                stations[0]
            };

            // Remove the post office as its already been inserted
            stations.RemoveAt(0);

            // Loop through each station
            foreach (Station testingStation in stations)
            {
                double minDist = double.MaxValue;
                int pos = 1;

                // Loop through each gap
                for (int j = 1; j < mainTour.Count; j++)
                {
                    // Calculate cheapest insertion
                    double impactDist =
                        Station.CalcDistance(mainTour[j - 1], testingStation) +
                        Station.CalcDistance(testingStation, mainTour[j]) -
                        Station.CalcDistance(mainTour[j - 1], mainTour[j]);

                    if (impactDist < minDist)
                    {
                        minDist = impactDist;
                        pos = j;
                    }
                }
                mainTour.Insert(pos, testingStation);
            }

            return mainTour;
        }

        /// <summary>
        /// Calculates level two TSP algorithm
        /// </summary>
        /// <returns>The tour as a list of stations</returns>
        /// <param name="currentTour">List of Stations from Level 1</param>
        public static List<Station> LevelTwo(List<Station> currentTour)
        {
            List<Station> mainTour = new List<Station>(currentTour.ToArray());
            int i = 1;
            while (i < currentTour.Count-1)
            {
                double tourDist = Tour.CalcTourDistance(mainTour);
                List<Station> testTour = new List<Station>(mainTour.ToArray());
                // Remove the candidate station 
                Station testingStation = testTour[i];
                double minDist = double.MaxValue;
                testTour.RemoveAt(i);

                // Loop through each gap
                int pos = 1;
                for (int j = 1; j < testTour.Count; j++)
                {
                    double impactDist =
                        Station.CalcDistance(testTour[j - 1], testingStation) +
                        Station.CalcDistance(testingStation, testTour[j]) -
                        Station.CalcDistance(testTour[j - 1], testTour[j]);

                    // If shorter redo the values change the final tour and restart
                    if (impactDist < minDist)
                    {
                        minDist = impactDist;
                        pos = j;
                    }
                }

                if (pos == i)
                {
                    i++;
                }
                else
                {
                    // Insert the new position and check distance
                    testTour.Insert(pos, testingStation);
                    if (Tour.CalcTourDistance(testTour) < tourDist)
                    {
                        mainTour = new List<Station>(testTour.ToArray());
                        // Reset loop
                        i = 1;
                    }
                    else
                    {
                        i++;
                    }
                }

            }

            return mainTour;

        }

        /// <summary>
        /// Calculates level three TSP algorithm, Heaps algorithm no recursion
        /// </summary>
        /// <returns>The tour as a list of stations</returns>
        /// <param name="stations">List of Stations</param>
        public static List<Station> LevelThree(List<Station> stations)
        {
            // Remove the PO and save it for later
            Station PO = stations[0];
            stations.Remove(PO);

            // Intialise some values
            int stationCount = stations.Count;
            double minDist = double.MaxValue;
            List<Station> bestTour = new List<Station>();

            // Set a condition per each station
            var indexes = new int[stationCount];
            for (int i = 0; i < stationCount; i++)
            {
                indexes[i] = 0;
            }

            // Loop through each station
            for (int i = 1; i < stationCount;)
            {
                if (indexes[i] < i)
                {
                    // Check if i is even (through a bitwise check) and swap values
                    if ((i & 1) == 1)
                    {
                        Station temp = stations[i];
                        stations[i] = stations[indexes[i]];
                        stations[indexes[i]] = temp;
                    }
                    else
                    {
                        Station temp = stations[i];
                        stations[i] = stations[0];
                        stations[0] = temp;
                    }

                    double permTourDist = Tour.CalcTourDistance(stations)
                        + Station.CalcDistance(PO, stations[0])
                        + Station.CalcDistance(stations[stations.Count - 1], PO);

                    // compare tour distance and update new best tour
                    if (permTourDist < minDist)
                    {
                        bestTour = new List<Station>(stations.ToArray());
                        minDist = permTourDist;
                    }

                    indexes[i]++;
                    i = 1;
                }
                else
                {
                    indexes[i++] = 0;
                }
            }

            // Reinsert post offices
            bestTour.Insert(0, PO);
            bestTour.Add(PO);
            return bestTour;
        }

        /// <summary>
        /// Calculates level four TSP algorithm
        /// </summary>
        /// <returns>The tour as a list of stations</returns>
        /// <param name="stations">List of Stations from Level 2</param>
        public static List<Station> LevelFour(List<Station> currenttour)
        {
            // Attempt 2-Opt improvement
            List<Station> bestTour = new List<Station>(currenttour.ToArray());
            bool improved = true;
            while (improved)
            {
                improved = false;
                double minDist = Tour.CalcTourDistance(bestTour);

                // Loop through and select two opposing pairs
                for (int i = 1; i < currenttour.Count-2; i++)
                {
                    for (int j = i+1; j < currenttour.Count-1; j++)
                    {
                        // reverse the pairs
                        List<Station> tempTour = new List<Station>();

                        for (int k = 0; k <= i - 1; k++)
                        {
                            tempTour.Add(bestTour[k]);
                        }

                        int reverse = 0;
                        for (var k = i; k <= j; k++)
                        {
                            tempTour.Add(bestTour[j-reverse]);
                            reverse++;
                        }

                        for (var k = j + 1; k < bestTour.Count; k++)
                        {
                            tempTour.Add(bestTour[k]);
                        }

                        // Check if the distance improves
                        if (Tour.CalcTourDistance(tempTour) < minDist)
                        {
                            bestTour = new List<Station>(tempTour.ToArray());
                            minDist = Tour.CalcTourDistance(tempTour);
                            improved = true;
                        } 
                    }
                }
            }

            return bestTour;
        }

        /// <summary>
        /// Calculates bonus level TSP algorithm using Cheapest insertion.
        /// </summary>
        /// <returns>The tour as a list of stations</returns>
        /// <param name="stations">List of Stations</param>
        public static List<Station> BonusLevel(List<Station> currentTour, Plane planeSpec, TimeSpan timeNow)
        {
            // Level 2 algo but check range
            List<Station> mainTour = new List<Station>(currentTour.ToArray());
            int i = 1;
            while (i < currentTour.Count - 1)
            {
                double tourDist = Tour.CalcTourDistance(mainTour);
                List<Station> testTour = new List<Station>(mainTour.ToArray());
                // Remove the candidate station 
                Station testingStation = testTour[i];
                double minDist = double.MaxValue;
                testTour.RemoveAt(i);

                // Loop through each gap
                int pos = 1;
                for (int j = 1; j < testTour.Count; j++)
                {
                    double impactDist =
                        Station.CalcDistance(testTour[j - 1], testingStation) +
                        Station.CalcDistance(testingStation, testTour[j]) -
                        Station.CalcDistance(testTour[j - 1], testTour[j]);

                    // If shorter change values for the final tour else check if range anyway
                    if (impactDist < minDist)
                    {
                        // Ensure the trip is within range
                        TimeSpan startTime = timeNow;
                        TimeSpan endTime = Plane.CalcTime(startTime, impactDist, planeSpec);
                        if ((endTime - startTime) <= planeSpec.range)
                        {
                            minDist = impactDist;
                            pos = j;
                        }
                    }
                    else
                    {
                        // Ensure the trip is within range
                        TimeSpan startTime = timeNow;
                        TimeSpan endTime = Plane.CalcTime(startTime, impactDist, planeSpec);
                        if ((endTime - startTime) <= planeSpec.range)
                        {
                            minDist = impactDist;
                            pos = j;
                        }
                    }
                }

                if (pos == i)
                {
                    i++;
                }
                else
                {
                    // Update the tour and check for an improvement then update the mainTour and reset the loops
                    testTour.Insert(pos, testingStation);
                    if (Tour.CalcTourDistance(testTour) < tourDist)
                    {
                        mainTour = new List<Station>(testTour.ToArray());
                        i = 1;
                    }
                    else
                    {
                        i++;
                    }
                }

            }

            return mainTour;
        }
    } // end of Levels class
}
