using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Markdown
{
    public class LineElement
    {
        public static string Parse(string markdown)
        {
            var line = markdown;

            var stackTag = new Stack<Marker>();
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    line = line.Remove(i, 1);
                    continue;
                }
                if (!Marker.TryCreateTag(line, i,
                    stackTag.Count > 0 ? stackTag.Peek() : null,
                    out var tag))
                    continue;

                i += tag.Length - 1;

                if (Marker.IsClosingTag(line, tag.Pos) && stackTag.Any(openTag => openTag.Type == tag.Type) && !IsRepeatTag(tag, stackTag.Peek())) 
                {
                    var tags = Marker.GetTagsPair(line, tag, stackTag);
                    line = Marker.ConvertToHtmlTag(line, tags.openingTag, tags.closingTag);
                }

                else if (Marker.IsOpeningTag(line, tag.Pos))
                {
                    stackTag.Push(tag);
                    if (tag.Type == MarkerType.Code)
                        i = SkipCode(line, i);
                }
            }
            return line;
        }

        private static int SkipCode(string line, int i)
        {
            var newIndex = line.IndexOf("``", i, StringComparison.Ordinal);
            if (newIndex > 0)
                i = newIndex - 1;
            return i;
        }

        private static bool IsRepeatTag(Marker tag, Marker peek)//Чтобы два подряд двойных подчеркивания расчитывались как два открывающих, а не как открывающий и закрывающий
        {
            return tag.Type == peek.Type && tag.Pos == peek.Pos + peek.Length;
        }
    }

    [TestFixture]
    public class LineElement_Should
    {
        [TestCase("qwertyqwerty", ExpectedResult = "qwertyqwerty")]
        [TestCase("hello world", ExpectedResult = "hello world")]
        [TestCase(@"\\", ExpectedResult = @"\")]
        [TestCase(@"\a", ExpectedResult = @"a")]
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

        //[TestCase("_a __a__ a_", ExpectedResult = "<em>a __a__ a</em>")]//Внутри одинарного двойное не работает
        [TestCase("__a _a_ a__", ExpectedResult = "<strong>a <em>a</em> a</strong>")]//Внутри двойного одинарное работает
        //[TestCase("_a __a__ __a__ a_", ExpectedResult = "<em>a __a__ __a__ a</em>")]//Внутри одинарного двойное не работает
        [TestCase("_a __a__ a", ExpectedResult = "_a <strong>a</strong> a")]
        [TestCase("_a __a__ __a__ a", ExpectedResult = "_a <strong>a</strong> <strong>a</strong> a")]
        [TestCase("___hello___", ExpectedResult = "<strong><em>hello</em></strong>")]
        [TestCase("____hello____", ExpectedResult = "<strong><strong>hello</strong></strong>")]
        [TestCase("______hello______", ExpectedResult = "<strong><strong><strong>hello</strong></strong></strong>")]
        [TestCase("_a __a___", ExpectedResult = "<em>a <strong>a</strong></em>")]
        public string ParseMixedStrongAndEmTokens(string markDown)
        {
            return LineElement.Parse(markDown);
        }
    }
}