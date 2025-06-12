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
        { "Item", new List<InterfaceMethod.TableData>()},
        { "SpaceShip", new List<InterfaceMethod.TableData>()},
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
                    Debug.Log("Enemy csv 로드");
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        //enemyData = null;
                        //string[] values = lines[i].Split(',');
                        ////EnemyBase.INDEX = int.Parse(values[0]);
                        //enemyData.enemyName = values[1].ToString();
                        //enemyData.damage = int.Parse(values[3]);
                        //enemyData.enemyMoveSpeed = float.Parse(values[4]);

                        //item.Value.Add(enemyData);
                    }
                    break;
                        
                case "Item":
                    Debug.Log("Item csv 로드");
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        string[] values = lines[i].Split(',');
                        ItemData itemData = new ItemData();
                        itemData.INDEX = int.Parse(values[0]);
                        itemData.Name = values[1].ToString();
                        itemData.ItemProbabilityTop = int.Parse(values[2]);
                        itemData.ItemProbabilityMid = int.Parse(values[3]);
                        itemData.ItemProbabilityBot = int.Parse(values[4]);
                        itemData.ItemConTime = int.Parse(values[6]);
                        itemData.ItemHeal = int.Parse(values[7]);
                        itemData.ItemShootSpeed = int.Parse(values[8]);
                        itemData.ItemExplRange = float.Parse(values[9]);
                        itemData.ItemMaxRange = int.Parse(values[10]);
                        itemData.ItemIceTime = int.Parse(values[11]);
                        itemData.ItemAlarmTime = int.Parse(values[12]);

                        item.Value.Add(itemData);
                    }
                    break;

                case "SpaceShip":
                    Debug.Log("SpaceShip csv 로드");
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        string[] values = lines[i].Split(',');
                        SpaceShipData spaceShipData = new SpaceShipData();
                        spaceShipData.Name = values[1].ToString();
                        spaceShipData.ShipMoveSpeed = float.Parse(values[2]);
                        spaceShipData.InventorySlotCount = int.Parse(values[3]);
                        spaceShipData.ShipCost = int.Parse(values[4]);

                        item.Value.Add(spaceShipData);
                    }

                    break;
            }
        }
    }
}
