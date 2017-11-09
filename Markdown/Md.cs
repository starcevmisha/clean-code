using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        public string RenderToHtml(string markdown)
        {
            BlockType oldBlockType = BlockType.Empty;

            var resultText = new List<string>();
            foreach (var line in markdown.Split('\n'))
            {
                var resultLine = new StringBuilder();
                var type = BlockElement.GetBlockType(line);
                if (oldBlockType != type)
                {
                    resultLine.Append(BlockElement.CloseLastBlock(oldBlockType));
                    resultLine.Append(BlockElement.OpenNewBlock(type));
                }

                resultLine.Append(BlockElement.GetLine(line, type));
                resultText.Add(LineElement.Parse(resultLine.ToString()));
                oldBlockType = type;
            }
            if(BlockElement.CloseLastBlock(oldBlockType) != "")
                resultText.Add(BlockElement.CloseLastBlock(oldBlockType));
            return string.Join("\n", resultText);
        }


        
    }

    [TestFixture]
    public class Md_ShouldRender
    {

        [TestCase("##Header", ExpectedResult = "<h2>Header</h2>")]
        [TestCase("###Header#########", ExpectedResult = "<h1>Header</h1>")]
        [TestCase("##_Header_", ExpectedResult = "<h2><em>Header</em></h2>")]
        [TestCase("####Header", ExpectedResult = "####Header")]
        public string ParseHeader(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }

        [TestCase("    Code", ExpectedResult = "<pre><code>Code\n</code></pre>")]
        [TestCase("\tCode", ExpectedResult = "<pre><code>Code\n</code></pre>")]
        public string ParseCodeBlock(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }
        [TestCase("Hello\nWorld", ExpectedResult = "<p>Hello\nWorld\n</p>")]
        [TestCase("Hello\n\nWorld", ExpectedResult = "<p>Hello\n</p>\n<p>World\n</p>")]
        public string ParseParagraphBlock(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }
        [TestCase("1. QWERT", ExpectedResult = "<ul>\n<li>QWERT</li>\n</ul>")]
        [TestCase("+ QWERT\n+ YUIOP", ExpectedResult = "<ul>\n<li>QWERT</li>\n<li>YUIOP</li>\n</ul>")]
        [TestCase("1. QWERT\n23. YUIOP", ExpectedResult = "<ul>\n<li>QWERT</li>\n<li>YUIOP</li>\n</ul>")]
        public string ParseListBlock(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }
        [TestCase("> Hello", ExpectedResult = "<blockquote>\n<p>Hello\n</p>\n</blockquote>")]
        [TestCase("> Hello\n> World", ExpectedResult = "<blockquote>\n<p>Hello\nWorld\n</p>\n</blockquote>")]
        public string ParseBlockQuotes(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }

        [TestCase("Hello\n---------\nWorld", ExpectedResult = "<p>Hello\n</p><hr />\n<p>World\n</p>")]
        public string ParseHorizontalRule(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }




    }
}