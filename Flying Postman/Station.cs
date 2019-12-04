using System;
using System.Collections.Generic;
using System.IO;

namespace Flying_Postman
{
    /// <summary>
    /// The station class, everything related with the station object go here.
    /// This includes creation fo the Station object and parsing the file.
    /// 
    /// Author Perdana Bailey May 2019 
    /// </summary>
    public class Station
    {
        public string name;
        public int xCord;
        public int yCord;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlyingPostman.Station"/> class.
        /// </summary>
        /// <param name="stationName">Station name</param>
        /// <param name="xCoords">x coordinates of station</param>
        /// <param name="yCoords">y coordinates of station</param>
        public Station(string stationName, int xCoords, int yCoords)
        {
            name = stationName;
            xCord = xCoords;
            yCord = yCoords;
        }

        /// <summary>
        /// Parses the input file and creates station objects that are put into a list then returned.
        /// </summary>
        /// <returns>A list of stations</returns>
        /// <param name="filePath">Filepath of file to parse</param>
        public static List<Station> FileParse(string filePath)
        {
            // Pre-initialize list of stations to be returned
            List<Station> stations = new List<Station>();
            string[] lines;

            // Attempt to read the files lines to an array
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception e) when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                Console.WriteLine("Station file not found. Ensure the file exists and you are referencing the correct filepath.");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Station file cannot be read. Ensure the format is txt (plain text) and the file has no special permissions.");
                throw;
            }

            // Loop through each line the lines array
            foreach (var line in lines)
            {
                // Split the lines into each component
                string[] stationDetails = line.Split(' ');

                // Check to make sure the file is the right format
                if (stationDetails.Length != 3)
                {
                    Console.WriteLine("Incorrect station file formatting, does not have all the values or has too many values.");
                    Console.WriteLine("The format is: <station name> <x coord> <y coord>");
                    Console.WriteLine("For example: PostOffice 250 400");
                    throw new FormatException();
                }
                else
                {
                    // Attempt to convert the values and create the station object and add it to the list
                    try
                    {
                        string stationName = stationDetails[0];
                        int stationX = Convert.ToInt32(stationDetails[1]);
                        int stationY = Convert.ToInt32(stationDetails[2]);

                        Station station = new Station(stationName, stationX, stationY);
                        stations.Add(station);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse station coordinates, ensure they are numbers.");
                        Console.WriteLine("The format is: <station name> <x coord> <y coord>");
                        Console.WriteLine("For example: PostOffice 250 400");
                        throw;
                    }
                }
            }

            // Return the list of stations
            return stations;
        }

        /// <summary>
        /// Calculates the distance between two stations.
        /// </summary>
        /// <returns>The distance as a decimal</returns>
        /// <param name="station1">The first station</param>
        /// <param name="station2">The second station</param>
        public static double CalcDistance(Station station1, Station station2)
        {
            // Return a double version of eulidean distance
            return (double)Math.Sqrt(Math.Pow(station1.xCord - station2.xCord, 2)
                + Math.Pow(station1.yCord - station2.yCord, 2));
        }
    } // end Station class
}
