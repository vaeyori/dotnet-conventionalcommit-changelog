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

namespace Vaeyori.Git.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using Xunit;

    public class GitRepositoryUnitTests
    {
        [Fact]
        public void GitRepository_Constructor_ThrowsArgumentNullException()
        {
            _ = Assert.ThrowsAny<ArgumentNullException>(() => new GitRepository(null));
        }

        [Fact]
        public async Task GitRepository_GetLatestCommitsFromTagAsync_ThrowsArgumentNullException()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var repositoryPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../../");
            var repository = new Repository(repositoryPath);
            var gitRepository = new GitRepository(repository);
            var tag = "";

            var totalNumberOfCommits = await gitRepository.GetCommitCountAsync(cancellationTokenSource.Token);
            _ = await Assert.ThrowsAsync<ArgumentNullException>(async () => await gitRepository.GetLatestCommitsFromTagAsync(
                tag,
                cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GitRepository_GetLatestCommitsFromTagAsync_ThrowsInvalidOperationException()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var repositoryPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../../");
            var repository = new Repository(repositoryPath);
            var gitRepository = new GitRepository(repository);
            var tag = "Initial_NotFound";

            var totalNumberOfCommits = await gitRepository.GetCommitCountAsync(cancellationTokenSource.Token);
            _ = await Assert.ThrowsAsync<InvalidOperationException>(async() => await gitRepository.GetLatestCommitsFromTagAsync(
                tag,
                cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GitRepository_GetLatestCommitsFromTagAsync_ReturnsCommitsSuccessfully()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var repositoryPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../../");
            var repository = new Repository(repositoryPath);
            var gitRepository = new GitRepository(repository);
            var tag = "Initial";

            var totalNumberOfCommits = await gitRepository.GetCommitCountAsync(cancellationTokenSource.Token);
            var latestCommitsFromTag = await gitRepository.GetLatestCommitsFromTagAsync(
                tag,
                cancellationTokenSource.Token);

            Assert.NotEmpty(latestCommitsFromTag);
            Assert.NotEqual(0, totalNumberOfCommits);
            Assert.Equal(totalNumberOfCommits - 1, latestCommitsFromTag.Count());
        }
    }
}
