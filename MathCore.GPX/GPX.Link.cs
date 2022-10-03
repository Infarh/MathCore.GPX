using System;
using System.Xml.Linq;

namespace MathCore.GPX
{
    public partial class GPX
    {
        /// <summary>Ссылка</summary>
        public class LinkElement
        {
            /// <summary>Адрес</summary>
            public Uri href { get; set; }
            /// <summary>Текст</summary>
            public string Text { get; set; }
            /// <summary>Тип</summary>
            public string Type { get; set; }

            public void SaveTo(XElement element, string name)
            {
                if (element.Name.LocalName != name) throw new ArgumentException($@"Родительский узел не является узлом {name}");
                var uri_str = href.ToString();
                if (!string.IsNullOrWhiteSpace(uri_str)) element.Add(new XAttribute("href", uri_str));
                var text = Text;
                if (!string.IsNullOrWhiteSpace(text)) element.Add(new XAttribute(__GPX_ns + "text", text));
                var type = Type;
                if (!string.IsNullOrWhiteSpace(text)) element.Add(new XAttribute(__GPX_ns + "type", type));
            }

            public void LoadFrom(XElement element)
            {
                XElement link;
                link = element.Element(__GPX_ns + nameof(link));
                if (link is null) return;
                var href_str = (string)link.Attribute(nameof(href));
                if (!string.IsNullOrWhiteSpace(href_str)) href = new Uri(href_str);
                Text = (string)link.Attribute(__GPX_ns + nameof(Text).ToLower());
                Type = (string)link.Attribute(__GPX_ns + nameof(Type).ToLower());
            }
        }
    }
}