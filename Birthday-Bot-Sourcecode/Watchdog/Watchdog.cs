using System.Collections.Concurrent;

namespace Watchdog
{
    public class Watchdog
    {
        private ConcurrentDictionary<String, WatchedThread> _threads = new ConcurrentDictionary<String, WatchedThread>();

        private Watchdog()
        {

        }

        private static Watchdog _instance = new Watchdog();

        public static Watchdog Instance
        {
            get { return _instance; }
        }

        public void watch()
        {
            Console.WriteLine("Watchdog started");
            while (true)
            {
                foreach (String threadName in _threads.Keys)
                {
                    WatchedThread thread = _threads[threadName];
                    if (!((thread.thread.ThreadState == ThreadState.Running) || (thread.thread.ThreadState == ThreadState.Background) || (thread.thread.ThreadState == ThreadState.WaitSleepJoin)))
                    {
                        Console.WriteLine($"Thread {thread.name} stopped");
                        thread.restartFunction();
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public void addThread(WatchedThread wthread)
        {
            _threads[wthread.name] = wthread;
            Console.WriteLine($"Thread added to Watchdog: {wthread.name}");
        }
    }

    public struct WatchedThread
    {

        public WatchedThread(String _name, Thread _thread, Action _restartfunction)
        {
            thread = _thread;
            name = _name;
            restartFunction = _restartfunction;
        }

        public Thread thread { get; init; }
        public string name { get; init; }
        public Action restartFunction { get; init; }
    }
}
