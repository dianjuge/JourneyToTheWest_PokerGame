using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    None = 0,
    Card_A = 1,
    Card_2 = 1 << 1,
    Card_3 = 1 << 2,
    Card_4 = 1 << 3,
    Card_8 = 1 << 4,
    Card_6 = 1 << 5,
    Card_7 = 1 << 6,
    Card_5 = 1 << 7,
    Card_9 = 1 << 8,
    Card_10= 1 << 9,
    Card_J = 1 << 10,
    Card_Q = 1 << 11,
    Card_K = 1 << 12,
    Card_Joker_Black = 1 << 13,
    Card_Joker_Color = 1 << 14,

    /// <summary>
    /// 师傅牌
    /// </summary>
    Card_Master = Card_10,
    /// <summary>
    /// 徒弟牌
    /// </summary>
    Card_Apprentice = Card_3 | Card_5 | Card_8,
    /// <summary>
    /// 妖怪牌
    /// </summary>
    Card_Evil = Card_2 | Card_4 | Card_6 | Card_7 | Card_9 | Card_J | Card_Q | Card_K,
    /// <summary>
    /// 跳牌
    /// </summary>
    Card_Jump = Card_A | Card_Joker_Black | Card_Joker_Color,
    /// <summary>
    /// 大小王
    /// </summary>
    Card_Joker = Card_Joker_Black | Card_Joker_Black,
}

public enum CardColor
{
    Heart,
    Diamond,
    Club, //梅花
    Spade,//黑桃
    None,
}

public struct Card
{
    public CardColor color;
    public CardType type;

    public Card(CardColor _color, CardType _type)
    {
        color = _color;
        type = _type;
    }
}

public class CardManager
{
    public List<Card> Deck = new List<Card>();
    // Start is called before the first frame update
    public void Init()
    {
        InitiateDeck();
        Deck = RandomSortDeck(Deck);
    }


    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化卡池
    /// </summary>
    void InitiateDeck()
    {
        //将各花色牌加入卡池
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                var curCard = new Card((CardColor)i, (CardType)(1<<j));
                Deck.Add(curCard);
            }
        }
        //将大小王加入卡池
        for (int j = 13; j < 15; j++)
        {
            var curCard = new Card((CardColor)4, (CardType)(1 << j));
            Deck.Add(curCard);
        }

        //打印加入卡池的牌
        foreach (var card in Deck)
        {
            LogManager.PrintLog($"[初始化卡池] {card.color} {card.type} \n");
        }
    }

    /// <summary>
    /// 随机排序卡池中的牌
    /// </summary>
    /// <param name="deck">卡池</param>
    /// <returns></returns>
    List<Card> RandomSortDeck(List<Card> deck)
    {
        List<Card> newDeck = new List<Card>();
        //随机选中原卡池的一枚牌洗入新卡池中
        while (deck.Count > 0)
        {
            var randomCardIndex = Random.Range(0, deck.Count);
            newDeck.Add(deck[randomCardIndex]);
            deck.RemoveAt(randomCardIndex);
        }
        //打印加入卡池的牌
        foreach (var card in newDeck)
        {
            LogManager.PrintLog($"[洗牌]: {card.color} {card.type} \n");
        }
        return newDeck;
    }

}
