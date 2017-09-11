using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace MefDemoConsoleApp
{
    internal class Program
    {
        [Import(typeof(ICalculator))]
        public ICalculator Calculator { get; set; }

        private CompositionContainer _container;

        private Program()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            _container = new CompositionContainer(catalog);
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Main(string[] args)
        {
            Program p = new Program(); //Composition is performed in the constructor
            String s;
            Console.WriteLine("Enter Command:");
            while (true)
            {
                s = Console.ReadLine();
                Console.WriteLine(p.Calculator.Calculate(s));
            }
        }
    }

    public interface ICalculator
    {
        String Calculate(String input);
    }

    [Export(typeof(ICalculator))]
    public class MyCalculator : ICalculator
    {
        [ImportMany]
        private IEnumerable<Lazy<IOperation, IOperationData>> operations;

      

        [Export(typeof(IOperation))]
        [ExportMetadata("Symbol", '+')]
        class Add : IOperation
        {
            public int Operate(int left, int right)
            {
                return left + right;
            }
        }
        public String Calculate(String input)
        {
            int left;
            int right;
            Char operation;
            int fn = FindFirstNonDigit(input); //finds the operator  
            if (fn < 0) return "Could not parse command.";

            try
            {
                //separate out the operands  
                left = int.Parse(input.Substring(0, fn));
                right = int.Parse(input.Substring(fn + 1));
            }
            catch
            {
                return "Could not parse command.";
            }

            operation = input[fn];

            foreach (Lazy<IOperation, IOperationData> i in operations)
            {
                if (i.Metadata.Symbol.Equals(operation))
                    return i.Value.Operate(left, right).ToString();
            }
            return "Operation Not Found!";
        }
        private int FindFirstNonDigit(String s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!(Char.IsDigit(s[i]))) return i;
            }
            return -1;
        }

        [Export(typeof(IOperation))]
        [ExportMetadata("Symbol", '-')]
        class Subtract : IOperation
        {
            public int Operate(int left, int right)
            {
                return left - right;
            }
        }
    }

    public interface IOperation
    {
        int Operate(int left, int right);
    }

    public interface IOperationData
    {
        Char Symbol { get; }
    }
}