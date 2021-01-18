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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    public static class Program
    {
        private static string[] Arguments { get; set; }

        private static void ConfigureServices(IServiceCollection services)
        {
            var result = Parser
                    .Default
                    .ParseArguments<ChangeLogGeneratorOptions>(Arguments)
                    .WithParsed(x =>
                    {
                        _ = services.AddSingleton(x);
                    });

            if (result.Tag == ParserResultType.NotParsed)
            {
                Environment.Exit(1);
            }

            _ = services.AddHostedService<ChangeLogGeneratorService>();
        }

        static async Task Main(string[] args)
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            await CreateHostBuilder(args).Build().RunAsync(cancellationTokenSource.Token);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Arguments = args;

                    ConfigureServices(services);
                });
    }
}
