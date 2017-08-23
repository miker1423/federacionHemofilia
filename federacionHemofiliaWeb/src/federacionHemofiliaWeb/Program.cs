using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace federacionHemofiliaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuilderWebHost(args).Run();
        }

        public static IWebHost BuilderWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
