using System;
using System.Xml.Linq;

namespace MathCore.GPX;

public partial class GPX
{
    public partial class Metadata
    {
        public partial class BoundsElement
        {
            public double MinLatitude { get; set; } = double.NaN;
            public double MaxLatitude { get; set; } = double.NaN;
            public double MinLongitude { get; set; } = double.NaN;
            public double MaxLongitude { get; set; } = double.NaN;

            public bool IsEmpty => double.IsNaN(MinLatitude) && double.IsNaN(MaxLatitude) && double.IsNaN(MinLongitude) && double.IsNaN(MaxLongitude);

            public void SaveTo(XElement metadata)
            {
                if (metadata.Name.LocalName != nameof(metadata)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(metadata)}");
                if (IsEmpty) return;
                metadata.Add(new XElement(__GPX_ns + "bounds",
                    new XAttribute("minlat", MinLatitude),
                    new XAttribute("mimlon", MinLongitude),
                    new XAttribute("maxlat", MaxLatitude),
                    new XAttribute("maxlon", MaxLongitude)));
            }

            public void LoadFrom(XElement metadata)
            {
                if (metadata.Name.LocalName != nameof(metadata)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(metadata)}");
                XElement bounds = metadata.Element(__GPX_ns + nameof(bounds));
                if (bounds is null) return;
                MinLatitude  = (double?)bounds.Attribute("minlat") ?? double.NaN;
                MaxLatitude  = (double?)bounds.Attribute("maxlat") ?? double.NaN;
                MinLongitude = (double?)bounds.Attribute("minlon") ?? double.NaN;
                MaxLongitude = (double?)bounds.Attribute("maxlon") ?? double.NaN;
            }
        }
    }
}