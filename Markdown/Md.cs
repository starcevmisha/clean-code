using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {


        public string RenderToHtml(string markdown)
        {
            var resultLine = new List<string>();
            foreach (var line in markdown.Split('\n'))
            {
               resultLine.Add(ParseLine(line));
            }
            return String.Join("\n", resultLine);
        }

        private static string ParseLine(string markdown)
        {
            var isInsideEmTags = false;
            var strongTagsPairs = new Stack<(Marker, Marker)>();
            var line = markdown;

            var stackTag = new Stack<Marker>();
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    line = line.Remove(i, 1);
                    i++;
                }
                var tag = Marker.CreateTag(line, i);
                if (tag == null)
                    continue;
                i += tag.Length - 1;

                if (Marker.IsClosingTag(line, tag.Pos) && stackTag.Any(openTag => openTag.Type == tag.Type))
                {
                    var tags = Marker.GetTagsPair(line, tag, stackTag);
                    if (tags.openingTag.Type == MarkerType.Em)
                    {
                        isInsideEmTags = false;
                        strongTagsPairs.Clear();
                    }
                    else if (tags.openingTag.Type == MarkerType.Strong && isInsideEmTags)//Если двойное внутри одинарного то запоминаем и идем дальше
                    {
                        strongTagsPairs.Push(tags);
                        continue;
                    }
                    line = Marker.ConvertToHtmlTag(line, tags.openingTag, tags.closingTag);
                }

                else if (Marker.IsOpeningTag(line, tag.Pos))
                {
                    if (tag.Type == MarkerType.Em)
                    {
                        isInsideEmTags = true;
                        line = ConvertStrongTagFromStack(strongTagsPairs, line);
                    }
                    stackTag.Push(tag);
                }
            }
            line = ConvertStrongTagFromStack(strongTagsPairs, line);
            return line;
        }
        private static string ConvertStrongTagFromStack(Stack<(Marker, Marker)> strongTagsPairs, string line)
        {
            while (strongTagsPairs.Count > 0)
            {
                var tagsPair = strongTagsPairs.Pop();
                line = Marker.ConvertToHtmlTag(line, tagsPair.Item1, tagsPair.Item2);
            }
            return line;
        }
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        [TestCase("qwertyqwerty", ExpectedResult = "qwertyqwerty")]
        [TestCase("hello world", ExpectedResult = "hello world")]
        public string ParseWithoutTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("_qwertyqwerty_", ExpectedResult = "<em>qwertyqwerty</em>")]
        [TestCase("_hello_ _world", ExpectedResult = "<em>hello</em> _world")]
        [TestCase("hello world_1_23", ExpectedResult = "hello world_1_23")]
        [TestCase(@"_hello\_world_", ExpectedResult = "<em>hello_world</em>")]
        public string ParseEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }
        [TestCase("__hello world__", ExpectedResult = "<strong>hello world</strong>")]
        [TestCase("hello **world**", ExpectedResult = "hello <strong>world</strong>")]
        [TestCase(@"\__hello world", ExpectedResult = "__hello world")]
        public string ParseStrongTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }
        [TestCase(@"``hello`` world", ExpectedResult = "<code>hello</code> world")]
        public string ParseCodeTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("_a __a__ a_", ExpectedResult = "<em>a __a__ a</em>")]//Внутри одинарного двойное не работает
        [TestCase("__a _a_ a__", ExpectedResult = "<strong>a <em>a</em> a</strong>")]//Внутри двойного одинарное работает
        [TestCase("_a __a__ __a__ a_", ExpectedResult = "<em>a __a__ __a__ a</em>")]
        [TestCase("_a __a__ a", ExpectedResult = "_a <strong>a</strong> a")]
        [TestCase("_a __a__ __a__ a", ExpectedResult = "_a <strong>a</strong> <strong>a</strong> a")]
        public string ParseMixedStrongAndEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }



    }
}