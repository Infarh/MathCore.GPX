﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global

namespace MathCore.GPX;

public partial class GPX : IEnumerable<GPX.Track>
{
    private static readonly XNamespace __GPX_ns = XNamespace.Get("http://www.topografix.com/GPX/1/1");
    private static readonly XNamespace __GPXX = XNamespace.Get("http://www.garmin.com/xmlschemas/GpxExtensions/v3");
    private static readonly XNamespace __GPX_tpx = XNamespace.Get("http://www.garmin.com/xmlschemas/TrackPointExtension/v1");

    public static GPX Open(string file)
    {
        var gpx = new GPX();
        gpx.Load(file);
        return gpx;
    }

    public string Creator { get; set; }
    private const string __Creator_xml_name = "creator";
    public DateTime Time { get; set; }
    private const string __Time_xml_name = "time";

    public Metadata MetaData { get; } = new();

    private readonly List<Point> _WayPoints = new();
    public ReadOnlyCollection<Point> WayPoints { get; }

    private readonly List<Route> _Routes = new();
    public ReadOnlyCollection<Route> Routes { get; }

    private readonly List<Track> _Tracks = new();
    public ReadOnlyCollection<Track> Tracks { get; }

    public XElement Extensions { get; private set; } = new(__GPX_ns + "extensions");

    public GPX()
    {
        WayPoints = _WayPoints.AsReadOnly();
        Routes    = _Routes.AsReadOnly();
        Tracks    = _Tracks.AsReadOnly();
    }

    public Track Create()
    {
        var track = new Track();
        _Tracks.Add(track);
        return track;
    }

    public void Add(Track track)
    {
        if (_Tracks.Contains(track)) return;
        _Tracks.Add(track);
    }

    public bool Remove(Track track) => _Tracks.Remove(track);

    public void Save(string FileName)
    {
        var      xsi_ns = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
        XElement gpx;
        gpx = new XElement(__GPX_ns + nameof(gpx),
            new XAttribute(xsi_ns + "schemaLocation", "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.garmin.com/xmlschemas/GpxExtensions/v3 http://www.garmin.com/xmlschemas/GpxExtensionsv3.xsd http://www.garmin.com/xmlschemas/TrackPointExtension/v1 http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd"),
            new XAttribute(XNamespace.Xmlns + "xsi", xsi_ns),
            new XAttribute(XNamespace.Xmlns + "gpxx", __GPXX),
            new XAttribute(XNamespace.Xmlns + "gpxtpx", __GPX_tpx),
            new XAttribute("version", "1.1"));
        SaveTo(gpx);

        new XDocument(new XDeclaration("1.0", "utf-8", null), gpx).Save(FileName);
    }

    public void SaveTo(XElement gpx)
    {
        if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
        if (!string.IsNullOrWhiteSpace(Creator)) gpx.Add(new XAttribute(__Creator_xml_name, Creator));
        if (Time != default) gpx.Add(new XElement(__GPX_ns + __Time_xml_name, Time));
        if (MetaData.Bounds.IsEmpty)
        {
            GetBounds(_Tracks.SelectMany(trk => trk.Segments).SelectAll(), out var min_lat, out var max_lat, out var min_lon, out var max_lon);
            var bnd = MetaData.Bounds;
            bnd.MinLatitude  = min_lat;
            bnd.MaxLatitude  = max_lat;
            bnd.MinLongitude = min_lon;
            bnd.MaxLongitude = max_lon;
        }
        MetaData.SaveTo(gpx);
        foreach (var way_point in _WayPoints)
            way_point.SaveTo(gpx, "wpt");
        foreach (var track in _Tracks)
            track.SaveTo(gpx);
        if (Extensions.HasAttributes || Extensions.HasElements) gpx.Add(Extensions);
    }

    private static void GetBounds(IEnumerable<Point> points, out double min_latitude, out double max_latitude, out double min_longitude, out double max_longitude)
    {
        min_latitude = min_longitude = double.PositiveInfinity;
        max_latitude = max_longitude = double.NegativeInfinity;
        foreach (var point in points)
        {
            var lat                                = point.Lattitude;
            var lon                                = point.Longitude;
            if (lat < min_latitude) min_latitude   = lat;
            if (lat > max_latitude) max_latitude   = lat;
            if (lon < min_longitude) min_longitude = lon;
            if (lon > max_longitude) max_longitude = lon;
        }

        if (double.IsInfinity(min_longitude))
            min_longitude = min_latitude = max_longitude = max_latitude = double.NaN;
    }

    public void Load(string FileName) => LoadFrom(XDocument.Load(FileName).Root ?? throw new InvalidOperationException());

    public void LoadFrom(XElement gpx)
    {
        if (gpx.Name.LocalName != nameof(gpx)) throw new ArgumentException($@"Родительский узел не является узлом {nameof(gpx)}");
        Creator = (string)gpx.Attribute(__Creator_xml_name);
        Time    = (DateTime?)gpx.Element(__GPX_ns + __Time_xml_name) ?? default(DateTime);
        MetaData.LoadFrom(gpx);
        foreach (var wpt in gpx.Elements(__GPX_ns + "wpt").Where(wpt => wpt.HasElements))
        {
            var way_point = new Point();
            way_point.LoadFrom(wpt);
            _WayPoints.Add(way_point);
        }

        foreach (var trk in gpx.Elements(__GPX_ns + "trk").Where(trk => trk.HasElements))
        {
            var track = new Track();
            track.LoadFrom(trk);
            _Tracks.Add(track);
        }
        var extensions                     = gpx.Element(__GPX_ns + "extensions");
        if (extensions != null) Extensions = extensions;
    }

    IEnumerator<Track> IEnumerable<Track>.GetEnumerator() => _Tracks.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _Tracks.GetEnumerator();
}