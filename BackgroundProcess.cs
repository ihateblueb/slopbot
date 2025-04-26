using System.Timers;

namespace slopbot;

public class BackgroundProcess
{
    static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    public static void Run()
    {
        Console.CancelKeyPress += (_, args) =>
        {
            _quitEvent.Set();
            args.Cancel = true;
        };

        var config = Config.Get();
        var min = config.Timer ?? 60;
        var time = min * 60 * 1000;

        Console.WriteLine($"Background process running ever {min} min");

        var timer = new System.Timers.Timer(time);
        timer.Elapsed += new ElapsedEventHandler(EventHandler);
        timer.Start();

        _quitEvent.WaitOne();

        timer.Stop();
        Console.WriteLine($"Background process stopping");
    }

    private static void EventHandler(object source, ElapsedEventArgs args)
    {
        Console.WriteLine($"Running at {DateTime.Now}\n");
        Program.GenerateModel();
    }
}