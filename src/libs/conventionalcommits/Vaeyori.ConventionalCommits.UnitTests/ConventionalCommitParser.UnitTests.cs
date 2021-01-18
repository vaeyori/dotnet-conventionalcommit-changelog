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

namespace Vaeyori.ConventionalCommits.UnitTests
{
    using System;
    using System.Linq;
    using LibGit2Sharp;
    using Moq;
    using Xunit;
    public class ConventionalCommitParserUnitTests
    {
        [Theory]
        [InlineData("feat: Added ability to successfully parse single line commit", false)]
        [InlineData("feat(scope): Added ability to successfully parse single line commit", true)]
        public void ConventionalCommitParser_Parse_SuccessfullyParsesSingleLineCommit(string message, bool hasScope)
        {
            // given commit message does not meet conventional commit standards
            var commit = new Mock<Commit>();
            commit.Setup(x => x.Message).Returns(message);

            // when we parse commit message
            var parser = new ConventionalCommitParser();
            var conventionalCommits = parser.Parse(new[] { commit.Object });
            ConventionalCommit conventionalCommit = conventionalCommits.FirstOrDefault();

            // then we return a null value
            Assert.NotNull(conventionalCommit);
            Assert.NotNull(conventionalCommit.Type);
            Assert.NotNull(conventionalCommit.Subject);

            if (hasScope)
            {
                Assert.NotNull(conventionalCommit.Scope);
            }
        }

        [Theory]
        [InlineData("feat: Added ability to\r\n\r\nfeat: successfully parse single line commit", false, false)]
        [InlineData("feat(scope): Added ability to\r\n\r\nfeat: successfully parse single line commit", true, false)]
        [InlineData("feat: Added ability to\r\n\r\nBREAKING CHANGE: successfully parse single line commit", false, true)]
        [InlineData("feat(scope): Added ability to\r\n\r\nBREAKING CHANGE: successfully parse single line commit", true, true)]
        public void ConventionalCommitParser_Parse_SuccessfullyParsesMultipleLineCommit(string message, bool hasScope, bool hasBreakingToken)
        {
            // given commit message does not meet conventional commit standards
            var commit = new Mock<Commit>();
            commit.Setup(x => x.Message).Returns(message);

            // when we parse commit message
            var parser = new ConventionalCommitParser();
            var conventionalCommits = parser.Parse(new[] { commit.Object });
            ConventionalCommit conventionalCommit = conventionalCommits.FirstOrDefault();

            var trailer = conventionalCommit.Trailers.FirstOrDefault();

            // then we return a null value
            Assert.NotNull(conventionalCommit);
            Assert.NotNull(conventionalCommit.Type);
            Assert.NotNull(conventionalCommit.Subject);

            if (hasScope)
            {
                Assert.NotNull(conventionalCommit.Scope);
            }

            if (hasBreakingToken)
            {
                Assert.True(conventionalCommit.HasBreakingToken);
            }

            Assert.NotEmpty(conventionalCommit.Trailers);
            Assert.NotNull(trailer);
            Assert.NotNull(trailer.Type);
            Assert.NotNull(trailer.Subject);
        }

        [Fact]
        public void ConventionalCommitParser_Parse_FailsToParseSingleLineCommit()
        {
            // given commit message does not meet conventional commit standards
            var commit = new Mock<Commit>();
            commit.Setup(x => x.Message).Returns("This is a failure test.");

            // when we parse commit message
            var parser = new ConventionalCommitParser();
            var conventionalCommits = parser.Parse(new[] { commit.Object });
            ConventionalCommit conventionalCommit = conventionalCommits.FirstOrDefault();

            // then we return a null value
            Assert.Null(conventionalCommit);
        }
    }
}
