using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MathCore.GPX;

public partial class GPX
{
    /// <summary>Машрут</summary>
    public partial class Route : IEnumerable<Point>
    {
        /// <summary>Имя</summary>
        public string Name { get; set; }
        /// <summary>Коментарий</summary>
        public string Comment { get; set; }
        /// <summary>Описание</summary>
        public string Description { get; set; }
        /// <summary>Источник данных</summary>
        public string Source { get; set; }
        /// <summary> Ссылка</summary>
        public LinkElement Link { get; set; } = new();
        /// <summary>Номер</summary>
        public int Number { get; set; } = -1;
        /// <summary>Тип (классификатор)</summary>
        public string Type { get; set; }
        /// <summary>Дополнительная информация</summary>
        public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");
        /// <summary>Массив точек маршрута</summary>
        private readonly List<Point> _Points = new();

        /// <summary>Массив точек маршрута</summary>
        public ReadOnlyCollection<Point> Points { get; }

        /// <summary>Новый маршрут</summary>
        public Route() => Points = _Points.AsReadOnly();

        public Point Create()
        {
            var point = new Point();
            _Points.Add(point);
            return point;
        }

        public void Add(Point point)
        {
            if (_Points.Contains(point)) return;
            _Points.Add(point);
        }

        public void Clear() => _Points.Clear();

        public bool Remove(Point point) => _Points.Remove(point);

        public void SaveTo(XElement gpx)
        {
            if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
            XElement rte;
            rte = gpx.Element(__GPX_ns + nameof(rte));
            if (rte is null) return;

            if (!string.IsNullOrWhiteSpace(Name)) rte.Add(new XElement(__GPX_ns + "name", Name));
            if (!string.IsNullOrWhiteSpace(Comment)) rte.Add(new XElement(__GPX_ns + "cmt", Comment));
            if (!string.IsNullOrWhiteSpace(Description)) rte.Add(new XElement(__GPX_ns + "desc", Description));
            if (!string.IsNullOrWhiteSpace(Source)) rte.Add(new XElement(__GPX_ns + "src", Source));
            Link.LoadFrom(rte);
            if (Number >= 0) rte.Add(new XElement(__GPX_ns + "number", Number));
            if (!string.IsNullOrWhiteSpace(Type)) rte.Add(new XElement(__GPX_ns + "type", Type));
            if (Extensions.HasAttributes || Extensions.HasElements) rte.Add(Extensions);
            foreach (var point in _Points) point.SaveTo(rte, "rtept");
            if (rte.HasElements) gpx.Add(rte);
        }

        public void LoadFrom(XElement rte)
        {
            if (rte.Name.LocalName != nameof(rte)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(rte)}");

            Name        = (string)rte.Element(__GPX_ns + "name");
            Comment     = (string)rte.Element(__GPX_ns + "cmt");
            Description = (string)rte.Element(__GPX_ns + "desc");
            Source      = (string)rte.Element(__GPX_ns + "src");
            Link.LoadFrom(rte);
            Number = (int?)rte.Element(__GPX_ns + "number") ?? -1;
            Type   = (string)rte.Element(__GPX_ns + "type");
            var extension                     = rte.Element(__GPX_ns + "extension");
            if (extension != null) Extensions = extension;
            foreach (var rtept in rte.Elements(__GPX_ns + "rtept").Where(rtept => rtept.HasElements))
            {
                var point = new Point();
                point.LoadFrom(rtept);
                _Points.Add(point);
            }
        }

        /// <inheritdoc />
        IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => _Points.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Points).GetEnumerator();
    }
}