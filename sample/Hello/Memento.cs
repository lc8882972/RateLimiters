using System;

namespace Hello
{
    // 备忘录模式
    class Originator
    {
        string state;
        public string State
        {
            get { return this.state; }
            set { this.state = value; }
        }
        public Memento CreateMemento()
        {
            return new Memento(this.state);
        }

        public void SetMemento(Memento memento)
        {
            this.state = memento.State;
        }

        public void Show()
        {
            Console.WriteLine($"State={this.state}");
        }
    }

    class Memento
    {
        private string state;
        public Memento(string state)
        {
            this.state = state;
        }

        public string State
        {
            get { return state; }
        }
    }

    class Caretaker
    {
        public Memento Memento { get; set; }
    }
}