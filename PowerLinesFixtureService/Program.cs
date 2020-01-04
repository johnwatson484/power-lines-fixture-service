using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerLinesFixtureService.Messaging;

namespace PowerLinesFixtureService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connection = new AmqpConnection();
            connection.CreateConnectionToQueue("amqp://artemis:artemis@power-lines-message:5672", "fixtures");
            connection.Listen();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
