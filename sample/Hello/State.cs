using System;

namespace Hello.State
{
    // 状态模式
    class Context
    {
        State state;
        public Context(State state)
        {
            this.state = state;
        }

        public State State
        {
            get;
            set;
        }

        public void Request()
        {
            state.Handle(this);
        }
    }

    abstract class State
    {
        public abstract void Handle(Context context);
    }

    class ConcreteStateA : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateB();
        }
    }

    class ConcreteStateB : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateA();
        }
    }
}