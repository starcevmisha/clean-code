using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
    public class LineElement
    {
        public static string Parse(string markdown)
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
                    if (tag.Type == MarkerType.Code)
                    {
                        var newIndex = line.IndexOf("``", i, StringComparison.Ordinal);
                        if (newIndex > 0)
                            i = newIndex - 1;
                    }
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
}