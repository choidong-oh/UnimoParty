using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetRate
{
    List<int> tworanks = new List<int>() { 1 , 2 };
    List<int> threeranks = new List<int>() { 1, 2, 3 };
    List<int> fourranks = new List<int>() { 1, 2, 2, 3 };
    List<int> fiveranks = new List<int>() { 1, 1, 2, 2, 3 };
    List<int> sixranks = new List<int>() { 1, 1, 2, 2, 3, 3 };
    List<int> sevenranks = new List<int>() {1, 1, 1, 2, 2, 3, 3 };
    List<int> eightranks = new List<int>() { 1, 1, 1, 2, 2, 2, 3, 3 };
    List<int> usebleranks;
    
    int currentPlayerCount;
    int currentRank;

    void ItemRateSetting()
    {
        currentPlayerCount = Manager.Instance.observer.roomInPlayerCount;
    }

   public void SelectList(int playercount)
    {
        switch (playercount)
        {
            case 2:
                usebleranks = tworanks;
                break;

            case 3:
                usebleranks = threeranks;
                break;

            case 4:
                usebleranks = fourranks;
                break;

            case 5:
                usebleranks = fiveranks;
                break;

            case 6:
                usebleranks = sixranks;
                break;

            case 7:
                usebleranks = sevenranks;
                break;

            case 8:
                usebleranks = eightranks;
                break;
        }
    }

    void ChangeUserProbability()
    { 
        switch(usebleranks.Count)
        {
            case 2:

                break;

            case 3:
                usebleranks = threeranks;
                break;

            case 4:
                usebleranks = fourranks;
                break;

            case 5:
                usebleranks = fiveranks;
                break;

            case 6:
                usebleranks = sixranks;
                break;

            case 7:
                usebleranks = sevenranks;
                break;

            case 8:
                usebleranks = eightranks;
                break;
        }
    }

}
