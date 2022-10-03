using System;
using System.Collections.Generic;
using System.Xml.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global

namespace MathCore.GPX;

public partial class GPXDocument
{
    /// <summary>Метаданные</summary>
    public partial class Metadata
    {
        /// <summary>Ссылка</summary>
        public LinkElement Link { get; } = new();
        /// <summary>Имя</summary>
        public string Name { get; set; }
        private const string Name_xml_name = "name";
        /// <summary>Описание</summary>
        public string Description { get; set; }
        private const string Description_xml_name = "desc";
        /// <summary>Информация об авторе</summary>
        public AuthorInfo Author { get; } = new();
        /// <summary>Ключевые слова</summary>
        public List<string> KeyWords { get; } = new();

        public BoundsElement Bounds { get; } = new();
        /// <summary>Дополнительная информация</summary>
        public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");

        public void SaveTo(XElement gpx)
        {
            if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
            XElement metadata;
            metadata = new XElement(__GPX_ns + nameof(metadata));
            Link.SaveTo(metadata, nameof(metadata));
            if (!string.IsNullOrWhiteSpace(Name)) metadata.Add(new XElement(__GPX_ns + Name_xml_name, Name));
            if (!string.IsNullOrWhiteSpace(Description)) metadata.Add(new XElement(__GPX_ns + Description_xml_name, Description));
            Author.SaveTo(metadata);
            if (KeyWords.Count > 0) metadata.Add(new XElement(__GPX_ns + "keywords", string.Join(",", KeyWords)));
            if (Extensions.HasAttributes || Extensions.HasElements) gpx.Add(Extensions);
            if (!metadata.HasElements) return;
            metadata.Add(new XElement(__GPX_ns + "time", DateTime.Now));
            Bounds.SaveTo(metadata);
            gpx.Add(metadata);
        }

        public void LoadFrom(XElement gpx)
        {
            if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
            XElement? metadata;
            metadata = gpx.Element(__GPX_ns + nameof(metadata));
            if (metadata is null) return;
            Link.LoadFrom(metadata);
            Name        = (string)metadata.Element(__GPX_ns + Name_xml_name);
            Description = (string)metadata.Element(__GPX_ns + Description_xml_name);
            var key_words = (string)metadata.Element(__GPX_ns + "keywords");
            if (!string.IsNullOrWhiteSpace(key_words))
            {
                KeyWords.Clear();
                KeyWords.AddRange(key_words.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            Author.LoadFrom(metadata);
            var extensions = metadata.Element(__GPX_ns + "extensions");
            Bounds.LoadFrom(metadata);
            if (extensions != null) Extensions = extensions;
        }
    }
}