using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Markdown
{
    public static class BlockElement
    {
        private static readonly Regex ListRegex = new Regex(@"^(?:\d+\.|\+) (.*)");
        private static readonly Regex BlockQuotesRegex = new Regex(@"^>{1,4} (.+)");
        private static readonly Regex HorizontalRuleRegex = new Regex(@"^(?:\*{3,}|-{3,})$");

        public static string OpenNewBlock(BlockType newBlockType)
        {
            switch (newBlockType)
            {
                case BlockType.Code:
                    return "<pre><code>";
                case BlockType.BlockQuotes:
                    return "<blockquote>\n<p>";
                case BlockType.List:
                    return "<ul>\n";
                case BlockType.Paragraph:
                    return "<p>";
            }
            return "";
        }

        public static string CloseLastBlock(BlockType lastBlockType)
        {
            switch (lastBlockType)
            {
                case BlockType.Code:
                    return "</code></pre>";
                case BlockType.List:
                    return "</ul>";
                case BlockType.BlockQuotes:
                    return "</p>\n</blockquote>";
                case BlockType.Paragraph:
                    return "</p>";
            }
            return "";
        }


        public static BlockType GetBlockType(string line)
        {
            if (IsHeader(line)) return BlockType.Header;
            if (IsList(line)) return BlockType.List;
            if (IsCode(line)) return BlockType.Code;
            if (IsBlockQuotes(line)) return BlockType.BlockQuotes;
            if (IsHorizontalRules(line)) return BlockType.HorizontalRule;
            if (IsParagraph(line)) return BlockType.Paragraph;
            return BlockType.Empty;
        }

        private static bool IsParagraph(string line)
        {
            return !string.IsNullOrEmpty(line);
        }

        public static bool IsCode(string line)
        {
            return line.Length > 0 && line[0] == '\t'
                   || line.Length > 3 && line.StartsWith("    ");
        }

        public static bool IsList(string line)
        {
            return ListRegex.IsMatch(line);
        }

        public static bool IsHeader(string line)
        {
            return line.Length > 0 && line[0] == '#';
        }

        public static bool IsHorizontalRules(string line)
        {
            return HorizontalRuleRegex.IsMatch(line);
        }

        public static bool IsBlockQuotes(string line)
        {
            return BlockQuotesRegex.IsMatch(line);
        }

        public static string GetLine(string line, BlockType type)
        {
            if (type == BlockType.Header)
            {
                var headerLevel = 0;
                while (line[headerLevel] == '#') headerLevel++;
                if (headerLevel > 3)
                    return line;
                var i = line.Length - 1;
                while (line[i] == '#') i--;
                return string.Format("<h{0}>{1}</h{0}>",
                    4 - headerLevel,
                    line.Substring(headerLevel, i - headerLevel + 1));
            }
            if (type == BlockType.Code)
            {
                if (line[0] == '\t')
                    return line.Substring(1);
                return line.Substring(4);
            }
            if (type == BlockType.List)
                return $"<li>{ListRegex.Match(line).Groups[1]}</li>";

            if (type == BlockType.BlockQuotes)
                return BlockQuotesRegex.Match(line).Groups[1].ToString();
            if (type == BlockType.HorizontalRule)
                return "<hr />";
            return line;
            
        }
    }

    public enum BlockType
    {
        Header,
        Paragraph,
        Code,
        List,
        BlockQuotes,
        Empty,
        HorizontalRule
    }

    [TestFixture]
    public class BlockElement_Should
    {
        [TestCase("1. QWerty", ExpectedResult = true)]
        [TestCase("123. QWerty", ExpectedResult = true)]
        [TestCase("+ QW", ExpectedResult = true)]
        [TestCase("1.QWerty", ExpectedResult = false)]
        [TestCase("QWe 1. QWerty", ExpectedResult = false)]
        public bool IsList_Test(string line)
        {
            return BlockElement.IsList(line);
        }

        [TestCase("    QWerty", ExpectedResult = true)]
        [TestCase("\tQWerty", ExpectedResult = true)]
        [TestCase("  QWerty", ExpectedResult = false)]
        public bool IsCode_Test(string line)
        {
            return BlockElement.IsCode(line);
        }

        [TestCase("##QWerty", ExpectedResult = true)]
        [TestCase("QWerty", ExpectedResult = false)]
        public bool IsHeader_Test(string line)
        {
            return BlockElement.IsHeader(line);
        }

        [TestCase("> QWerty", ExpectedResult = true)]
        [TestCase(">> QWerty", ExpectedResult = true)]
        [TestCase(">>>>> QWerty", ExpectedResult = false)]
        [TestCase(" >> QWerty", ExpectedResult = false)]
        [TestCase(">>QWerty", ExpectedResult = false)]
        public bool IsBlockQuotes_Test(string line)
        {
            return BlockElement.IsBlockQuotes(line);
        }
        [TestCase("--------", ExpectedResult = true)]
        [TestCase("**********", ExpectedResult = true)]
        [TestCase("---   ew", ExpectedResult = false)]
        [TestCase(" ** fdg", ExpectedResult = false)]
        public bool IsHorizontalRule_Test(string line)
        {
            return BlockElement.IsHorizontalRules(line);
        }
    }
}