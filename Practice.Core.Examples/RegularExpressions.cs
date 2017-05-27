using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace Practice.Core.Examples
{
    public class RegularExpressions
    {
        /// <summary>
        /// operator "." matches any preceding or subsequent character.
        /// </summary>
        public static bool MatchNumbers()
        {
            return Regex.Match("abc123xyz", "123").Success
                && Regex.Match("define " + 123 + "", "123").Success
                && Regex.Match("var g = 123;", "123").Success;
        }

        public static bool MatchTextWithStartAndEndConditions()
        {
            return Regex.Match("file_record_transcript.pdf", "^(file.+).pdf$").Success
                && Regex.Match("file_07241999.pdf", "^(file.+).pdf$").Success;

        }

    }
}
