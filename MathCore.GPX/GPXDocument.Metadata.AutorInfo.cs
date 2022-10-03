using System;
using System.Xml.Linq;

namespace MathCore.GPX;

public partial class GPXDocument
{
    public partial class Metadata
    {
        /// <summary>Информаиця об авторе</summary>
        public class AuthorInfo
        {
            /// <summary>Имя</summary>
            public string Name { get; set; } = null!;
            private const string Name_xml_name = "name";

            public void SaveTo(XElement metadata)
            {
                if (metadata.Name.LocalName != nameof(metadata)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(metadata)}");
                XElement author;
                author = new XElement(__GPX_ns + nameof(author));
                if (!string.IsNullOrWhiteSpace(Name)) author.Add(new XElement(__GPX_ns + Name_xml_name, Name));
                if (author.HasElements) metadata.Add(author);
            }

            public void LoadFrom(XElement metadata)
            {
                if (metadata.Name.LocalName != nameof(metadata)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(metadata)}");
                XElement? author;
                author = metadata.Element(__GPX_ns + nameof(author));
                if (author is null) return;
                Name = (string)author.Element(__GPX_ns + Name_xml_name);
            }
        }
    }
}