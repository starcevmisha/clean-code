using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        private Dictionary<string, string> Rules = new Dictionary<string, string>
        {
            {@"(?<!\\) ([*_]{2})([*_]) ([^\n]+?) \2\1 ", @"<strong><em>$3</em></strong>" },//parse bold-italic
            {@"(?<!\\) ([*_]{2}) (.*?) (?<!\\)\1", @"<strong>$2</strong>"},//parse bold
            {@"(?<!\\) ([*_]) (.+?) (?<!\\) \1", @"<em>$2</em>"}, //parse italic
            {@"\\([_*])","$1"}//Remove slash before escape symbols

        };

        public string RenderToHtml(string markdown)
        {
            foreach (var rule in Rules)
            {
                markdown = Regex.Replace(markdown, rule.Key, rule.Value, RegexOptions.IgnorePatternWhitespace);
            }
            return markdown;
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

        [TestCase("*qwertyqwerty*", ExpectedResult = "<em>qwertyqwerty</em>")]
        [TestCase("_hello_ world", ExpectedResult = "<em>hello</em> world")]
        public string ParseEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase(@"\_hello world\_", ExpectedResult = @"_hello world_")]
        [TestCase(@"\_hello_ world\_", ExpectedResult = @"_hello_ world_")]
        public string RemoveSlashBeforeEscapeSymbols(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("__qwertyqwerty__", ExpectedResult = "<strong>qwertyqwerty</strong>")]
        [TestCase("__hello world__", ExpectedResult = "<strong>hello world</strong>")]
        [TestCase("__hello__ world", ExpectedResult = "<strong>hello</strong> world")]
        public string ParseStrongTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("___qwertyqwerty___", ExpectedResult = "<strong><em>qwertyqwerty</em></strong>")]
        [TestCase("__he_llo_ world__", ExpectedResult = "<strong>he<em>llo</em> world</strong>")]
        [TestCase("__hello__ _world_", ExpectedResult = "<strong>hello</strong> <em>world</em>")]
        public string ParseMixedStrongAndEmTokens(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }



    }
}