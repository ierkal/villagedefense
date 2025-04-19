using System;

namespace _Scripts.OdinAttributes
{
    public class LogTagAttribute : Attribute
    {
        public string Tag { get; }

        public LogTagAttribute(string tag)
        {
            Tag = tag;
        }
    }
}