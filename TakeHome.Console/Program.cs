using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;

namespace GuaranteedRate.TakeHome
{
    class Program
    {
        private static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"Migration Report v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            Console.WriteLine("-------------------------------------");


        }
    }
}
