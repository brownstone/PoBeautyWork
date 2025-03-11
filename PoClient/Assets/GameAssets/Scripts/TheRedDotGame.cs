using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using PokerH;
using System;
using static UnityEngine.Rendering.GPUSort;

public class TheRedDotGame : MonoBehaviour
{

    public class PlayerInfo
    {
        public int playerId;
        public string nickname;
        public int damage;
    }

    PokerBeautyBase _gameLogic = new PokerBeautyBase();

    int MyPlayerId { get; set; } = 1001;
    PlayerInfo[] _playerInfos = new PlayerInfo[2] { new PlayerInfo(), new PlayerInfo() };

    CardController CardCont { get; set; }
    BottomPlayerController BottomPlayerCont { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardCont = FindFirstObjectByType<CardController>();
        BottomPlayerCont = FindFirstObjectByType<BottomPlayerController>();

        _gameLogic.Init();
        _gameLogic.AddFakeUser(MyPlayerId);

        // bind actions
        _gameLogic._dealer._OnPrepare += OnEventPrepare;
        _gameLogic._dealer._OnPlayerSetting += OnEventPlayerSetting;
        _gameLogic._dealer._OnGameReStart += OnEventGameReStart;
        _gameLogic._dealer._OnDealCards += OnEventDealCards;
        _gameLogic._dealer._OnTakeCards += OnEventTakeCards;
        _gameLogic._dealer._OnTurnStart += OnEventTurnStart;
        _gameLogic._dealer._OnTurnEnd += OnEventTurnEnd;
        _gameLogic._dealer._OnPlaceCard += OnEventPlaceCard;
        _gameLogic._dealer._OnAttackDamage += OnEventAttackDamage;
        _gameLogic._dealer._OnPass += OnEventPass;
        _gameLogic._dealer._OnGameResult += OnEventGameResult;
        _gameLogic._dealer._OnCleanUp += OnEventCleanUp;


        //public Action _OnPrepare;
        //public Action<PlayerManager> _OnPlayerSetting;
        //public Action _OnGameReStart;
        //public Action<int, List<int>> _OnDealCards;
        //public Action<int, List<int>> _OnTakeCards;
        //public Action<int> _OnTurnStart;
        //public Action<int> _OnTurnEnd;
        //public Action<int, int, int> _OnPlaceCard;
        //public Action<int> _OnGameResult;
        //public Action _OnCleanUp;

        // Start Game !!!!
        _gameLogic.StartGame();
    }

    void Update()
    {
        _gameLogic.UpdateState(Time.deltaTime);
    }


    void OnEventPrepare()
    {

    }
    void OnEventPlayerSetting(PlayerManager players)
    {
        int myIndex = Array.FindIndex(players._players, e => e._playerId == MyPlayerId);
        if (myIndex < 0)
            myIndex = 0;

        for (int i = 0; i < players._players.Length; i++)
        {
            int baseIndex = (i + myIndex) % players._players.Length;
            Player p = players._players[baseIndex];
            _playerInfos[i].nickname = p._nickname;
            _playerInfos[i].playerId = p._playerId;
        }
    }

    int GetPlayerIndex(int playerId)
    {
        int playerIndex = Array.FindIndex(_playerInfos, e => e.playerId == playerId);
        if (playerIndex < 0)
        {
            Debug.Log($"Check playerid {playerId}");
            Debug.Break();
        }
        return playerIndex;
    }
    void OnEventGameReStart()
    {

    }
    void OnEventDealCards(int notusevalue, List<int> cards)
    {
        CardCont.OnDeal9Cards(cards);
    }
    void OnEventTakeCards(int playerId, List<int> cards)
    {
        int playerIndex = GetPlayerIndex(playerId);
        CardCont.OnTakeCards(playerId, playerIndex, cards);
    }
    void OnEventTurnStart(int playerId)
    {
        BottomPlayerCont.OnTurnStart();

    }
    void OnEventTurnEnd(int playerId)
    {

    }
    void OnEventPlaceCard(int playerId, int y, int x, int card)
    {
        int playerIndex = GetPlayerIndex(playerId);
        CardCont.OnPlaceCard(playerId, playerIndex, y, x, card);
    }
    void OnEventAttackDamage(int playerId, int rank)
    {
        int playerIndex = GetPlayerIndex(playerId);
        BottomPlayerCont.OnAttackDamage(playerId, playerIndex, rank);
    }
    void OnEventPass(int playerId)
    {
        int playerIndex = GetPlayerIndex(playerId);
        CardCont.OnPass(playerId, playerIndex);
    }
    void OnEventGameResult(int notuse)
    {

    }

    void OnEventCleanUp()
    {
        CardCont.OnCleanUp();
        BottomPlayerCont.OnCleanUp();
    }

}
