using System;
using System.IO;

namespace Flying_Postman
{
    /// <summary>
    /// The plane class, everything related with the plane object go here.
    /// This includes creation fo the plane object and parsing the file.
    /// 
    /// Author Perdana Bailey May 2019 
    /// </summary>
    public class Plane
    {
        public TimeSpan range;
        public int speed;
        public TimeSpan takeOffTime;
        public TimeSpan landingTime;
        public int refuelTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlyingPostman.Station"/> class.
        /// </summary>
        /// <param name="planeRange">plane range in hours as TimeSpan</param>
        /// <param name="planeSpeed">plane speed in (km/h) as int</param>
        /// <param name="planeTakeOff">plane take off time in minutes as TimeSpane</param>
        /// <param name="planeLanding">plane take off time in minutes as TimeSpane</param>
        /// <param name="planeRefuel">plane refuel time in minutes as int</param>
        public Plane(TimeSpan planeRange, int planeSpeed, TimeSpan planeTakeOff, TimeSpan planeLanding, int planeRefuel)
        {
            range = planeRange;
            speed = planeSpeed;
            takeOffTime = planeTakeOff;
            landingTime = planeLanding;
            refuelTime = planeRefuel;
        }

        /// <summary>
        /// Parses the input file and creates a plane object
        /// </summary>
        /// <returns>plane object</returns>
        /// <param name="filePath">Filepath of file to parse</param>
        public static Plane FileParse(string filePath)
        {
            string[] planeData = new string[5];
            string[] lines;
            Plane planeSpec;

            // Attempt to read the files lines to an array.
            try
            {
                lines = File.ReadAllLines(filePath);
                planeData = lines[0].Split(' ');
            }
            catch (Exception e) when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                Console.WriteLine("Plane file not found. Ensure the file exists and you are referencing the correct filepath.");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Plane file cannot be read. Ensure the format is txt (plain text) and the file has no special permissions.");
                throw;
            }

            // Check to make sure the file is the right format.
            if (planeData.Length != 5)
            {
                Console.WriteLine("Incorrect plane file formatting, does not have all the values or has too many values.");
                Console.WriteLine("The format is: <range> <speed> <take off time> <landing time> <refuel time>");
                Console.WriteLine("For example: 3 300 3 3 10");
                throw new FormatException();
            }
            else
            {
                // Attempt to convert the values and create the plane object.
                try
                {
                    TimeSpan planeRange = TimeSpan.FromHours(Convert.ToDouble(planeData[0]));
                    int planeSpeed = Convert.ToInt32(planeData[1]);
                    TimeSpan planeTakeOff = TimeSpan.FromMinutes(Convert.ToDouble(planeData[2]));
                    TimeSpan planeLanding = TimeSpan.FromMinutes(Convert.ToDouble(planeData[3]));
                    int planeRefuel = Convert.ToInt32(planeData[4]);

                    planeSpec = new Plane(planeRange, planeSpeed, planeTakeOff, planeLanding, planeRefuel);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse plane specifications, ensure they are numbers.");
                    Console.WriteLine("The format is: <range> <speed> <take off time> <landing time> <refuel time>");
                    Console.WriteLine("For example: 3 300 3 3 10");
                    throw;
                }

            }
            return planeSpec;
        }

        /// <summary>
        /// Calculates the time it takes for a <paramref name="plane"/> to go a <paramref name="distance"/> 
        /// with respect to <paramref name="currenttime"/>.
        /// </summary>
        /// <returns>Destination time as a TimeSpan</returns>
        /// <param name="currenttime">Current time as a TimeSpan</param>
        /// <param name="distance">Distance to travel in km as a decimal</param>
        /// <param name="plane">The plane specs as a plane</param>
        public static TimeSpan CalcTime(TimeSpan currenttime, double distance, Plane plane)
        { 
            // Calculate the time for in air flight, time = distance/speed
            TimeSpan time = TimeSpan.FromHours((double)(distance / plane.speed));
            // Add currenttime + air flight time + landing and take off time
            time = currenttime + time + plane.landingTime + plane.takeOffTime;
            return time;
        }
    } // end Plane class
}
