/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FancyScrollView.Leaderboard
{
    class Leaderboard : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;

        void Start()
        {
            scrollView.OnCellClicked(index => SelectCell(index));
            var myRank = -1; //以out取回
            var leaderboardData = PlayerPrefs.GetString("leaderboard", "");
            if (!String.IsNullOrEmpty(leaderboardData)) 
            {
                GenerateCells(CollectData(leaderboardData, out myRank));//UpdateContents(items);
                SelectCell(myRank > 0 ? myRank : 0);
                //JumpTo
                scrollView.JumpTo(0);
            }
        }

        void TryParseValue(InputField inputField, int min, int max, Action<int> success) //輸入數值限制
        {
            if (!int.TryParse(inputField.text, out int value))
            {
                return;
            }

            if (value < min || value > max)
            {
                inputField.text = Mathf.Clamp(value, min, max).ToString();
                return;
            }

            success(value);
        }

        void SelectCell(int cellIndex = -1) //從InputField找index，不從參數執行
        {
            //Debug.Log($"SelectCell:{cellIndex}");
            if (scrollView.DataCount == 0)
            {
                return;
            }
            if (cellIndex == -1)
            {
                scrollView.ScrollTo(0, 0.3f, Ease.InOutQuint, Alignment.Middle);
            }
            else
            {
                scrollView.ScrollTo(cellIndex, 0.3f, Ease.InOutQuint, Alignment.Middle);
            }
        }

        ItemData[] CollectData(string jsonString, out int myRank) 
        {
            JObject jsonObject = JObject.Parse(jsonString);//解出第一層
            myRank = jsonObject["rank"]?.ToObject<int>() ?? -1; // 默認為 -1(無排名)
            //Debug.Log("我的排名: " + myRank);
            JArray dbArray = (JArray)jsonObject["db"];//解出排行榜資料
            Int32 countLV1 = jsonObject.Properties().Count();
            //Debug.Log($"JSON有{countLV1}個資料");
            // 遍歷 jsonObject 的每個屬性
            //foreach (var property in jsonObject.Properties())
            //{
            //    Debug.Log("- " + property.Name);
            //}

            int dataCount = dbArray.Count();

            //Debug.Log($"db有{dataCount}個資料");
            // 獲取 "db" 陣列的第一個完整物件
            //Debug.Log($"第一筆資料的姓名為: {dbArray?[0]?["name"]}");

            var items = Enumerable.Range(0, dataCount)
                .Select(i =>
                {
                    string rank = $"{(i + 1).ToString("D4")}.";//初始為0，排名從0001開始
                    string userName = dbArray?[i]?["name"].ToString(); ;
                    string score = dbArray?[i]?["score"].ToString(); ;
                    string imageUrl = dbArray?[i]?["avatar"].ToString(); ;
                    return new ItemData(rank, userName, score, imageUrl); // 回傳初始化的 ItemData
                })
                .ToArray();
            return items;
        }

        void GenerateCells(ItemData[] items)
        {
            scrollView.UpdateData(items);
            //Debug.Log($"scrollView.DataCount:{scrollView.DataCount}");//UpdateData後會自動更新DataCount
        }
    }
}
