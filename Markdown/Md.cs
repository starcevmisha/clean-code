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
                    //TODO Закрыть старый блок и открыть новый
                }

                resultLine.Append(BlockElement.GetLine(line, type));

//                if (BlockElement.IsHeader(line))
//                    resultLine.Add(LineElement.Parse(BlockElement.Parse(line)));
//                else
//                    resultLine.Add(LineElement.Parse(line));
                resultText.Add(LineElement.Parse(resultLine.ToString()));
                oldBlockType = type;
            }
            return string.Join("\n", resultText);
        }


        
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        [TestCase("qwertyqwerty", ExpectedResult = "qwertyqwerty")]
        [TestCase("hello world", ExpectedResult = "hello world")]
        public string ParseWithoutTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }

        [TestCase("_qwertyqwerty_", ExpectedResult = "<em>qwertyqwerty</em>")]
        [TestCase("_hello_ _world", ExpectedResult = "<em>hello</em> _world")]
        [TestCase("hello world_1_23", ExpectedResult = "hello world_1_23")]
        [TestCase(@"_hello\_world_", ExpectedResult = "<em>hello_world</em>")]
        public string ParseEmTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }
        [TestCase("__hello world__", ExpectedResult = "<strong>hello world</strong>")]
        [TestCase("hello **world**", ExpectedResult = "hello <strong>world</strong>")]
        [TestCase(@"\__hello world", ExpectedResult = "__hello world")]
        public string ParseStrongTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }
        [TestCase(@"``hello`` world", ExpectedResult = "<code>hello</code> world")]
        [TestCase(@"``_hello_`` world", ExpectedResult = "<code>_hello_</code> world")]
        [TestCase(@"``_hello_`` ``world``", ExpectedResult = "<code>_hello_</code> <code>world</code>")]
        [TestCase(@"``_h__e`l`l__o_`` world", ExpectedResult = "<code>_h__e`l`l__o_</code> world")]
        public string ParseCodeTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }

        [TestCase("_a __a__ a_", ExpectedResult = "<em>a __a__ a</em>")]//Внутри одинарного двойное не работает
        [TestCase("__a _a_ a__", ExpectedResult = "<strong>a <em>a</em> a</strong>")]//Внутри двойного одинарное работает
        [TestCase("_a __a__ __a__ a_", ExpectedResult = "<em>a __a__ __a__ a</em>")]
        [TestCase("_a __a__ a", ExpectedResult = "_a <strong>a</strong> a")]
        [TestCase("_a __a__ __a__ a", ExpectedResult = "_a <strong>a</strong> <strong>a</strong> a")]
        public string ParseMixedStrongAndEmTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }

        [TestCase("##Header", ExpectedResult = "<h2>Header</h2>")]
        [TestCase("###Header#########", ExpectedResult = "<h1>Header</h1>")]
        [TestCase("##_Header_", ExpectedResult = "<h2><em>Header</em></h2>")]
        [TestCase("####Header", ExpectedResult = "####Header")]
        public string ParseHeader(string markdown)
        {
            return new Md().RenderToHtml(markdown);
        }




    }
}