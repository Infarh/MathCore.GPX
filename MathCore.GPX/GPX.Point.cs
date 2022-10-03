using System;
using System.Xml.Linq;

namespace MathCore.GPX;

partial class GPX
{
    /// <summary>Точка маршрута</summary>
    public class Point
    {
        /// <summary>Широта</summary>
        public double Lattitude { get; set; } = double.NaN;
        private const string Lattitude_xml_name = "lat";
        /// <summary>Долгота</summary>
        public double Longitude { get; set; } = double.NaN;
        private const string Longitude_xml_name = "lon";
        /// <summary>Высота</summary>
        public double Height { set; get; } = double.NaN;
        private const string Height_xml_name = "h";
        /// <summary>Курс</summary>
        public double Cource { set; get; } = double.NaN;
        private const string Cource_xml_name = "cource";

        /// <summary>Высота</summary>
        public double Elevation { get; set; } = double.NaN;
        private const string Elevation_xml_name = "ele";
        /// <summary>Время</summary>
        public DateTime? Time { get; set; }
        private const string Time_xml_name = "time";
        /// <summary>Точность магнитного курса в градусах</summary>
        public double MagneticVariation { get; set; }
        private const string MagneticVariation_xml_name = "magvar";
        /// <summary>Высота над геоидом</summary>
        public double GeoidHeight { get; set; } = double.NaN;
        private const string GeoidHeight_xml_name = "geoidheight";
        /// <summary>Имя</summary>
        public string Name { get; set; }
        private const string Name_xml_name = "name";
        /// <summary>Комментарий</summary>
        public string Comment { get; set; }
        private const string Comment_xml_name = "cmt";
        /// <summary>Описание</summary>
        public string Description { get; set; }
        private const string Description_xml_name = "desc";
        /// <summary>Источник данных</summary>
        public string Source { get; set; }
        private const string Source_xml_name = "src";
        /// <summary>Имя символа на карте</summary>
        public string SymbolName { get; set; }
        private const string SymbolName_xml_name = "sym";
        /// <summary>Тип (классификация)</summary>
        public string Type { get; set; }
        private const string Type_xml_name = "type";
        /// <summary>Количество спутников</summary>
        public int SatelliteCount { get; set; } = -1;
        private const string SatelliteCount_xml_name = "sat";

        /// <summary>Дополнительные данные</summary>
        public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");

        /// <summary>Данные точки корректны</summary>
        public bool Correct => !double.IsNaN(Lattitude) && !double.IsNaN(Longitude);

        /// <summary>Новая точка маршрута</summary>
        public Point() { }

        /// <summary>Новая точка маршрута</summary>
        /// <param name="Lattitude">Широта</param>
        /// <param name="Longitude">Долгота</param>
        /// <param name="Height">Высота</param>
        /// <param name="Cource">Курс</param>
        /// <param name="Time">Время</param>
        public Point(double Lattitude, double Longitude, double Height = double.NaN, double Cource = double.NaN, DateTime? Time = null)
        {
            this.Lattitude = Lattitude;
            this.Longitude = Longitude;
            this.Height    = Height;
            this.Cource    = Cource;
            this.Time      = Time;
        }

        /// <summary>Сохранить в XML</summary>
        /// <param name="points">Элемент XML-структуры, содержащий информацию о точках</param>
        /// <param name="name">Имя рзадела</param>
        public void SaveTo(XElement points, string name)
        {
            var point = new XElement(__GPX_ns + name);
            if (!double.IsNaN(Lattitude)) point.Add(new XAttribute(Lattitude_xml_name, Lattitude));
            if (!double.IsNaN(Longitude)) point.Add(new XAttribute(Longitude_xml_name, Longitude));
            if (!double.IsNaN(Height)) point.Add(new XAttribute(Height_xml_name, Height));
            if (!double.IsNaN(Cource)) point.Add(new XAttribute(Cource_xml_name, Cource));
            if (Time != null) point.Add(new XElement(__GPX_ns + Time_xml_name, Time));

            if (!double.IsNaN(Elevation)) point.Add(new XElement(__GPX_ns + Elevation_xml_name, Elevation));
            if (!double.IsNaN(MagneticVariation)) point.Add(new XElement(__GPX_ns + MagneticVariation_xml_name, MagneticVariation));
            if (!double.IsNaN(GeoidHeight)) point.Add(new XElement(__GPX_ns + GeoidHeight_xml_name, GeoidHeight));

            if (!string.IsNullOrWhiteSpace(Name)) point.Add(new XElement(__GPX_ns + Name_xml_name, Name));
            if (!string.IsNullOrWhiteSpace(Comment)) point.Add(new XElement(__GPX_ns + Comment_xml_name, Comment));
            if (!string.IsNullOrWhiteSpace(Description)) point.Add(new XElement(__GPX_ns + Description_xml_name, Description));
            if (!string.IsNullOrWhiteSpace(Source)) point.Add(new XElement(__GPX_ns + Source_xml_name, Source));
            if (!string.IsNullOrWhiteSpace(SymbolName)) point.Add(new XElement(__GPX_ns + SymbolName_xml_name, SymbolName));
            if (!string.IsNullOrWhiteSpace(Type)) point.Add(new XElement(__GPX_ns + Type_xml_name, Type));
            if (SatelliteCount >= 0) point.Add(new XElement(__GPX_ns + SatelliteCount_xml_name, SatelliteCount));

            if (Extensions.HasAttributes || Extensions.HasElements) point.Add(Extensions);
            if (point.HasAttributes || point.HasElements) points.Add(point);
        }

        /// <summary>Загрузить из XML</summary>
        /// <param name="point">Элемент XML-структуры, содержащий информацию точке</param>
        public void LoadFrom(XElement point)
        {
            Lattitude = (double?)point.Attribute(Lattitude_xml_name) ?? double.NaN;
            Longitude = (double?)point.Attribute(Longitude_xml_name) ?? double.NaN;
            Height    = (double?)point.Attribute(Height_xml_name) ?? double.NaN;
            Cource    = (double?)point.Attribute(Cource_xml_name) ?? double.NaN;

            Elevation         = (double?)point.Element(__GPX_ns + Elevation_xml_name) ?? double.NaN;
            Time              = (DateTime?)point.Element(__GPX_ns + Time_xml_name);
            MagneticVariation = (double?)point.Element(__GPX_ns + MagneticVariation_xml_name) ?? double.NaN;
            GeoidHeight       = (double?)point.Element(__GPX_ns + GeoidHeight_xml_name) ?? double.NaN;
            Name              = (string)point.Element(__GPX_ns + Name_xml_name);
            Comment           = (string)point.Element(__GPX_ns + Comment_xml_name);
            Description       = (string)point.Element(__GPX_ns + Description_xml_name);
            Source            = (string)point.Element(__GPX_ns + Source_xml_name);
            SymbolName        = (string)point.Element(__GPX_ns + SymbolName_xml_name);
            Type              = (string)point.Element(__GPX_ns + Type_xml_name);
            SatelliteCount    = (int?)point.Element(__GPX_ns + Type_xml_name) ?? -1;

            var extensions                     = point.Element(__GPX_ns + "extensions");
            if (extensions != null) Extensions = extensions;
        }

        /// <inheritdoc />
        public override string ToString() => $"{Lattitude};{Longitude};{Height}";
    }
}