using NUnit.Framework;

namespace Markdown
{
    public class HtmlTag
    {
        public string Tag;
        private readonly string data;
        public int Length => data.Length;
        public HtmlTag(string tag, string data)
        {
            this.data = data;
            Tag = tag;
        }

        public string InsertInTag(string body)
        {
            return string.IsNullOrEmpty(Tag) ? body : $"<{Tag}>{body}</{Tag}>";
        }

        public override string ToString()
        {
            return InsertInTag(data);
        }
    }


    public class HtmlToken_Should
    {
        [TestCase("em", "hello world", ExpectedResult = "<em>hello world</em>")]
        public string ShouldInsertDataInToTags_WhenToStringCalls(string tag, string data)
        {
            return new HtmlTag("em", data).ToString();
        }

    }
}