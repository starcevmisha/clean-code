using System.Collections.Generic;
using NUnit.Framework;

namespace Markdown
{
    public class Marker
    {
        private static readonly Dictionary<MarkerType, string> HtmlTags = new Dictionary<MarkerType, string>
        {
            {MarkerType.Strong, "<strong>"},
            {MarkerType.Em, "<em>" },
            {MarkerType.Code, "<code>"}
        };

        public readonly int Length;
        public int Pos;
        public readonly MarkerType Type;

        private Marker(int pos, MarkerType type, int length )
        {
            Pos = pos;
            Type = type;
            Length = length;
        }


        public static bool TryCreateTag(string line, int pos, Marker lastTag, out Marker tag)
        {
            var isCorrectStrongCloseTag = !(lastTag != null &&lastTag.Type == MarkerType.Em && IsOpeningTag(line, lastTag.Pos) &&
                    IsClosingTag(line, pos));//Корректное закрывание тегов в случае ___, сначала закрывается одинарный, потом двойной

            if (IsStrongTag(line, pos) && isCorrectStrongCloseTag)
            {
                tag = new Marker(pos, MarkerType.Strong, 2);
                return true;
            }
            if (IsEmTag(line, pos)) { 
                tag =  new Marker(pos, MarkerType.Em, 1);
                return true;
            }
            if (IsCodeTag(line, pos))
            {
                tag =  new Marker(pos, MarkerType.Code, 2);
                return true;
            }
            tag = null;
            return false;
        }


        private static bool IsStrongTag(string line, int pos)
        {
            return line[pos] == '_' && pos < line.Length - 1 && line[pos + 1] == '_'
                || line[pos] == '*' && pos < line.Length - 1 && line[pos + 1] == '*';
        }
        private static bool IsEmTag(string line, int pos)
        {
            return line[pos] == '_'
                || line[pos] == '*';
        }
        private static bool IsCodeTag(string line, int pos)
        {
            return line[pos] == '`' && pos < line.Length - 1 && line[pos + 1] == '`';
        }

        public static bool IsOpeningTag(string line, int pos)
        {
            return pos < line.Length - 1 && line[pos + 1] != ' ' && !IsInsideDigits(line, pos);
        }
        public static bool IsClosingTag(string line, int pos)
        {
            return pos > 0 && line[pos - 1] != ' ' && !IsInsideDigits(line, pos);
        }

        public static bool IsInsideDigits(string line, int pos)
        {
            return (pos+1 >= line.Length || char.IsDigit(line[pos + 1])) 
                && (pos-1 < 0 || char.IsDigit(line[pos - 1]));
        }

        public static (Marker openingTag, Marker closingTag) GetTagsPair(Marker closingTag, Stack<Marker> stackTag)
        {
            var openingTag = stackTag.Pop();
            while (openingTag.Type != closingTag.Type)
                openingTag = stackTag.Pop();
            return (openingTag, closingTag);
        }

        public static string ConvertToHtmlTag(string line, Marker openingTag, Marker closingTag)
        {
            closingTag.Pos += HtmlTags[closingTag.Type].Length - closingTag.Length;
            line = line
                .Remove(openingTag.Pos, openingTag.Length)
                .Insert(openingTag.Pos, HtmlTags[openingTag.Type]);
            line = line
                .Remove(closingTag.Pos, closingTag.Length)
                .Insert(closingTag.Pos, HtmlTags[openingTag.Type].Insert(1, "/"));
            return line;
        }
    }

    [TestFixture]
    public class Marker_Should
    {
        [TestCase("q1_2_3", 2, ExpectedResult = true)]
        [TestCase("_2_3", 0, ExpectedResult = true)]
        [TestCase("23_", 2, ExpectedResult = true)]
        [TestCase("q_w_e", 2, ExpectedResult = false)]
        public bool IsInsideDigits_Tests(string line, int pos)
        {
            return Marker.IsInsideDigits(line, pos);
        }

        [TestCase("_qwerty", 0, ExpectedResult = true)]
        [TestCase("qw _erty", 4, ExpectedResult = true)]
        [TestCase("qwe1_3rty", 4, ExpectedResult = false)]
        [TestCase("qwer_ ty", 4, ExpectedResult = false)]
        public bool IsOpeningTag_Tests(string line, int pos)
        {
            return Marker.IsOpeningTag(line, pos);
        }
        [TestCase("_qwerty", 0, ExpectedResult = false)]
        [TestCase("_qw _erty", 4, ExpectedResult = false)]
        [TestCase("qwe1_3rty", 4, ExpectedResult = false)]
        [TestCase("qwer_ ty", 4, ExpectedResult = true)]
        [TestCase("qwerty_", 6, ExpectedResult = true)]
        public bool IsClosingTag_Tests(string line, int pos)
        {
            return Marker.IsClosingTag(line, pos);
        }
    }

    public enum MarkerType
    {
        Strong,
        Em,
        Code
    }
}