using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using GeoJSON.Net;

public class Coastline : MonoBehaviour
{
	public TextAsset coastlineFile;
	public bool draw;

	void Start()
	{
		if (draw)
		{
			Draw();
		}
	}


	public PolyLine[] Read()
	{

		List<PolyLine> paths = new List<PolyLine>();

		GeoJSON.Net.Feature.FeatureCollection collection = new GeoJSON.Net.Feature.FeatureCollection();
		collection = JsonConvert.DeserializeObject<GeoJSON.Net.Feature.FeatureCollection>(coastlineFile.text);
		for (int i = 0; i < collection.Features.Count; i++)
		{
			var f = collection.Features[i];
			if (f.Geometry.Type == GeoJSON.Net.GeoJSONObjectType.LineString)
			{
				var lineString = f.Geometry as GeoJSON.Net.Geometry.LineString;
				paths.Add(new PolyLine(GetCoordinates(lineString)));
			}
			if (f.Geometry.Type == GeoJSON.Net.GeoJSONObjectType.MultiLineString)
			{
				var multilineString = f.Geometry as GeoJSON.Net.Geometry.MultiLineString;
				foreach (var lineString in multilineString.Coordinates)
				{
					paths.Add(new PolyLine(GetCoordinates(lineString)));
				}
			}
		}

		return paths.ToArray();
	}





	Coordinate[] GetCoordinates(GeoJSON.Net.Geometry.LineString lineString)
	{
		Coordinate[] coordinates = new Coordinate[lineString.Coordinates.Count];
		for (int j = 0; j < coordinates.Length; j++)
		{
			float lat = (float)lineString.Coordinates[j].Latitude * Mathf.Deg2Rad;
			float longitude = (float)lineString.Coordinates[j].Longitude * Mathf.Deg2Rad;
			Coordinate coord = new Coordinate(longitude, lat);
			coordinates[j] = coord;
		}
		return coordinates;
	}

	void Draw()
	{
		PolyLine[] paths = Read();
		OutlineRenderer outlineRenderer = GetComponent<OutlineRenderer>();


		List<LineSegment> lineSegments = new List<LineSegment>();

		foreach (PolyLine path in paths)
		{
			Coordinate[] path2D = path.points;

			for (int i = 0; i < path2D.Length - 1; i++)
			{
				LineSegment lineSegment = new LineSegment();
				Vector3 a = path2D[i].ToVector2();
				Vector3 b = path2D[i + 1].ToVector2();

				lineSegment.pointA = a;
				lineSegment.pointB = b;
				lineSegments.Add(lineSegment);
			}

		}

		outlineRenderer.Add(lineSegments.ToArray(), Color.white);

	}
}
