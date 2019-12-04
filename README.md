# Flying Postman 

A variation of the travelling salesperson problem for the CAB201 unit at QUT.
Flying Postman is a command line program that calculates the shortest tour distance between stations. It accepts a mail file, plane specification file, trip starting time (in 24-hour time format), and optionally an output file as input. The program is to produce an itinerary for the flight as the output on screen or both onscreen and as a file.

Level One is based off the Cheapest Insertion Heuristic.
Level Two is a modified Cheapest Insertion that uses Level Ones' solution to find a better solution.
Level Three is a non-recursive heaps algorithm, it generates all permutations and finds the best solution.
Level Four is based off the 2-Opt heuristic. It attempts to solve optimal solution.
Bonus Level uses the solution of Level Four and uses Level Twos' algorithm to calculate the optimal tour with each station within the planes range.

If the station file has 12 or less stations, by default it will run only Level 3 which should result in the optimal tour everytime.

## Running Specific Levels

To run only up to certain level (such as only level two), you will need to alter the code in Tour.cs, change BeginTour (depending on the required overload) to only run the levels you want.

By default the Bonus Level does not run. More info on that in the Usage section.

There are example tours and plane data with expected results in the examples folder for each level. 

### Examples information

The starting times for each set are as followed:
*     6 Stations: 23:00
*     12 Stations: 20:00
*     51 Stations: 09:00
*     101 Stations: 11:00
*     237 Stations: 00:00
*     1002 Stations: 12:00
*     105-Bonus Stations: 09:00

    
The examples optimal itinerarys' do not have a running time or level as they were not included in the assessment resources.

## Issues with the code

Sometimes there is a discrepency with the results as I used doubles for the data type.

Level Four attempts to solve the optimal solution but fails miserably, if given another opportunity I would have implemented a lin kerg algo instead of 2-Opt.

The choice of using Environment.Exit() as opposed to Throw error was because I wanted a cleaner exit even if it allowed for a slight code smell. The error messages are enough to see the issue without the need for a Throw error.

## Installation

Clone it, open it in Visual Studio and build it. Using the built executable run the usage commands in a terminal.

## Usage

Default usage:

```bash
"Flying Postman.exe" mail.txt plane-spec.txt 23:00
```

Saving itinerary to file:

```bash
"Flying Postman.exe" mail.txt plane-spec.txt 23:00 -o itinerary.txt
```

Running with the bonus level:

```bash
"Flying Postman.exe" mail.txt plane-spec.txt 23:00 bonus
```

Running with the bonus level and saving itinerary to file:

```bash
"Flying Postman.exe" mail.txt plane-spec.txt 23:00 -o itinerary.txt bonus
```

## Contributing

No contributions as this was a project for a university class that is now finished.

## License
[MIT](https://choosealicense.com/licenses/mit/)