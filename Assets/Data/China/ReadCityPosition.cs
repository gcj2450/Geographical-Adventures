/****************************************************
    文件：ReadCityPosition.cs
    作者：#CREATEAUTHOR#
    邮箱:  gaocanjun@baidu.com
    日期：#CREATETIME#
    功能：Todo
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 读出城市的经纬度数据，将其转换到球面坐标，然后使用碰撞器根据碰撞射线，将其贴合到地面，记录位置用于下次加载
/// </summary>
public class ReadCityPosition : MonoBehaviour
{
    public TextAsset TextAsset;
    public GameObject textPrefab;

    public GameObject MainLandMesh;

    List<cityData> cities;
    List<GameObject> citiNames = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //chinacities2.json
        string strs = TextAsset.text;

        cities= JsonConvert.DeserializeObject<List<cityData>>(strs);
        Debug.Log(cities.Count);

        for (int i = 0; i < cities.Count; i++)
        {
            GameObject go = Instantiate(textPrefab);
            go.SetActive(true);
            go.transform.SetParent(textPrefab.transform.parent);
            go.GetComponent<Text>().text = cities[i].name;

            Coordinate coord = new Coordinate((float)cities[i].longitude * Mathf.Deg2Rad, (float)cities[i].latitude * Mathf.Deg2Rad);
            go.transform.localScale = Vector3.one * 0.5f;
            go.transform.position = GeoMaths.CoordinateToPoint(coord, 150);

            Vector3 up= GeoMaths.CoordinateToPoint(coord, 152)-go.transform.position;
            citiNames.Add(go);
            go.transform.LookAt(up);
        }


        JArray jsonObj = JArray.Parse(strs);
        Debug.Log(jsonObj.Count);
    }

    List<cityPos> poss = new List<cityPos>();

    // Update is called once per frame
    void Update()
    {
        //从当前位置向球面发射射线，使地名贴近地面
        if (Input.GetKeyUp(KeyCode.G))
        {
            MainLandMesh.AddComponent<MeshCollider>();
            for (int i = 0; i < cities.Count; i++)
            {
                if (cities[i].name != "台湾省")
                {
                    Coordinate coord = new Coordinate((float)cities[i].longitude * Mathf.Deg2Rad, (float)cities[i].latitude * Mathf.Deg2Rad);

                    Vector3 up = GeoMaths.CoordinateToPoint(coord, 255);
                    var raycast = default(RaycastHit);
                    var isHit = Physics.Raycast(new Ray(up,-up), out raycast, 500);

                    if (isHit)
                    {
                        Debug.Log($" raycast.point: {raycast.point}");
                        citiNames[i].transform.position = raycast.point;
                    }
                    poss.Add(new cityPos(cities[i].name, citiNames[i].transform.position, citiNames[i].transform.localEulerAngles));
                }
                else
                    poss.Add(new cityPos(cities[i].name, citiNames[i].transform.position, citiNames[i].transform.localEulerAngles));
            }

          string posStr=  JsonConvert.SerializeObject(poss);
            Debug.Log(poss.Count);
            File.WriteAllText(Application.dataPath + "/CityPos.txt", posStr, System.Text.Encoding.UTF8);
        }
    }

}

public class cityData
{
    public string letter = "";
    public string name = "";
    public float longitude = 0;
    public float latitude = 0;
}

public class cityPos
{
    public string name = "";
    public float posx = 0;
    public float posy = 0;
    public float posz = 0;

    public float rotx = 0;
    public float roty = 0;
    public float rotz = 0;

    public cityPos(string _name,Vector3 _pos,Vector3 _rot)
    {
        name = _name;
        posx = _pos.x;
        posy = _pos.y;
        posz = _pos.z;

        rotx = _rot.x;
        roty = _rot.y;
        rotz = _rot.z;
    }
}
