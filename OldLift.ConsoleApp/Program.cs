using System;

public class Program
{
    private const float MotorSpeed = 0.71f;
    private const float FloorHeight = 3.0f;
    private const int MinFloor = 1;
    private const int MaxFloor = 9;

    private static ILiftController _controller = null!;
    private static IAutomatedDoor _carDoor = null!;
    private static IAutomatedDoor[] _landingDoors = null!;
    private static ILandingButton[] _landingButtons = null!;
    private static IFloorButton[] _carFloorButtons = null!;
    private static IAlarmButton _carAlarmButton = null!;
    private static IBuzzer _buzzer = null!;
    private static ILogger _logger = null!;

    public static void Main(string[] args)
    {
        BuildLift();
        RunLoop();
    }

    private static void RunLoop()
    {
        Console.WriteLine("Welcome to the Old Lift console app!");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var command = input.Trim().Split(' ');
            var commandKey = command[0].ToLower();
            switch (commandKey)
            {
                case "help":
                    PrintHelp();
                    break;
                case "car":
                    HandleCarFloorButtonPress(command);
                    break;
                case "landing":
                    HandleLandingButtonPress(command);
                    break;
                case "alarm":
                    HandleAlarmButtonPress(command);
                    break;
                case "quit":
                case "exit":
                    return;
                default:
                    Console.WriteLine("Unknown command. Type 'help' for the list of commands.");
                    break;
            }
        }
    }

    private static void HandleAlarmButtonPress(string[] command)
    {
        if (command.Count() < 2)
        {
            PrintInvalidAlarmParameterMessage();
            return;
        }

        var commandParameters = command[1];
        if (commandParameters == "on")
        {
            _carAlarmButton.Press();
        }
        else if (commandParameters == "off")
        {
            _carAlarmButton.Release();
        }
        else
        {
            PrintInvalidAlarmParameterMessage();
        }
    }

    private static void HandleLandingButtonPress(string[] command)
    {
        var floor = ParseFloor(command);
        if (floor == null) return;

        _landingButtons[floor.Value - MinFloor].Press();
    }

    private static void HandleCarFloorButtonPress(string[] command)
    {
        var floor = ParseFloor(command);
        if (floor == null) return;

        _carFloorButtons[floor.Value - MinFloor].Press();
    }

    private static int? ParseFloor(string[] command)
    {
        if (command.Count() < 2)
        {
            PrintInvalidFloorParameterMessage();
            return null;
        }

        if (!int.TryParse(command[1], out int floor))
        {
            PrintInvalidFloorParameterMessage();
            return null;
        }

        if (floor < MinFloor || floor > MaxFloor)
        {
            PrintInvalidFloorParameterMessage();
            return null;
        }

        return floor;
    }

    private static void PrintInvalidFloorParameterMessage()
    {
        Console.WriteLine($"Please provide a valid floor parameter.");
        Console.WriteLine($"Min floor: {MinFloor}, Max floor: {MaxFloor}.");
    }

    private static void PrintInvalidAlarmParameterMessage()
    {
        Console.WriteLine("Unknown alarm parameter.");
        if (_buzzer.IsOn)
        {
            Console.WriteLine("Buzzer is on. To switch it off, type 'alarm off'.");
        }
        else
        {
            Console.WriteLine("Buzzer is off. To switch it on, type 'alarm on'.");
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("     quit                     - exit");
        Console.WriteLine("     alarm                    - press the alarm button in the car");
        Console.WriteLine("     car [floorNumber]        - press a floor button in the car");
        Console.WriteLine("     landing [floorNumber]    - press a landing button");
        Console.WriteLine("     status                   - current lift status");
    }

    private static void BuildLift()
    {
        _logger = new LiftLogger();
        _logger.Verbose = true;
        _buzzer = new Buzzer(_logger);
        _carAlarmButton = new AlarmButton(_buzzer, _logger);

        var totalFloors = MaxFloor - MinFloor + 1;
        _carDoor = new AutomatedDoor(_logger);
        _landingDoors = new AutomatedDoor[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _landingDoors[i] = new AutomatedDoor(_logger);
        }
        
        _controller = new LiftController(MinFloor, _carDoor, _landingDoors, MotorSpeed, FloorHeight);
        _carFloorButtons = new FloorButton[totalFloors];
        _landingButtons = new LandingButton[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            _carFloorButtons[i] = new FloorButton(i + MinFloor, _controller, _logger);
            _landingButtons[i] = new LandingButton(i + MinFloor, _controller, _logger);
        }
    }
}
