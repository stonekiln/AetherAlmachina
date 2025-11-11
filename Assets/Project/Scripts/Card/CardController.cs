using System;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public GameObject playerHand;
    [NonSerialized] public List<SkillData> playerDeck;
    SkillDataBase skillDataBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        skillDataBase = transform.parent.GetChild(1).GetComponent<SkillDataBase>();
        skillDataBase.ReadDataBase();
        ReadDeck();
        playerHand.GetComponent<HandController>().deck = FisherYates(playerDeck);
    }

    List<SkillData> FisherYates(List<SkillData> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int RandomNumber = UnityEngine.Random.Range(0, i);
            (array[i], array[RandomNumber]) = (array[RandomNumber], array[i]);
        }
        return array;
    }

    void ReadDeck()
    {
        int[] index = new int[4] { 0, 0, 0, 1 };
        playerDeck = new List<SkillData>();
        for (int i = 1; i < 14; i++)
        {
            foreach (int j in index)
            {
                playerDeck.Add(skillDataBase.Skills[j, i][0]);
            }
        }
    }
}