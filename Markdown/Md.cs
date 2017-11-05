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
        [TestCase("_hello world_", ExpectedResult = "<em>hello world</em>")]
        [TestCase("_hello_ world", ExpectedResult = "<em>hello</em> world")]
        public string ParseEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }


    }
}