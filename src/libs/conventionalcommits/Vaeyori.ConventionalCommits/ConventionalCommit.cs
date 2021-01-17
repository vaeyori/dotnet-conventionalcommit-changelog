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
    using System.ComponentModel.DataAnnotations;

    public sealed class ConventionalCommit
    {
        public ConventionalCommit(
            string type,
            string subject,
            bool hasBreakingToken)
            : this(type, null, subject, hasBreakingToken)
        {
        }

        public ConventionalCommit(
            string type,
            string scope,
            string subject,
            bool hasBreakingToken)
            : this(type, scope, subject)
        {
            HasBreakingToken = hasBreakingToken;
        }

        public ConventionalCommit(
            string type,
            string scope,
            string subject)
        {
            Type = type;
            Subject = subject;
            Scope = scope;
        }

        [Required]
        public string Type { get; }

        public string Scope { get; }

        [Required]
        public string Subject { get; }

        public bool HasBreakingToken { get; private set; }

        private ICollection<ConventionalCommitTrailer> _trailers =
            new List<ConventionalCommitTrailer>();

        public IEnumerable<ConventionalCommitTrailer> Trailers { get { return _trailers; } }

        public void AppendTrailer(string token, string value)
        {
            var trailer = new ConventionalCommitTrailer(token, value);

            _trailers.Add(trailer);
        }

        public void FoundBreakingChangeToken()
        {
            HasBreakingToken = true;
        }
    }
}
