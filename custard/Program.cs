using System;
using CreamRoll;

namespace custard {
    class Program {
        static void Main(string[] args) {
            var bundle = new BundleManager("bundles", "current.txt");

            var server = new RouteServer<Server>(new Server(), host: "localhost", port: 4000);
            var adminServer = new RouteServer<AdminServer>(new AdminServer(), host: "localhost", port: 4040);

            server.StartAsync();
            adminServer.StartAsync();

            Console.WriteLine("press enter twice to stop..");

            Console.ReadLine();
            Console.ReadLine();

            server.Stop();
            adminServer.Stop();
            Console.WriteLine("server stopped");
        }
    }
}