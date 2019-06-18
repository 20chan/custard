using System;
using CreamRoll;

namespace custard {
    class Program {
        static void Main(string[] args) {
            var server = new RouteServer<Server>(new Server());
            server.StartAsync();

            Console.WriteLine("press enter twice to stop..");

            Console.ReadLine();
            Console.ReadLine();

            server.Stop();
            Console.WriteLine("server stopped");
        }
    }
}