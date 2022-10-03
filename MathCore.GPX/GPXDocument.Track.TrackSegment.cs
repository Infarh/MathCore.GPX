using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MathCore.GPX
{
    public partial class GPXDocument
    {
        public partial class Track
        {
            /// <summary>Сегмент трека</summary>
            public class TrackSegment : IEnumerable<Point>
            {
                /// <summary>Список точек трека</summary>
                private readonly List<Point> _Points = new();
                /// <summary>Список точек трека</summary>
                public ReadOnlyCollection<Point> Points { get; }

                /// <summary>Дополнительная информация о сегменте</summary>
                public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");

                /// <summary>Новый сегмент трека</summary>
                public TrackSegment() => Points = _Points.AsReadOnly();

                /// <summary>Новый сегмент трека</summary>
                /// <param name="points">Точки трека</param>
                public TrackSegment(IEnumerable<Point> points) : this() => _Points.AddRange(points);

                /// <summary>Создать новую точку в конце сегмента трека</summary>
                /// <returns>Созданная точка</returns>
                public Point Create()
                {
                    var point = new Point();
                    _Points.Add(point);
                    return point;
                }

                /// <summary>Добавить точку в конец сегмента трека</summary>
                /// <param name="point">Добавляемая точка</param>
                public void Add(Point point)
                {
                    if (_Points.Contains(point)) return;
                    _Points.Add(point);
                }

                /// <summary>Удалить точку из сегмента трека</summary>
                /// <param name="point">Удаляемая точка</param>
                /// <returns>Истина, если удаление выполнено успешно</returns>
                public bool Remove(Point point) => _Points.Remove(point);

                public void Clear() => _Points.Clear();

                /// <summary>Сохранить сегмент маршрута в XML</summary>
                /// <param name="trk">XML-структкра информации о треке</param>
                public void SaveTo(XElement trk)
                {
                    if (trk.Name.LocalName != nameof(trk)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(trk)}");
                    XElement trkseg;
                    trkseg = new XElement(__GPX_ns + nameof(trkseg));
                    if (Extensions.HasAttributes || Extensions.HasElements) trkseg.Add(Extensions);
                    foreach (var point in _Points) point.SaveTo(trkseg, "trkpt");
                    if (trkseg.HasElements) trk.Add(trkseg);
                }

                /// <summary>Загрузить сегмент маршрута из XML</summary>
                /// <param name="trkseg">XML-структкра информации о сегменте трека</param>
                public void LoadFrom(XElement trkseg)
                {
                    if (trkseg.Name.LocalName != nameof(trkseg)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(trkseg)}");

                    var extensions = trkseg.Element(__GPX_ns + "extensions");
                    if (extensions != null) Extensions = extensions;

                    foreach (var trkpt in trkseg.Elements(__GPX_ns + "trkpt").Where(trkpt => trkpt.HasElements || trkpt.HasAttributes))
                    {
                        var point = new Point();
                        point.LoadFrom(trkpt);
                        _Points.Add(point);
                    }
                }

                IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => _Points.GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => _Points.GetEnumerator();
            }
        }
    }
}
