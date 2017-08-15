using System;

namespace Hello
{
    // 命令模式
    public class Invoke
    {
        Command command;

        public Invoke(Command command)
        {
            this.command = command;
        }

        public void Execute()
        {
            this.command.Execute();
        }
    }

    public abstract class Command
    {
        protected  Receiver receiver;
        public Command(Receiver receiver)
        {
            this.receiver = receiver;
        }
        public abstract void Execute();
    }

    public class ConcreteCommand : Command
    {
        public ConcreteCommand(Receiver receiver) :base(receiver)
        {

        }
        public override void Execute()
        {
            this.receiver.Action();
        }
    }
    public class Receiver
    {
        public void Action()
        {
            Console.WriteLine("Run()");
        }
    }
}