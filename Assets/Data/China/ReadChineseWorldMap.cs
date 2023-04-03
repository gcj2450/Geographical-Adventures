/****************************************************
    文件：ReadChineseWorldMap.cs
    作者：#CREATEAUTHOR#
    邮箱:  gaocanjun@baidu.com
    日期：#CREATETIME#
    功能：Todo
*****************************************************/
using GeoJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class ReadChineseWorldMap : MonoBehaviour
{
    //World_cn.json
    public TextAsset worldCn;

    Country[] countries;

    // Start is called before the first frame update
    private void Start()
    {
        //CountryReader_Chinese countryReader = new CountryReader_Chinese();
        //countries = countryReader.ReadCountries(worldCn);
        //Debug.Log($"countries.Length: {countries.Length}");

        string str = worldCn.text;

        //FeatureCollection featureCollection= JsonConvert.DeserializeObject<FeatureCollection>(str);
        //Debug.Log(featureCollection.features.Count);
    }

    public Country[] GetCountries()
    {
        string str = worldCn.text;
        JObject jsonObj = JObject.Parse(str);
        JArray geos =(JArray) jsonObj["features"];

        List<Country> countries = new List<Country>();
        for (int i = 0; i < geos.Count; i++)
        {
            Country country = new Country();
            country.name = geos[i]["properties"]["name"].ToString();
            int polycnt =(int)geos[i]["properties"]["childNum"];

            country.shape = new Shape();
            country.shape.polygons = new Polygon[polycnt];

            string typeStr = geos[i]["geometry"]["type"].ToString();
            if (typeStr== "Polygon")
            {
                JArray Paths = (JArray)geos[i]["geometry"]["coordinates"];
                Debug.Log($"Polygon  Paths Count:{Paths.Count}");

                country.shape.polygons[0].paths = new Path[Paths.Count];
                for (int kk = 0; kk < Paths.Count; kk++)
                {
                    List<Coordinate> coordList = new List<Coordinate>();
                    JArray potArray = (JArray)Paths[kk];
                    for (int ppp = 0; ppp < potArray.Count; ppp++)
                    {
                        JArray pot = (JArray)potArray[ppp];
                        double x = (double)pot[0];
                        double y = (double)pot[1];
                        Coordinate coord = new Coordinate((float)x * Mathf.Deg2Rad, (float)y * Mathf.Deg2Rad);
                        coordList.Add(coord);

                        Debug.Log($"Polygon pot:{pot.Count}");
                    }

                    coordList.Add(coordList[0]);
                    Path path = new Path() { points = coordList.ToArray() };
                    country.shape.polygons[0].paths[kk] = path;
                    coordList.Clear();
                }
               
            }
            else if (typeStr == "MultiPolygon")
            {
                JArray mpolys = (JArray)geos[i]["geometry"]["coordinates"];
                Debug.Log($"MultiPolygon Count:{mpolys.Count}");
                for (int kk = 0; kk < mpolys.Count; kk++)
                {
                    JArray paths = (JArray)mpolys[kk];
                    country.shape.polygons[kk].paths = new Path[paths.Count];
                    for (int ppp = 0; ppp < paths.Count; ppp++)
                    {
                        List<Coordinate> coordList = new List<Coordinate>();
                        JArray potArray = (JArray)paths[ppp];
                        for (int bb = 0; bb < potArray.Count; bb++)
                        {
                            JArray pot = (JArray)potArray[bb];

                            double x = (double)pot[0];
                            double y = (double)pot[1];

                            Coordinate coord = new Coordinate((float)x * Mathf.Deg2Rad, (float)y * Mathf.Deg2Rad);
                            coordList.Add(coord);

                            Debug.Log($"MultiPolygon pot:{pot.Count}");
                        }
                        coordList.Add(coordList[0]); // duplicate start point at end for conveniece in some other code
                        Path path = new Path() { points = coordList.ToArray() };
                        country.shape.polygons[kk].paths[ppp] = path;
                        coordList.Clear();
                    }

                    
                }

            }

            countries.Add(country);
        }

        Debug.Log(geos.Count+"__"+ countries.Count);
        return countries.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//public class FeatureCollection
//{
//    public string type = "";
//    public List<features> features = new List<features>();
//}

//public class features
//{
//    public geometry geometry;
//    public properties properties;
//}

//public class geometry
//{
//    public string type = "";
//    //public List<List<double[,]>> coordinates = new List<List<double[,]>>();
//    public List<List<double[]>> coordinates { get; set; }
//}

//public class properties
//{
//    public string name = "";
//    public int childNum=0;
//}