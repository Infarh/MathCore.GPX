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
        public string Name { get; set; } = null!;
        private const string Name_xml_name = "name";

        /// <summary>Описание</summary>
        public string Description { get; set; } = null!;
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
            var metadata = new XElement(__GPX_ns + "metadata");
            Link.SaveTo(metadata, "metadata");
            if (Name is { Length: > 0 } name) metadata.Add(new XElement(__GPX_ns + Name_xml_name, name));
            if (Description is { Length: > 0} description) metadata.Add(new XElement(__GPX_ns + Description_xml_name, description));
            Author.SaveTo(metadata);
            if (KeyWords.Count > 0) metadata.Add(new XElement(__GPX_ns + "keywords", string.Join(",", KeyWords)));
            if (Extensions.HasAttributes || Extensions.HasElements) gpx.Add(Extensions);
            if (!metadata.HasElements) return;
            metadata.Add(new XElement(__GPX_ns + "time", DateTime.Now));
            Bounds.SaveTo(metadata);
            gpx.Add(metadata);
        }

        public Metadata LoadFrom(XElement gpx)
        {
            if (gpx.Name.LocalName != nameof(gpx)) 
                throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");

            if (gpx.Element(__GPX_ns + "metadata") is not { } metadata) 
                return this;

            Link.LoadFrom(metadata);
            Name          = (string)metadata.Element(__GPX_ns + Name_xml_name);
            Description   = (string)metadata.Element(__GPX_ns + Description_xml_name);

            if ((string)metadata.Element(__GPX_ns + "keywords") is { Length: > 0 } key_words)
            {
                KeyWords.Clear();
                KeyWords.AddRange(key_words.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            Author.LoadFrom(metadata);
            var extensions = metadata.Element(__GPX_ns + "extensions");
            Bounds.LoadFrom(metadata);

            if (extensions != null) 
                Extensions = extensions;

            return this;
        }
    }
}