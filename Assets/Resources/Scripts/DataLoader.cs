using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader
{
    EnemyBase enemyData;
    public Dictionary<string, List<InterfaceMethod.TableData>> data = new Dictionary<string, List<InterfaceMethod.TableData>>()
    {
        { "Enemy", new List<InterfaceMethod.TableData>()},
        { "ItemData", new List<InterfaceMethod.TableData>()},
    };

    public void DataLoad()
    {
        foreach (var item in data)
        {
            Debug.Log(item.Key);
            TextAsset csvFIles = Resources.Load<TextAsset>($"Table/{item.Key}");

            if (csvFIles == null)
            {
                Debug.Log("Csv 파일이 할당되지 않았습니다!!!");
                return;
            }

            string[] lines = csvFIles.text.Split('\n');
            switch (csvFIles.name)
            {
                case "Enemy":
                    Debug.Log("에너미 csv  로드");
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        enemyData = null;
                        string[] values = lines[i].Split(',');
                        //EnemyBase.INDEX = int.Parse(values[0]);
                        enemyData.enemyName = values[1].ToString();
                        enemyData.damage = int.Parse(values[3]);
                        enemyData.enemyMoveSpeed = float.Parse(values[4]);

                        //item.Value.Add(enemyData);
                    }
                    break;

                case "ItemData":
                    Debug.Log(" csv 로드");
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        string[] values = lines[i].Split(',');
                        ItemData itemData = new ItemData();
                        itemData.INDEX = int.Parse(values[0]);

                        item.Value.Add(itemData);
                    }
                    break;
            }
        }
    }

}
