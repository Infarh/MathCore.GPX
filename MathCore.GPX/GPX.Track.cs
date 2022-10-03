using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global

namespace MathCore.GPX
{
    public partial class GPX
    {
        /// <summary>Трек</summary>
        public partial class Track : IEnumerable<Track.TrackSegment>
        {
            /// <summary>Имя</summary>
            public string Name { get; set; }
            private const string Name_xml_name = "name";
            /// <summary>Коментарий</summary>
            public string Comment { get; set; }
            private const string Comment_xml_name = "cmt";
            /// <summary>Описание</summary>
            public string Description { get; set; }
            private const string Description_xml_name = "desc";
            /// <summary>Источник данных</summary>
            public string Source { get; set; }
            private const string Source_xml_name = "src";
            /// <summary>Номер</summary>
            public int Number { get; set; } = -1;
            private readonly string Number_xml_name = "number";
            /// <summary>Тип (классификатор)</summary>
            public string Type { get; set; }
            private const string Type_xml_name = "type";

            /// <summary>Дополнительная информация о треке</summary>
            public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");

            /// <summary>Список сегментов трека</summary>
            private readonly List<TrackSegment> _Segments = new();

            /// <summary>Список сегментов трека</summary>
            public ReadOnlyCollection<TrackSegment> Segments { get; }

            /// <summary>Новый трек</summary>
            public Track() => Segments = _Segments.AsReadOnly();

            /// <summary>Создать новый сегмент в конце трека</summary>
            /// <returns>Новый трек</returns>
            public TrackSegment Create()
            {
                var segment = new TrackSegment();
                _Segments.Add(segment);
                return segment;
            }

            /// <summary>Добавить сегмент в конец трека</summary>
            /// <param name="segment">Добавляемый сегмент трека</param>
            public void Add(TrackSegment segment)
            {
                if (_Segments.Contains(segment)) return;
                _Segments.Add(segment);
            }

            /// <summary>Удалить сегмент трека</summary>
            /// <param name="segment">Удаляемый сегмент трека</param>
            /// <returns>Истина, если удаление сегмента трека прошло успешно</returns>
            public bool Remove(TrackSegment segment) => _Segments.Remove(segment);

            public void Clear() => _Segments.Clear();

            /// <summary>Сохранить машрут в XML</summary>
            /// <param name="gpx">XML-структура файла GPX</param>
            public void SaveTo(XElement gpx)
            {
                if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
                XElement trk;
                trk = new XElement(__GPX_ns + nameof(trk));
                if (!string.IsNullOrWhiteSpace(Name)) trk.Add(new XElement(__GPX_ns + Name_xml_name, Name));
                if (!string.IsNullOrWhiteSpace(Comment)) trk.Add(new XElement(__GPX_ns + Comment_xml_name, Comment));
                if (!string.IsNullOrWhiteSpace(Description)) trk.Add(new XElement(__GPX_ns + Description_xml_name, Description));
                if (!string.IsNullOrWhiteSpace(Source)) trk.Add(new XElement(__GPX_ns + Source_xml_name, Source));
                if (Number >= 0) trk.Add(new XElement(__GPX_ns + Number_xml_name, Number));
                if (!string.IsNullOrWhiteSpace(Type)) trk.Add(new XElement(__GPX_ns + Type_xml_name, Type));

                if (Extensions.HasAttributes || Extensions.HasElements) trk.Add(Extensions);

                foreach (var segment in _Segments) segment.SaveTo(trk);

                if (trk.HasElements || trk.HasAttributes) gpx.Add(trk);
            }

            /// <summary>Загрузить трек из XML</summary>
            /// <param name="trk">XML-структура блока данных трека</param>
            public void LoadFrom(XElement trk)
            {
                if (trk.Name.LocalName != nameof(trk)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(trk)}");
                Name = (string)trk.Element(__GPX_ns + Name_xml_name);
                Comment = (string)trk.Element(__GPX_ns + Comment_xml_name);
                Description = (string)trk.Element(__GPX_ns + Description_xml_name);
                Source = (string)trk.Element(__GPX_ns + Source_xml_name);
                Number = (int?)trk.Element(__GPX_ns + Number_xml_name) ?? -1;
                Type = (string)trk.Element(__GPX_ns + Type_xml_name);

                var extensions = trk.Element(__GPX_ns + "extensions");
                if (extensions != null) Extensions = extensions;

                foreach (var trkseg in trk.Elements(__GPX_ns + "trkseg").Where(trkseg => trkseg.HasElements))
                {
                    var segment = new TrackSegment();
                    segment.LoadFrom(trkseg);
                    _Segments.Add(segment);
                }
            }

            IEnumerator<TrackSegment> IEnumerable<TrackSegment>.GetEnumerator() => _Segments.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _Segments.GetEnumerator();
        }
    }
}
