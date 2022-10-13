using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum GameState
{
    None,
    EnterState,
    DealState,
    PlayState,
    CountState,
    EndState,
}

public class GameManager
{
    //公共卡池
    public List<Card> publicDeck = new List<Card>();
    //上一回合公共牌打出数
    public int lastPublicCardCount = 0;

    //玩家列表
    public List<Player> players = new List<Player>();
    //玩家排名
    public List<Player> rank = new List<Player>();

    //游戏状态
    GameState gameState = GameState.None;

    //当前回合玩家
    public Player curPlayer;
    //当前回合玩家Id
    public int curPlayerId;

    //当前玩家回合是否结束
    public bool isCurPlayerTurnOver;

    //公共区跳牌前的牌
    Card publicpPreJumpCard = new Card(CardColor.None, CardType.None);

    //出牌按钮
    Button playCardBtn;
    //吃牌按钮
    Button consumeCardBtn;
    //出牌选框
    GameObject playCardInputField;
    // Start is called before the first frame update
    public void Init()
    {
        ChangeState(GameState.EnterState);

        //初始化出牌按钮
        playCardBtn = GameObject.Find("GameEnter").transform.Find("Canvas/PlayCard").GetComponent<Button>();
        playCardBtn.onClick.AddListener(PlayerSelectCard);
        //初始化吃牌按钮
        consumeCardBtn = GameObject.Find("GameEnter").transform.Find("Canvas/Consume").GetComponent<Button>();
        consumeCardBtn.onClick.AddListener(PlayerConsumeCard);
        //初始化出牌选框
        playCardInputField = GameObject.Find("GameEnter").transform.Find("Canvas/InputField").gameObject;

    }



    // Update is called once per frame
    public void Update()
    {
        switch (gameState)
        {
            case GameState.EnterState:
                PlayerEnter();
                break;
            case GameState.DealState:
                DealCard();
                break;
            case GameState.PlayState:
                PlayCard();
                break;
            case GameState.CountState:
                CalculateRank();
                break;
        }
    }





    /// <summary>
    /// 玩家入场
    /// </summary>
    private void PlayerEnter()
    {
        var playerCount = 2;
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new Player(i + 1));
        }

        foreach (var player in players)
        {
            player.isPlaying = true;
            Debug.Log($"player{player.id} enter \n");
        }

        curPlayer = players[0];
        curPlayerId = players[0].id;
        isCurPlayerTurnOver = true;

        ChangeState(GameState.DealState);
    }

    /// <summary>
    /// 发牌
    /// </summary>
    private void DealCard()
    {
        //洗牌
        Singleton<CardManager>.Inst.Init();

        //给每个玩家发牌
        //开局发牌数
        var OpenDealCardCount = 5;
        foreach (var player in players)
        {
            for (int i = 0; i < OpenDealCardCount; i++)
            {
                PlayerDrawCard(player);
            }
        }

        //打印卡池中所剩的牌
        foreach (var card in Singleton<CardManager>.Inst.Deck)
        {
            Debug.Log($"Cards remain in deck: {card.color} {card.type} \n");
        }

        ChangeState(GameState.PlayState);
    }

    /// <summary>
    /// 玩家抽卡
    /// </summary>
    /// <param name="player"></param>
    private void PlayerDrawCard(Player player)
    {
        if (Singleton<CardManager>.Inst.Deck.Count == 0)
        {
            Debug.Log($"[抽牌] 牌库为空! 玩家: {player.id} \n");
            return;
        }

        //抽取卡池最上面一张牌
        var card = Singleton<CardManager>.Inst.Deck[0];
        //发给玩家
        player.AddCardToHandDeck(card);
        //从卡池移除
        Singleton<CardManager>.Inst.Deck.RemoveAt(0);
        Debug.Log($"[抽牌] 玩家: {player.id} 获得牌: {card.color} {card.type} \n");
    }

    /// <summary>
    /// 玩家出牌
    /// </summary>
    private void PlayCard()
    {
        if (!isCurPlayerTurnOver) return;

        //所有玩家都没有手牌则切换到结算状态
        if (players.Count == 0)
        {
            ChangeState(GameState.CountState);
            return;
        }

        //显示公共牌区
        ShowPublicDeck();

        //玩家抽牌
        PlayerDrawCard(curPlayer);

        Debug.Log($"[新回合] 当前到玩家:{curPlayer.id} 出牌! \n");

        //显示玩家手牌
        curPlayer.ShowHandDeck();
        //显示玩家吃牌
        curPlayer.ShowConsumeDeck();

        isCurPlayerTurnOver = false;
    }

    /// <summary>
    /// 玩家选牌
    /// </summary>
    public void PlayerSelectCard()
    {
        bool isSelectSuccess = false;

        //从文本框读取玩家想出第几张牌的字符串
        var cardsString = playCardInputField.GetComponent<TMP_InputField>().text;
        if (cardsString == null)
        {
            Debug.Log($"[选牌] 玩家:{curPlayer.id} 请选择正确的玩家手牌! \n");
            return;
        }

        //拆分字符串
        var playCardsSplit = cardsString.Split(',');

        List<int> selectCardIdx = new List<int>();
        //将第几张转换为索引
        foreach (var cardNum in playCardsSplit)
        {
            var index = Convert.ToInt32(cardNum) - 1;
            selectCardIdx.Add(index);

            if (index < 0 || index >= curPlayer.handDeck.Count)
            {
                Debug.Log($"[选牌] 玩家:{curPlayer.id} 请选择正确的玩家手牌! \n");
                return;
            }
        }

        //当前要打出的所有牌
        List<Card> curPlayCards = new List<Card>();
        //判断选择的所有牌是否大小相同
        foreach (var idx in selectCardIdx)
        {
            var firstSelectCard = curPlayer.handDeck[selectCardIdx[0]];
            var curPlayCard = curPlayer.handDeck[idx];
            if (curPlayCard.type != firstSelectCard.type)
            {
                Debug.Log($"[选牌] 玩家:{curPlayer.id} 选择的牌必须是相同大小! 首张牌:{firstSelectCard.type} 不同牌:{curPlayCard.type}\n");
                return;
            }
            curPlayCards.Add(curPlayCard);
        }

        //将所选牌与公共牌进行比较
        var playCard = curPlayCards[0];
        //公共牌为[无牌]
        if (publicDeck.Count == 0)
        {
            foreach (var card in curPlayCards)
            {
                publicDeck.Add(card);
                curPlayer.handDeck.RemoveAt(curPlayer.handDeck.IndexOf(card));
                Debug.Log($"[选牌] 玩家:{curPlayer.id} 公共牌区为空! 当前打出牌: {card.type} \n");
            }
            lastPublicCardCount = curPlayCards.Count;
            isSelectSuccess = true;
        }
        else
        {
            //当前公共牌
            var curpublicCard = publicDeck[publicDeck.Count - 1];
            var curpublicCardType = curpublicCard.type;

            //如果非大小王 打出牌数必须与公共牌数相同
            if (!CheckCardTypeEqual(playCard.type, CardType.Card_Joker)
                && lastPublicCardCount != curPlayCards.Count)
            {
                Debug.Log($"[选牌] 玩家:{curPlayer.id} 打出牌数必须与公共牌数相同! 选牌:{curPlayCards[0].type} 公共牌:{curpublicCard.type} 选牌数:{curPlayCards.Count} 公共牌数:{lastPublicCardCount}\n");
                return;
            }

            //打出牌为[跳牌]
            if (CheckCardTypeEqual(playCard.type, CardType.Card_Jump))
            {
                isSelectSuccess = true;
            }
            //打出牌不是[跳牌]
            else
            {
                //公共牌为[跳牌]
                if (CheckCardTypeEqual(curpublicCardType, CardType.Card_Jump))
                {
                    var isFindNoneJumpCard = false;
                    //如果是跳牌则 往之前的牌方向寻找非跳牌的牌
                    for (int i = publicDeck.Count - 2; i >= 0; i--)
                    {
                        //找到公共牌为非跳牌
                        var card = publicDeck[i];
                        if (!CheckCardTypeEqual(card.type, CardType.Card_Jump))
                        {
                            publicpPreJumpCard = card;

                            Debug.Log($"[公共牌] 玩家:{curPlayer.id} 找到非跳牌的公共牌! 非跳牌的公共牌:{card.type} 原公共牌:{curpublicCard.type} \n");
                            //替代当前公共牌
                            curpublicCard = card;
                            curpublicCardType = card.type;

                            isFindNoneJumpCard = true;

                            break;
                        }
                    }
                    //如果没找到非跳牌的牌则可直接出牌
                    if (!isFindNoneJumpCard)
                    {
                        isSelectSuccess = true;
                    }
                }
                //公共牌为[妖怪牌]
                if (CheckCardTypeEqual(curpublicCardType, CardType.Card_Evil))
                {
                    //TODO:如果当前选择的牌是妖怪牌且更大 或者 徒弟牌
                    if (CheckCardTypeEqual(playCard.type, CardType.Card_Evil) && playCard.type > curpublicCard.type
                        || CheckCardTypeEqual(playCard.type, CardType.Card_Apprentice))
                    {
                        isSelectSuccess = true;
                    }
                }
                //公共牌为[徒弟牌]
                else if (CheckCardTypeEqual(curpublicCardType, CardType.Card_Apprentice))
                {
                    //TODO:如果当前选择的牌是徒弟牌且更大 或者 师傅牌
                    if (CheckCardTypeEqual(playCard.type, CardType.Card_Apprentice) && playCard.type > curpublicCard.type
                            || CheckCardTypeEqual(playCard.type, CardType.Card_Master))
                    {
                        isSelectSuccess = true;
                    }
                }
                //公共牌为[师傅牌]
                else if (CheckCardTypeEqual(curpublicCardType, CardType.Card_Master))
                {
                    //如果当前选的牌为妖怪牌
                    if (CheckCardTypeEqual(playCard.type, CardType.Card_Evil))
                    {
                        isSelectSuccess = true;
                    }
                }
            }

            //判断是否出牌成功
            if (isSelectSuccess)
            {
                foreach (var card in curPlayCards)
                {
                    publicDeck.Add(card);
                    curPlayer.handDeck.RemoveAt(curPlayer.handDeck.IndexOf(card));
                    Debug.Log($"[选牌] 玩家:{curPlayer.id} 打出牌: {card.type} 公共牌:{curpublicCard.type} \n");
                }
                //非跳牌则更新公共牌的单次打出牌数
                if (!CheckCardTypeEqual(playCard.type, CardType.Card_Jump))
                {
                    lastPublicCardCount = curPlayCards.Count;
                }
            }
            else
            {
                Debug.Log($"[选牌] 玩家:{curPlayer.id} 选的牌压不过! 当前牌:{playCard.type} 公共牌:{curpublicCard.type} \n");
            }
        }

        //如果出牌成功切换下一名玩家为当前出牌玩家
        if (isSelectSuccess)
        {
            //检测此玩家是否应该退出比赛
            CheckPlayerShouldQuit(curPlayer);

            ChangeTurnToNextPlayer();
        }
    }

    /// <summary>
    /// 检测此玩家是否应该退出比赛
    /// </summary>
    /// <param name="player"></param>
    private void CheckPlayerShouldQuit(Player player)
    {
        //如果是最后一名玩家则吃掉公共区牌和自己的手牌
        if (players.Count == 1)
        {
            //吃手牌
            foreach (var handCard in player.handDeck)
            {
                player.consumeDeck.Add(handCard);
            }
            //吃公共区牌
            foreach (var card in publicDeck)
            {
                player.consumeDeck.Add(card);
            }
            player.handDeck.Clear();
            publicDeck.Clear();
        }

        //TODO:当前玩家没有手牌则让其离开游戏并记录名次
        if (player.handDeck.Count == 0)
        {
            rank.Add(player);
            players.RemoveAt(players.IndexOf(player));

            Debug.Log($"[玩家退出] 玩家:{player.id} 没有手牌了 退出比赛! \n");
        }
        
    }

    /// <summary>
    /// 切换到下一名玩家的回合
    /// </summary>
    private void ChangeTurnToNextPlayer()
    {
        if (players.Count > 0)
        {
            var playerId = curPlayerId;
            if (playerId + 1 <= players.Count)
            {
                playerId++;
            }
            else
            {
                playerId = 1;
            }
            var player = players[playerId - 1];

            curPlayer = player;
            curPlayerId = playerId;
        }
        
        isCurPlayerTurnOver = true;
    }

    /// <summary>
    /// 玩家吃牌
    /// </summary>
    private void PlayerConsumeCard()
    {
        if (publicDeck.Count == 0)
        {
            Debug.Log($"[吃牌] 玩家:{curPlayer.id} 公共牌区为空! \n");
            return;
        }

        //将公共牌区的所有牌移动到当前玩家的吃牌区
        for (int i = 0; i < publicDeck.Count; i++)
        {
            var card = publicDeck[i];
            curPlayer.AddCardToConsumeDeck(card);
            Debug.Log($"[吃牌] 玩家:{curPlayer.id} 吃牌:{card.color} {card.type} \n");
        }
        publicDeck.Clear();

        //之后从当前玩家开始 每名玩家抽一张牌
        for (int i = curPlayer.id - 1; i < players.Count; i++)
        {
            var player = players[i];
            PlayerDrawCard(player);
        }
        for (int i = 0; i < curPlayer.id - 1; i++)
        {
            var player = players[i];
            PlayerDrawCard(player);
        }
        Debug.Log($"[吃牌] 玩家:{curPlayer.id} 吃牌成功，请任意出牌！\n");
    }

    /// <summary>
    /// 计算排名
    /// </summary>
    private void CalculateRank()
    {
        //根据吃牌数排名[冒泡排序]
        for (int i = 0; i < rank.Count; i++)
        {
            var player1 = rank[i];
            for (int j = i; j < rank.Count; j++)
            {
                var player2 = rank[j];

                if(player1.consumeDeck.Count > player2.consumeDeck.Count)
                {
                    var tempPlayer = player1;
                    player1 = player2;
                    player2 = player1;
                }
                
            }
        }

        for (int i = 0; i < rank.Count; i++)
        {
            var player = rank[i];
            Debug.Log($"[排名] 玩家:{player.id} 排名:{i + 1} 吃牌:{player.consumeDeck.Count}");
        }
        ChangeState(GameState.EndState);
    }

    /// <summary>
    /// 游戏状态切换
    /// </summary>
    /// <param name="state"></param>
    private void ChangeState(GameState state)
    {
        Debug.Log($"[游戏状态切换] from {gameState} to {state} \n");
        gameState = state;
    }

    /// <summary>
    /// 检测卡牌类型是否相同
    /// </summary>
    /// <param name="cardA"></param>
    /// <param name="cardB"></param>
    /// <returns></returns>
    private bool CheckCardTypeEqual(CardType cardA, CardType cardB)
    {
        //如果 位与& 为0的话说明不在一个集合中
        //0111 & 1000 = 0000 = 0
        //0111 & 100 = 0100 != 0
        var isEqual = (cardA & cardB) != 0;
        return isEqual;
    }

    /// <summary>
    /// 检测卡牌属于哪种类型
    /// </summary>
    /// <returns></returns>
    private CardType CheckCardTypeBelongsTo(CardType card)
    {
        var cardType = CardType.Card_Evil;
        if (CheckCardTypeEqual(card, CardType.Card_Evil))
        {
            cardType = CardType.Card_Evil;
        }
        else if (CheckCardTypeEqual(card, CardType.Card_Apprentice))
        {
            cardType = CardType.Card_Apprentice;
        }
        else if (CheckCardTypeEqual(card, CardType.Card_Master))
        {
            cardType = CardType.Card_Master;
        }
        else if (CheckCardTypeEqual(card, CardType.Card_Jump))
        {
            cardType = CardType.Card_Jump;
        }
        return cardType;
    }

    /// <summary>
    /// 显示公共牌区的所有牌
    /// </summary>
    private void ShowPublicDeck()
    {
        if (publicDeck.Count == 0)
        {
            Debug.Log($"[选牌] 玩家:{curPlayer.id} 公共牌区为空! \n");
            return;
        }
        foreach (var card in publicDeck)
        {
            Debug.Log($"[公共牌] 玩家:{curPlayer.id} 公共牌: {card.color} {card.type} \n");
        }
    }
}
