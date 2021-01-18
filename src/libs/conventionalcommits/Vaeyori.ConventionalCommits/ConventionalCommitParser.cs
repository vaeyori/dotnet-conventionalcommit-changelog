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

namespace Vaeyori.ConventionalCommits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using LibGit2Sharp;

    public sealed class ConventionalCommitParser
    {
        private const string CARRIAGE_RETURN = "\r";
        private const string LINE_FEED = "\n";
        private const string BREAKING_CHANGE_TOKEN = "BREAKING CHANGE";
        private const string CONVENTIONAL_COMMIT_FORMAT = "^(?<type>(BREAKING CHANGE|\\w*))(?:(?<scope>.*))?: (?<subject>.*)$";
        private static readonly Regex HEADER_PATTERN =
            new Regex($"{CONVENTIONAL_COMMIT_FORMAT}",
                      RegexOptions.Multiline);

        public IEnumerable<ConventionalCommit> Parse(IEnumerable<Commit> commits)
        {
            return commits.Select(Parse);
        }

        private ConventionalCommit Parse(Commit commit)
        {
            var commitMessageLines = commit.Message.Split(
                new[] { CARRIAGE_RETURN, LINE_FEED },
                System.StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line));

            var header = commitMessageLines.ElementAt(0);
            ConventionalCommit conventionalCommit = default;
            if (header is object)
            {
                var matchHeader = HEADER_PATTERN.Match(header);
                if (matchHeader.Success)
                {
                    string type = matchHeader.Groups[nameof(type)].Value;
                    string scope = matchHeader.Groups[nameof(scope)].Value;
                    string subject = matchHeader.Groups[nameof(subject)].Value;

                    conventionalCommit = new ConventionalCommit(type, scope, subject);

                    for (var i = 1; i < commitMessageLines.Count(); i++)
                    {
                        string line = commitMessageLines.ElementAt(i);
                        var matchTrailer = HEADER_PATTERN.Match(line);

                        if (matchTrailer.Success)
                        {
                            var trailerType = matchTrailer.Groups[nameof(type)].Value;
                            var trailerScope = matchTrailer.Groups[nameof(scope)].Value;
                            var trailerSubject = matchTrailer.Groups[nameof(subject)].Value;

                            conventionalCommit.AppendTrailer(trailerType, trailerScope, trailerSubject);

                            if (trailerType.Equals(BREAKING_CHANGE_TOKEN))
                            {
                                conventionalCommit.FoundBreakingChangeToken();
                            }
                        }
                    }
                }
            }

            return conventionalCommit;
        }
    }
}
