using System.Timers;

namespace slopbot;

public class BackgroundProcess
{
    public static void Run()
    {
        var config = Config.Get();
        var min = config.Timer ?? 60;
        var time = min * 60 * 1000;

        Console.WriteLine($"Background process running ever {min} min");

        var timer = new System.Timers.Timer(time);
        timer.Elapsed += new ElapsedEventHandler(EventHandler);
        timer.Start();

        while (true)
        { }
    }

    private static void EventHandler(object source, ElapsedEventArgs args)
    {
        Console.WriteLine($"Running at {DateTime.Now}\n");
        Program.GenerateModel();
    }
}