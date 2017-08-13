using System;

namespace Hello
{

  // 策略模式
  abstract class Strategy
  {
    public abstract void AlgorithmInterface();
  }

  class ConcreateStrategyA : Strategy
  {
    public override void AlgorithmInterface()
    {
      Console.WriteLine("算法A实现");
    }
  }

  class ConcreateStrategyB : Strategy
  {
    public override void AlgorithmInterface()
    {
      Console.WriteLine("算法B实现");
    }

  }

  class ConcreateStrategyC : Strategy
  {
      public override void AlgorithmInterface()
    {
      Console.WriteLine("算法C实现");
    }
  }

  class Context
  {
    Strategy strategy;

    public Context(Strategy strategy)
    {
      this.strategy = strategy;
    }

    public void ContextInterfact()
    {
      this.strategy.AlgorithmInterface();
    }
  }
}