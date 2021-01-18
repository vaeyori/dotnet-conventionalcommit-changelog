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
    using CommandLine;

    public sealed class ChangeLogGeneratorOptions
    {
        [Option(shortName: 's', longName: "start-tag", Required = true)]
        public string StartTag { get; set; }

        [Option(shortName: 'o', longName: "output", Required = true)]
        public string OutputPath { get; set; }

        [Option(shortName: 'e', longName: "end-tag", Required = false)]
        public string EndTag { get; set; }

        [Option(shortName: 'v', longName: "version", Required = true)]
        public string Version { get; set; }
    }
}
