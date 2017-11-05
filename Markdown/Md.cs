using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {


        public string RenderToHtml(string markdown)
        {
            var line = markdown;

            var stackTag = new Stack<Marker>();
            for (var i = 0; i <line.Length; i++)
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
                    line = Marker.ConvertToHtmlTag(line, tag, stackTag);
                    continue;
                }
                if (Marker.IsOpeningTag(line, tag.Pos))
                    stackTag.Push(tag);
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

        [TestCase("_a __a__ a_", ExpectedResult = "<em>a __a__ a</em>")]//Внутри одинарного двойное не работает
        [TestCase("__a _a_ a__", ExpectedResult = "<strong>a <em>a</em> a</strong>")]//Внутри двойного одинарное работает
        public string ParseMixedStrongAndEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }



    }
}