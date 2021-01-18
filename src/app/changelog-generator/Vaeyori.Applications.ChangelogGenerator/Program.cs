/*
*    Copyright (C) 2021 Joshua Thompson @ysovuka
*
*    This program is free software: you can redistribute it and/or modify
*    it under the terms of the GNU Affero General Public License as published
*    by the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU Affero General Public License for more details.
*
*    You should have received a copy of the GNU Affero General Public License
*    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace Vaeyori.Applications.ChangelogGenerator
{
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    public static class Program
    {
        private static string[] Arguments { get; set; }

        private static void ConfigureServices(IServiceCollection services)
        {

            _ = Parser
                    .Default
                    .ParseArguments<ChangeLogGeneratorOptions>(Arguments)
                    .WithParsed(x =>
                    {
                        _ = services.AddSingleton(x);
                    });


            _ = services.AddHostedService<ChangeLogGeneratorService>();
        }

        static Task Main(string[] args) =>
            CreateHostBuilder(args).Build().RunAsync();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Arguments = args;

                    ConfigureServices(services);
                });
    }
}
