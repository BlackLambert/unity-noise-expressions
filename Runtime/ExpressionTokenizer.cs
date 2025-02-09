using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SBaier.Expressions
{
    public class ExpressionTokenizer
    {
        private static readonly Regex TokenPattern = new Regex(@"\s*(\d+(\.\d+)?|\+|\-|\*|\/|\(|\)|\|\||\||,|&&|==|!=|<=?|>=?|!|\?|:|\b[Ff]loor\b|\b[Cc]lamp\b|\b[Oo]ctave\b|\b[Ss]implex\b|\b[Mm]ax\b|\b[Mm]in\b|\b[Ee]se[Ii]n[Oo]ut\b|[a-zA-Z][a-zA-Z0-9]*)\s*");

        public List<string> Tokenize(string input)
        {
            string inputWithoutWhitespace = Regex.Replace(input, @"\s+", "");
            string inputSnakeCase = inputWithoutWhitespace.ToLower();
            MatchCollection matches = TokenPattern.Matches(inputSnakeCase);
            return matches.Select(match => match.Groups[1].Value).ToList();
        }
    }
}
