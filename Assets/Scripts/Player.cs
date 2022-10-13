using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    //玩家编号
    public int id;
    //玩家是否正在游玩
    public bool isPlaying = false;
    //手牌池
    public List<Card> handDeck = new List<Card>();

    //吃牌池
    public List<Card> consumeDeck = new List<Card>();

    public Player(int _id)
    {
        id = _id;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    public void ShowHandDeck()
    {
        if (handDeck.Count == 0)
        {
            Debug.Log($"[显示手牌] 玩家:{id} 没有手牌! \n");
            return;
        }

        Debug.Log($"[显示手牌] 玩家:{id} 手牌数:{handDeck.Count} \n");
        for (int i = 0; i < handDeck.Count; i++)
        {
            var card = handDeck[i];
            Debug.Log($"[显示手牌] 玩家:{id} 第:{i+1}张牌: {card.color} {card.type} \n");
        }
    }

    /// <summary>
    /// 显示吃牌
    /// </summary>
    public void ShowConsumeDeck()
    {
        if(consumeDeck.Count == 0)
        {
            Debug.Log($"[显示吃牌] 玩家:{id} 没有吃牌! \n");
            return;
        }

        Debug.Log($"[显示吃牌] 玩家:{id} 吃牌数:{consumeDeck.Count} \n");
        for (int i = 0; i < consumeDeck.Count; i++)
        {
            var card = consumeDeck[i];
            Debug.Log($"[显示吃牌] 玩家:{id} 第:{i+1}张牌: {card.color} {card.type} \n");
        }
    }

    public void AddCardToHandDeck(Card card)
    {
        handDeck.Add(card);
    }

    public void AddCardToConsumeDeck(Card card)
    {
        consumeDeck.Add(card);
    }
}
