using System;
using NUnit.Framework;

namespace Markdown
{
    public static class BlockElement
    {

        public static string Parse(string line)
        {
            var lineType = GetBlockType(line);

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



        public static BlockType GetBlockType(string line)
        {
            if (IsHeader(line)) return BlockType.Header;
            if (IsList(line)) return BlockType.List;
            if (IsCode(line)) return BlockType.Code;
            if (IsParagraph(line)) return BlockType.Paragraph;
            return BlockType.Empty;
        }

        public static bool IsParagraph(string line)
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
            return line.Length > 2 && char.IsDigit(line[0]) && line[1] == '.' && line[2]==' ';
        }

        public static bool IsHeader(string line)
        {
            return line.Length > 0 && line[0] == '#';
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
            return null;
        }
    }

    public enum BlockType
    {
        Header,
        Paragraph,
        Code,
        List,
        BlockQuotes,
        Empty

    }

    [TestFixture]
    public class BlockElement_Should
    {
        [TestCase("1. QWerty", ExpectedResult = true)]
        [TestCase("1.QWerty", ExpectedResult = false)]
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

    }
}