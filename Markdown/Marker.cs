using System.Collections.Generic;

namespace Markdown
{
    public class Marker
    {
        static Dictionary<MarkerType, string> HtmlTags = new Dictionary<MarkerType, string>
        {
            {MarkerType.Strong, "<strong>"},
            {MarkerType.Em, "<em>" }
        };
        public int Length;
        public int Pos;
        public MarkerType Type;

        public Marker(int pos, MarkerType type )
        {
            Pos = pos;
            Type = type;
            Length = type == MarkerType.Strong ? 2 : 1;
        }


        public static Marker CreateTag(string line, int pos)
        {
            if (IsStrongTag(line, pos))
                return new Marker(pos, MarkerType.Strong);
            if (IsEmTag(line, pos))
                return new Marker(pos, MarkerType.Em);
            return null;
        }

        private static bool IsStrongTag(string line, int pos)
        {
            return (line[pos] == '_' && pos < line.Length - 1 && line[pos + 1] == '_') ||
                   (line[pos] == '*' && pos < line.Length - 1 && line[pos + 1] == '*');
        }
        private static bool IsEmTag(string line, int pos)
        {
            return line[pos] == '_';
        }

        public static bool IsOpeningTag(string line, int pos)
        {
            return pos < line.Length - 1 && line[pos + 1] != ' ';
        }
        public static bool IsClosingTag(string line, int pos)
        {
            return pos > 0 && line[pos - 1] != ' ';
        }


        public static string ConvertToHtmlTag(string line, Marker closingTag, Stack<Marker> stackTag)
        {
            var openingTag = stackTag.Pop();
            while (openingTag.Type != closingTag.Type)
                openingTag = stackTag.Pop();

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

    public enum MarkerType
    {
        Strong,
        Em
    }
}