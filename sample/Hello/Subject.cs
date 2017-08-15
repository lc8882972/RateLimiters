using System;
using System.Collections.Generic;

namespace Hello
{
    delegate void EventHandler();
    // 观察者模式
    abstract class Subject
    {
        public event EventHandler Update;
        IList<Observer> observers = new List<Observer>();
        public void Attch(Observer observer)
        {
            this.observers.Add(observer);
        }
        public void Detach(Observer observer)
        {
            this.observers.Remove(observer);
        }
        public void Notify()
        {
            foreach (var observer in this.observers)
            {
                observer.update();
            }
        }
    }

    abstract class Observer
    {
        public abstract void update();
    }

    // class StockObserver : Observer
    // {
    //     public StockObserver(string name, Subject sub) : base(name, sub)
    //     {

    //     }

    //     public override void update()
    //     {
    //         // Console.WriteLine($"{subject.SubjectState}{name}关闭股票行情，继续工作！");
    //     }
    // }
}