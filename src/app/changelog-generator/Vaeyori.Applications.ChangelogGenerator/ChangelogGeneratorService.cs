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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Vaeyori.ConventionalCommits;
    using Vaeyori.Git;
    using Humanizer;
    using LibGit2Sharp;
    using Microsoft.Extensions.Hosting;

    public sealed class ChangeLogGeneratorService : IHostedService
    {
        private readonly ChangeLogGeneratorOptions _options;
        private readonly IHostApplicationLifetime _applicationLifetime;
        public ChangeLogGeneratorService(
            ChangeLogGeneratorOptions options,
            IHostApplicationLifetime applicationLifetime)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var repositoryPath = Directory.GetCurrentDirectory();
            var repository = new Repository(repositoryPath);
            var gitRepository = new GitRepository(repository);

            IEnumerable<Commit> commits = new List<Commit>();
            if (string.IsNullOrEmpty(_options.EndTag))
            {
                commits = await gitRepository.GetLatestCommitsFromTagAsync(
                    _options.StartTag,
                    cancellationToken);
            }

            var parser = new ConventionalCommitParser();
            var conventionalCommits = parser.Parse(commits);

            IDictionary<string, IDictionary<string, ICollection<string>>> changelogMessages = new Dictionary<string, IDictionary<string, ICollection<string>>>();
            foreach (var conventionalCommit in conventionalCommits)
            {
                if (!changelogMessages.TryGetValue(conventionalCommit.Type, out var typeCollection))
                {
                    typeCollection = new Dictionary<string, ICollection<string>>();
                    changelogMessages.Add(conventionalCommit.Type, typeCollection);
                }

                var scope = conventionalCommit.Scope;
                if (string.IsNullOrEmpty(scope))
                {
                    scope = "_General";
                }

                if (!typeCollection.TryGetValue(scope, out var scopeCollection))
                {
                    scopeCollection = new List<string>();
                    typeCollection.Add(scope, scopeCollection);
                }

                scopeCollection.Add(conventionalCommit.Subject);
            }

            StringBuilder builder = new StringBuilder();
            _ = builder.AppendLine($"# {_options.Version} - {DateTimeOffset.UtcNow:yyyy/MM/dd}");
            foreach (var keyPair in changelogMessages.OrderBy(x => x.Key))
            {
                _ = builder.AppendLine($"## **{keyPair.Key.Humanize().Transform(To.SentenceCase).Pluralize()}**");
                foreach (var pair in keyPair.Value.OrderBy(x => x.Key))
                {
                    _ = builder.AppendLine($"* **{pair.Key.Replace("_", "").Humanize().Transform(To.SentenceCase)}**");
                    foreach (var message in pair.Value)
                    {
                        _ = builder.AppendLine($"  * {message.Humanize().Transform(To.SentenceCase)}");
                    }
                    _ = builder.AppendLine();
                }

                _ = builder.AppendLine();
            }

            using var readFileStream = File.Open(_options.OutputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var streamReader = new StreamReader(readFileStream);

            _ = builder.AppendLine(streamReader.ReadToEnd());

            streamReader.Close();

            using var writeFileStream = File.Open(_options.OutputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var streamWriter = new StreamWriter(writeFileStream);

            _ = writeFileStream.Seek(0, SeekOrigin.Begin);

            await streamWriter.WriteAsync(builder, cancellationToken);

            streamWriter.Close();

            _applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
