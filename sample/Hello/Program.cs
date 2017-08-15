using Bert.RateLimiters;
using System;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            // int x =10;
            IThrottleStrategy strategy = new FixedTokenBucket(100, 10, 1000);
            Throttler throttle = new Throttler(strategy);

            if (!throttle.CanConsume())
            {
                Console.WriteLine("CanConsume");
            }
            // 策略模式
            Context context = new Context(new ConcreateStrategyA());
            context.ContextInterfact();

            // 备忘录模式
            Originator originator = new Originator();
            originator.State = "on";
            originator.Show();

            Caretaker caretaker = new Caretaker();
            caretaker.Memento = originator.CreateMemento();
            originator.State = "off";
            originator.Show();

            originator.SetMemento(caretaker.Memento);
            originator.Show();

            // 命令模式
            Receiver receiver = new Receiver();
            Command command = new ConcreteCommand(receiver);
            Invoke invoke = new Invoke(command);
            invoke.Execute();

            Console.WriteLine("Hello World!");
        }
    }
}