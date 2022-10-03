using System;
using System.Xml.Linq;

namespace MathCore.GPX
{
    public partial class GPXDocument
    {
        /// <summary>Ссылка</summary>
        public class LinkElement
        {
            /// <summary>Адрес</summary>
            public Uri href { get; set; } = null!;
            /// <summary>Текст</summary>
            public string Text { get; set; } = null!;
            /// <summary>Тип</summary>
            public string Type { get; set; } = null!;

            public void SaveTo(XElement element, string name)
            {
                if (element.Name.LocalName != name) 
                    throw new ArgumentException($@"Родительский узел не является узлом {name}");

                if (href.ToString() is { Length: > 0 } uri_str) element.Add(new XAttribute("href", uri_str));
                if (Text is { Length           : > 0 } text)    element.Add(new XAttribute(__GPX_ns + "text", text));
                if (Type is { Length           : > 0 } type)    element.Add(new XAttribute(__GPX_ns + "type", type));
            }

            public LinkElement LoadFrom(XElement element)
            {
                if (element.Element(__GPX_ns + "link") is not { } link) 
                    return this;

                if ((string)link.Attribute(nameof(href)) is { Length: >0 } href_str) href = new(href_str);
                Text = (string)link.Attribute(__GPX_ns + nameof(Text).ToLower());
                Type = (string)link.Attribute(__GPX_ns + nameof(Type).ToLower());

                return this;
            }
        }
    }
}