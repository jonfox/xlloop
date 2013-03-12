using System;

using Trafigura.XLLoop;

namespace cstestharness {
    class Program {
        static void Main(string[] args) {
            var myClassInt = new MyClassInt();
            var myClassString = new MyClassString();
            var myClassDouble = new MyClassDouble();
            var myClassCollections = new MyClassCollections();

            var rfh =
                new ReflectFunctionHandler("MyClassInt.", myClassInt).AddInstanceMethods("MyClassString.", myClassString)
                                                                     .AddInstanceMethods("MyClassDouble.", myClassDouble)
                                                                     .AddInstanceMethods("MyClassCollections.", myClassCollections);
            var fs = new FunctionServer(5455, rfh);
            fs.Start();
            Console.WriteLine("Started server...\nPress enter to terminate");
            Console.Read();
            fs.Stop();
        }
    }
}
