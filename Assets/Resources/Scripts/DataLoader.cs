using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{

    public Dictionary<string, List<InterfaceMethod.TableData>> data = new Dictionary<string, List<InterfaceMethod.TableData>>()
    {
        { "Enemy", new List<InterfaceMethod.TableData>()},
        { "UserPlayer", new List<InterfaceMethod.TableData>()},
    };

    void Awake()
    {
        //DataLoad();
    }

    void DataLoad()
    {
        foreach (var item in data)
        {
            TextAsset csvFIles = Resources.Load<TextAsset>($"Tables/{item.Key}");

            if (csvFIles == null)
            {
                Debug.Log("Csv 파일이 할당되지 않았습니다!!!");
                return;
            }

            string[] lines = csvFIles.text.Split('\n');
            switch (csvFIles.name)
            {
                case "Enemy":
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        string[] values = lines[i].Split(',');
                        Enemy enemyData = new Enemy();

                        item.Value.Add(enemyData);
                    }
                    break;

                case "UserPlayer":
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        string[] values = lines[i].Split(',');
                        UserPlayer characterData = new UserPlayer();

                        item.Value.Add(characterData);
                    }
                    break;
            }
        }
    }

}
