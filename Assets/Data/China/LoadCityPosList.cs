/****************************************************
    文件：LoadCityPosList.cs
    作者：#CREATEAUTHOR#
    邮箱:  gaocanjun@baidu.com
    日期：#CREATETIME#
    功能：Todo
*****************************************************/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadCityPosList : MonoBehaviour
{
    public TextAsset posList;

    List<cityPos> poss = new List<cityPos>();
    public GameObject textPrefab;

    public GameObject TextUI;
    public Text LocText;
    public Button CloseBtn;
    public Button QuitBtn;

    // Start is called before the first frame update
    void Start()
    {
        poss= JsonConvert.DeserializeObject<List<cityPos>>(posList.text);

        for (int i = 0; i < poss.Count; i++)
        {
            GameObject go = Instantiate(textPrefab);
            go.GetComponent<PageItem>().OnClickHandler+=ItemClick;
            go.SetActive(true);
            go.transform.SetParent(textPrefab.transform.parent);
            go.GetComponent<Text>().text = poss[i].name;

            go.transform.localScale = Vector3.one * 0.5f;
            go.transform.position = new Vector3(poss[i].posx, poss[i].posy, poss[i].posz);
            go.transform.localEulerAngles= new Vector3(poss[i].rotx, poss[i].roty, poss[i].rotz);
        }

        CloseBtn.onClick.AddListener(ClickBtnClick);
        QuitBtn.onClick.AddListener(QuitBtnClick);
    }

    private void ClickBtnClick()
    {
        TextUI.SetActive(false);
    }

    private void QuitBtnClick()
    {
        Application.Quit();
    }

    private void ItemClick(PageItem arg1, PageItemData arg2)
    {
        LocText.text = arg1.GetComponent<Text>().text;
        Debug.Log(arg1.GetComponent<Text>().text);
        TextUI.SetActive(true);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
