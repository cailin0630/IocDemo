using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace UnityDemo
{
    internal class Program
    {
        public Program(ICalculator[] myCalculator)
        {
            myCalculator.FirstOrDefault(p => p.id == 2)?.Calculate();
        }

        [Dependency]
        public ICalculator myCalculator { get; set; }

        private static void Main(string[] args)
        {
            #region 类型的配置容器注册

            var myContainer = new UnityContainer();
            myContainer.RegisterType<ICalculator, MyCalculator>(nameof(MyCalculator));
            myContainer.RegisterType<ICalculator, MyCalculator2>(nameof(MyCalculator2));
            ServiceLocator.SetLocatorProvider(()=>new UnityServiceLocator(myContainer));
            var mycalculator= ServiceLocator.Current.GetInstance<ICalculator>(nameof(MyCalculator));
            var mycalculator2 = myContainer.Resolve<ICalculator>(nameof(MyCalculator2));
            var mycalculators = ServiceLocator.Current.GetAllInstances<ICalculator>();
            foreach (var p in mycalculators)
            {
                p.Calculate();
            }
            var b = mycalculator.Equals(mycalculator2);
            mycalculator.Calculate();
            mycalculator2.Calculate();

            #endregion 类型的配置容器注册

            #region 已有对象实例的配置容器注册

            //var myContainer = new UnityContainer();
            //var myCalculator = new MyCalculator();
            //myContainer.RegisterInstance<ICalculator>(myCalculator);
            //var myCalculatorInstence = myContainer.Resolve<ICalculator>();
            //myCalculatorInstence.Calculate();

            #endregion 已有对象实例的配置容器注册

            #region 构造函数注入

            //var myContainer = new UnityContainer();
            //myContainer.RegisterType<ICalculator, MyCalculator>(nameof(UnityDemo.MyCalculator));
            //myContainer.RegisterType<ICalculator, MyCalculator2>(nameof(MyCalculator2));

            //var pro = myContainer.Resolve<Program>();
            //pro.MyCalculator.Calculate();

            #endregion 构造函数注入

            #region 属性（设置器）注入

            //var myContainer = new UnityContainer();
            //myContainer.RegisterType<ICalculator, MyCalculator>();

            //var pro = myContainer.Resolve<Program>();
            //pro.myCalculator.Calculate();

            #endregion 属性（设置器）注入

            Console.ReadLine();
        }
    }

    public interface ICalculator
    {
        int id { get; set; }

        void Calculate();
    }

    public class MyCalculator : ICalculator
    {
        public int id { get; set; } = 1;

        public void Calculate()
        {
            Console.WriteLine("this is MyCalculator");
        }
    }

    public class MyCalculator2 : ICalculator
    {
        public int id { get; set; } = 2;

        public void Calculate()
        {
            Console.WriteLine("this is MyCalculator2");
        }
    }
}