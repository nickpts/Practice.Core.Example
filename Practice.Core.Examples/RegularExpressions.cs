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
        public static void MatchAnySingleCharacter()
        {
            var result = Regex.Match("little praire in the woods", ".l.");
            //tle
            Print(result);

            result = Regex.Match("little praire in the woods", "l.....");
            //little
            Print(result);

            result = Regex.Match("little praire in the woods", "....s");
            //woods
            Print(result);

            result = Regex.Match("little praire in the woods", "w...s");
            //woods
            Print(result);
        }

        /// <summary>
        /// A character may be matched at most once. Result will be successful if character not matched.
        /// </summary>
        public static void MatchCharacterOptional()
        { 
            var result = Regex.Match("little praire in the woods", "c?");
            //success: true
            //value: 
            Print(result);

            result = Regex.Match("little praire in the woods", "l?");
            //success: true (result is matched at least once)
            //value: 
            Print(result);
        }

        public static void MatchCharacterZeroOrMoreTimes()
        {
            var result = Regex.Match("little praire in the woods", "l*");
            //success: true (result is matched at least once)
            //value: 
            Print(result);

            result = Regex.Match("little praire in the woods", "y*");
            //success: true (result is matched zero times)
            //value: 
            Print(result);
        }

        public static void MatchOneOrMoreTimes()
        {
            var result = Regex.Match("little praire in the woods", "y+");
            //success: false (cannot find result)
            //value: 
            Print(result);

            result = Regex.Match("little praire in the woods", "e+");
            //success: true
            //value: 
            Print(result);
        }

        public static void MatchExactlyNTimes()
        {
            var result = Regex.Match("little praire in the woods", @"little{1}");
            Print(result);
        }

        public static void MatchExactlyOneTime()
        {
            var result = Regex.Match("little praire in the woods", "");
            Print(result);
        }

        public static void Print(Match result)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(result))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(result);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
    }
}
