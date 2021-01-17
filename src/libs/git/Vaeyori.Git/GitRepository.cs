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

namespace Vaeyori.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    public sealed class GitRepository
    {
        private readonly IRepository _repository;
        public GitRepository(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<long> GetCommitCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(_repository.Commits.LongCount());
        }


        public Task<IEnumerable<Commit>> GetLatestCommitsFromTagAsync(string startingTagName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(startingTagName))
            {
                throw new ArgumentNullException(nameof(startingTagName));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var startingCommitsWithTags = GetCommitsWithTags(startingTagName);
            var startingCommit = startingCommitsWithTags
                .LastOrDefault();

            var filter = new CommitFilter
            {
                ExcludeReachableFrom = startingCommit,
                SortBy = CommitSortStrategies.Reverse
            };

            return Task.FromResult(
                (IEnumerable<Commit>)_repository.Commits.QueryBy(filter));
        }

        private IEnumerable<Commit> GetCommitsWithTags(string tagName)
        {
            var tag = _repository.Tags.First(x => x.FriendlyName.Equals(tagName));

            var filter = new CommitFilter
            {
                IncludeReachableFrom = tag,
                SortBy = CommitSortStrategies.Reverse
            };
            var commitsWithTags =
                _repository.Commits.QueryBy(filter);

            return commitsWithTags;
        }
    }
}
