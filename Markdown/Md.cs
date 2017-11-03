using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        public Dictionary<char, Func<string, int, HtmlTag>> tokenDict = new Dictionary<char, Func<string, int, HtmlTag>>
        {
            ['_'] = ParseEmToken
        };
        public List<HtmlTag> ResultHtml = new List<HtmlTag>();


        private static HtmlTag ParseEmToken(string markdown, int startIndex)
        {
            var i = 1;
            while (markdown[i + 1] != '_' && startIndex + i < markdown.Length)
                i++;
            return new HtmlTag("em", markdown.Substring(startIndex + 1, i));
        }

        private HtmlTag ParseWithoutToken(string markdown, int startIndex)
        {
            var i = 1;
            while (startIndex + i < markdown.Length)
            {
                if (tokenDict.ContainsKey(markdown[i]))
                    return new HtmlTag("", markdown.Substring(startIndex, i));
                i++;
            }
            return new HtmlTag("", markdown.Substring(startIndex, i));
        }

        public string RenderToHtml(string markdown)
        {
            for (var i = 0; i < markdown.Length; i++)
            {
                var parseFunc = tokenDict.ContainsKey(markdown[i]) ? tokenDict[markdown[i]] : ParseWithoutToken;
                var parsed = parseFunc(markdown, i);
                i += parsed.Length+1;
                ResultHtml.Add(parsed);
            }
            return string.Join("", ResultHtml);

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