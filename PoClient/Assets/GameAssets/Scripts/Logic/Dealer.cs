using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


#if LOGIC_CONSOLE_TEST
#else
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;
#endif

namespace PokerH
{
    public class Dealer
    {
        UserManager _userManager = null;
        PlayerManager _playerManager = new();
        Boarder _boarder = new Boarder();
        Rounder _rounder = new Rounder();
        Player _currTurnPlayer = null;

        GameRoomInfo _roomInfo;

        enum GameSequence { GS_NONE, GS_PREPARE, GS_PLAYER_SETTING, GS_GAME_START, GS_LOOP_START, GS_SHUFFLE, GS_DEAL_13, GS_DEALING_13, GS_ROUNDING, GS_RESULT, GS_CLEANUP, GS_ADMOBTIME, GS_RECHARGE_COIN, GS_CLEANUP_IMMEDI, GS_CLEANUP_LATE };
        enum RoundSequence { RS_NONE, RS_START, RS_LOOP_START, RS_TURN_START, RS_TAKE_CARDS, RS_THINK, RS_PASS, RS_PASSING, RS_FIRST_CARD_PLACE, RS_FIRST_CARD_PLACING, RS_SECOND_CARD_PLACE, RS_SECOND_CARD_PLACING, RS_DAMAGE, RS_TURN_END, RS_WIN_CHECK, RS_ROUND_END_NOTHING }

        StatusTimer<GameSequence> _currGameStatus = new StatusTimer<GameSequence>();
        StatusTimer<RoundSequence> _currRoundStatus = new StatusTimer<RoundSequence>();

        List<int> _decks = null;
        int _deckIndex = 0;

        float DEAL_13_TIMER = 2.0f;
        float CARD_THROW_TIMER = 1.0f;
        int _turnCount;

        public Action _OnPrepare;
        public Action<PlayerManager> _OnPlayerSetting;
        public Action _OnGameReStart;
        public Action<int, List<int>> _OnDealCards;
        public Action<int, List<int>> _OnTakeCards;
        public Action<int> _OnTurnStart;
        public Action<int> _OnTurnEnd;
        public Action<int, int, int, int> _OnPlaceCard;
        public Action<int, int> _OnAttackDamage;
        public Action<int> _OnPass;
        public Action<int> _OnGameResult;
        public Action _OnCleanUp;



        public void Init(UserManager users)
        {
            _userManager = users;

            _playerManager.Init();
            _rounder.Init(_boarder);
            //_boarder.Init();
        }

        public void SetRoomInfo(GameRoomInfo roomInfo)
        {
            _roomInfo = roomInfo;

            //PREPARE_TIMER = 0.4f;
            //DEAL10_TIMER = 3.0f;
            //TAKE3_END_TIMER = 1.5f;
            //DROP3_TIMER = 2.5f;
            //VID_VIEWING_TIMER = 0.7f;
            //TRICK_CARD_THROWING_TIMER = 0.4f;
            //TRICK_WIN_CHECK_TIMER = 0.8f;
            //TRICK_TAKING_TIMER = 1.0f;

            //if (_roomInfo.tempo == 1)
            //{
            //    PREPARE_TIMER = 0.8f;
            //    DEAL10_TIMER = 3.8f;
            //    TAKE3_END_TIMER = 2.0f;
            //    DROP3_TIMER = 3.5f;
            //    VID_VIEWING_TIMER = 1.0f;
            //    TRICK_CARD_THROWING_TIMER = 1.0f;
            //    TRICK_WIN_CHECK_TIMER = 1.5f;
            //    TRICK_TAKING_TIMER = 1.5f;
            //}
            //else if (_roomInfo.tempo == 2)
            //{
            //    PREPARE_TIMER = 0.6f;
            //    DEAL10_TIMER = 3.5f;
            //    TAKE3_END_TIMER = 1.8f;
            //    DROP3_TIMER = 3.0f;
            //    VID_VIEWING_TIMER = 0.8f;
            //    TRICK_CARD_THROWING_TIMER = 0.6f;
            //    TRICK_WIN_CHECK_TIMER = 1.0f;
            //    TRICK_TAKING_TIMER = 1.2f;
            //}
        }

        public void StartGame()
        {
            _currGameStatus.SetNewStatus(GameSequence.GS_PREPARE);
        }

        public void Update(float tick)
        {
            switch (_currGameStatus._currStatus)
            {
                case GameSequence.GS_PREPARE:
                    {
                        //DoPrepare();
                        InitDeck();
                        _OnPrepare?.Invoke();

                        _currGameStatus.SetNewStatus(GameSequence.GS_PLAYER_SETTING);
                    }
                    break;
                case GameSequence.GS_PLAYER_SETTING:
                    {
                        bool userDirty = true;
                        if (userDirty)
                            _playerManager.ReconnectPlayers(_userManager);

                        _OnPlayerSetting?.Invoke(_playerManager);
                        //DoPlayerSetting(_playerManager);


                        _currGameStatus.SetNewStatus(GameSequence.GS_GAME_START);

                    }
                    break;
                case GameSequence.GS_GAME_START:
                    //DoGameStart();
                    _currGameStatus.SetNewStatus(GameSequence.GS_LOOP_START);
                    break;
                case GameSequence.GS_LOOP_START:
                    //DoLoopStart();
                    _currGameStatus.SetNewStatus(GameSequence.GS_SHUFFLE);
                    break;
                    
                case GameSequence.GS_SHUFFLE:
                    {
                        ShuffleDeck();
                        _currGameStatus.SetNewStatus(GameSequence.GS_DEAL_13);
                    }
                    break;

                case GameSequence.GS_DEAL_13:
                    {
                        List<int> cards = _decks.GetRange(_deckIndex, 9);
                        _deckIndex += 9;
                        NetworkSender.Instance.SendDeal9Cards(cards);

                        cards = _decks.GetRange(_deckIndex, 2);
                        _deckIndex += 2;
                        NetworkSender.Instance.SendTakeCards(_playerManager._players[0]._playerId, cards);

                        cards = _decks.GetRange(_deckIndex, 2);
                        _deckIndex += 2;
                        NetworkSender.Instance.SendTakeCards(_playerManager._players[1]._playerId, cards);

                        _currGameStatus.SetNewStatus(GameSequence.GS_DEALING_13);
                    }
                    break;
                case GameSequence.GS_DEALING_13:
                    {
                        _currGameStatus._accumTime += tick;
                        if (_currGameStatus._accumTime > DEAL_13_TIMER)
                        {
                            _currGameStatus.SetNewStatus(GameSequence.GS_ROUNDING);
                            _currRoundStatus.SetNewStatus(RoundSequence.RS_START);
                        }
                    }
                    break;

                case GameSequence.GS_ROUNDING:
                    UpdateRound(tick);

                    break;
                case GameSequence.GS_RESULT:
                    _currGameStatus._accumTime += tick;

                    //bool allAi = _seatManager.IsAllAI();
                    //if (allAi && _currGameStatus._accumTime > 10.0f)

                    if (_currGameStatus._accumTime > 5.0f)
                    {
                        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP);
                    }

                    break;
                case GameSequence.GS_CLEANUP:
                    {

#if LOGIC_CONSOLE_TEST
                        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP_IMMEDI);
#else
                        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP_LATE);
                        //if (_adType == 1)
                        //{
                        //    bool succed = Managers.Admob.RequestInterstiAd((a) => { OnEventAdsClose(a); });
                        //    if (succed)
                        //    {
                        //        OnEnterGameSequence(GameSequence.GS_ADMOBTIME);
                        //        _currGameStatus.SetNewStatus(GameSequence.GS_ADMOBTIME);
                        //    }
                        //    else
                        //    {
                        //        OnEnterGameSequence(GameSequence.GS_CLEANUP_LATE);
                        //        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP_LATE);
                        //    }
                        //}
                        //else
                        //{
                        //    OnEnterGameSequence(GameSequence.GS_CLEANUP_LATE);
                        //    _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP_LATE);
                        //}
                        //_adType = 0;
#endif
                    }
                    break;
                case GameSequence.GS_ADMOBTIME:

                    break;
                case GameSequence.GS_RECHARGE_COIN:

                    break;
                case GameSequence.GS_CLEANUP_IMMEDI:
                    {
                        DoCleanUp();
                        _currGameStatus.SetNewStatus(GameSequence.GS_LOOP_START);
                    }
                    break;
                case GameSequence.GS_CLEANUP_LATE:
                    {
                        _currGameStatus._accumTime += tick;

                        if (_currGameStatus._accumTime > 0.6f)
                        {
                            DoCleanUp();
                            _currGameStatus.SetNewStatus(GameSequence.GS_LOOP_START);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        void UpdateRound(float tick)
        {
            switch (_currRoundStatus._currStatus)
            {
                case RoundSequence.RS_START:
                    {
                        _currTurnPlayer = _playerManager.GetFirstTurnPlayer();

                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_START);
                    }
                    break;
                case RoundSequence.RS_LOOP_START:
                    {
                        _currTurnPlayer = _playerManager.GetNextTurner(_currTurnPlayer);

                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_START);
                    }
                    break;
                case RoundSequence.RS_TURN_START:
                    {
                        _OnTurnStart?.Invoke(_currTurnPlayer._playerId);
                        _turnCount++;
                        _rounder.OnTurnStart(_currTurnPlayer);
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TAKE_CARDS);
                    }
                    break;
                case RoundSequence.RS_TAKE_CARDS:
                    {
                        //if (_currTurnPlayer != null)
                        //    _currTurnPlayer.Update(tick);

                        int needCardCount = 4 - _currTurnPlayer._hands.Count;
                        List<int> cards = _decks.GetRange(_deckIndex, needCardCount);
                        _deckIndex += needCardCount;
                        NetworkSender.Instance.SendTakeCards(_currTurnPlayer._playerId, cards);

                        _currRoundStatus.SetNewStatus(RoundSequence.RS_THINK);
                    }
                    break;
                case RoundSequence.RS_THINK:
                    {
                        if (_currTurnPlayer._isAI == false)
                            break;  // waiting...

                        if (_rounder != null)
                        {
                            _rounder.Think(_currTurnPlayer, tick);
                        }
                            

                    }
                    break;
                case RoundSequence.RS_PASSING:
                    _currRoundStatus._accumTime += tick;
                    if (_currRoundStatus._accumTime > CARD_THROW_TIMER)
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_END);
                    }

                    break;
                //case RoundSequence.RS_FIRST_CARD_PLACE:
                //    {
                //        //if (_currTurnPlayer != null)
                //        //    _currTurnPlayer.Update(tick);

                //        if (_currTurnPlayer._isAI == false)
                //            break;  // waiting...

                //        if (_rounder != null)
                //            _rounder.FirstCardThrow(_currTurnPlayer, tick);

                //        // pass
                //    }

                //    break;

                case RoundSequence.RS_FIRST_CARD_PLACING:
                    _currRoundStatus._accumTime += tick;
                    if (_currRoundStatus._accumTime > (CARD_THROW_TIMER * 0.5f))
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_SECOND_CARD_PLACE);
                    }
                    break;
                case RoundSequence.RS_SECOND_CARD_PLACE:
                    {
                        if (_currTurnPlayer._isAI == false)
                            break;  // waiting...

                        if (_rounder != null)
                            _rounder.SecondCardChoice(_currTurnPlayer, tick);
                    }

                    break;

                case RoundSequence.RS_SECOND_CARD_PLACING:
                    _currRoundStatus._accumTime += tick;
                    if (_currRoundStatus._accumTime > (CARD_THROW_TIMER * 2))
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_DAMAGE);
                    }

                    break;
                case RoundSequence.RS_DAMAGE:
                    {
                        if (_rounder != null)
                            _rounder.SecondCardChoice(_currTurnPlayer, tick);

                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_END);
                    }
                    break;

                case RoundSequence.RS_TURN_END:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_WIN_CHECK);
                    }
                    break;
                case RoundSequence.RS_WIN_CHECK:
                    {
                        if (_turnCount > 7)
                        {
                            _currRoundStatus.SetNewStatus(RoundSequence.RS_NONE);
                            _currGameStatus.SetNewStatus(GameSequence.GS_RESULT);
                        }
                        else
                        {
                            _currRoundStatus.SetNewStatus(RoundSequence.RS_LOOP_START);
                        }
                    }
                    break;
                default:
                    break;

            }
        }

        void DoCleanUp()
        {
            _turnCount = 0;
            _boarder.OnCleanUp();
            _rounder.OnCleanUp();
            _playerManager.OnCleanUp();

            _OnCleanUp?.Invoke();
        }

        private void InitDeck()
        {
            _decks = SequenceDeck();
        }

        private List<int> SequenceDeck()
        {
            const int LINE_1 = 0;
            const int LINE_2 = 1 * 13;
            const int LINE_3 = 2 * 13;
            const int LINE_4 = 3 * 13;
            List<int> newDeck = new List<int> {
                LINE_1 + 1,  LINE_1 + 2,  LINE_1 + 3,  LINE_1 + 4,  LINE_1 + 5,  LINE_1 + 6,  LINE_1 + 7,  LINE_1 + 8,  LINE_1 + 9,  LINE_1 + 10, LINE_1 + 11, LINE_1 + 12, LINE_1 + 13,
                LINE_2 + 1,  LINE_2 + 2,  LINE_2 + 3,  LINE_2 + 4,  LINE_2 + 5,  LINE_2 + 6,  LINE_2 + 7,  LINE_2 + 8,  LINE_2 + 9,  LINE_2 + 10, LINE_2 + 11, LINE_2 + 12, LINE_2 + 13,
                LINE_3 + 1,  LINE_3 + 2,  LINE_3 + 3,  LINE_3 + 4,  LINE_3 + 5,  LINE_3 + 6,  LINE_3 + 7,  LINE_3 + 8,  LINE_3 + 9,  LINE_3 + 10, LINE_3 + 11, LINE_3 + 12, LINE_3 + 13,
                LINE_4 + 1,  LINE_4 + 2,  LINE_4 + 3,  LINE_4 + 4,  LINE_4 + 5,  LINE_4 + 6,  LINE_4 + 7,  LINE_4 + 8,  LINE_4 + 9,  LINE_4 + 10, LINE_4 + 11, LINE_4 + 12, LINE_4 + 13,
                53
            };

            return newDeck;
        }

        private void ShuffleDeck()
        {
            const int FromShuffle = 1;
            const int FromLastSaveData = 2;
            const int FromPracticeLevel = 3;

            int deckMethod = FromShuffle;

            //_overrideBidBeginer = -1;
            //{
            //    deckMethod = FromLastSaveData;
            //    _overrideBidBeginer = _playerManager._players[0]._id;
            //}
            //if (_practiceMode == true)
            //{
            //    _deckMethod = FromPracticeLevel;
            //}

            _deckIndex = 0;

            switch (deckMethod)
            {
                case FromLastSaveData:

#if LOGIC_CONSOLE_TEST
                    {
                        var filePath = "SavedLastDeck.txt";
                        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                        if (lines.Length > 0)
                        {
                            _decks.Clear();

                            foreach (var line in lines)
                            {
                                string[] lineNums = line.Split(',');
                                for (var i = 0; i < lineNums.Count(); i++)
                                {
                                    _decks.Add(int.Parse(lineNums[i]));
                                }
                            }
                        }
                    }
#else
                    {
                        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "SavedLastDeck.txt");
                        //UnityEngine.Debug.Log(filePath);
                        //filePath    "C:/Users/bywon/AppData/LocalLow/trendlab/playrummy\\SavedLastDeck.txt" string

                        //string filePath = Application.dataPath + "/SavedLastDeck.txt";
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                        string value = "";

                        if (fileInfo.Exists)
                        {
                            _decks.Clear();
                            System.IO.StreamReader reader = new System.IO.StreamReader(filePath, Encoding.UTF8);
                            value = reader.ReadToEnd();

                            string[] lineNums = value.Split(',');
                            for (var i = 0; i < lineNums.Count(); i++)
                            {
                                _decks.Add(int.Parse(lineNums[i]));
                            }

                            reader.Close();
                        }
                    }
#endif
                    break;
                case FromShuffle:

#if LOGIC_CONSOLE_TEST
                    System.Random r = new System.Random();

                    for (int i = 0; i < _decks.Count; i++)
                    {
                        int j = r.Next(_decks.Count);
                        if (i == j)
                            continue;

                        int tmp = _decks[i];
                        _decks[i] = _decks[j];
                        _decks[j] = tmp;
                    }
#else

                    for (int i = 0; i < _decks.Count; i++)
                    {
                        int temp = _decks[i];
                        int randomIndex = UnityEngine.Random.Range(0, _decks.Count);
                        _decks[i] = _decks[randomIndex];
                        _decks[randomIndex] = temp;
                    }
                    _deckIndex = 0;
#endif

#if LOGIC_CONSOLE_TEST
                    {
                        string filePath = "SavedLastDeck.txt";
                        TextWriter tw = new StreamWriter(filePath);

                        tw.WriteLine(string.Join(",", _decks));
                        tw.Close();
                    }
#else
                    {
                        // C:/Users/user/AppData/LocalLow/DefaultCompany/Mighty\SavedLastDeck.txt
                        //string filePath = Application.dataPath + "/SavedLastDeck.txt";
                        //string path = Application.persistentDataPath;
                        //path = path.Substring(0, path.LastIndexOf('/'));
                        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "SavedLastDeck.txt");
                        //UnityEngine.Debug.Log(filePath);
                        DirectoryInfo directoryInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePath));

                        if (!directoryInfo.Exists)
                        {
                            directoryInfo.Create();
                        }

                        FileStream fileStream
                            = new FileStream(filePath, System.IO.FileMode.OpenOrCreate, FileAccess.Write);

                        StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.Unicode);

                        writer.WriteLine(string.Join(",", _decks));
                        writer.Close();
                    }
#endif
                    break;

                case FromPracticeLevel:
                    _decks = SequenceDeck();
                    //_decks = PracticeDeck(_roomInfo.practiceLevel);
                    break;

            }

            if (_decks.Count != 53)
            {
#if LOGIC_CONSOLE_TEST
                Console.WriteLine(" ** Invalid deck count : {0} ", _decks.Count);
                Debugger.Break();
#else
                UnityEngine.Debug.Log($" ** Invalid deck count : {_decks.Count} ");
                UnityEngine.Debug.Break();
#endif
            }
        }

#if LOGIC_CONSOLE_TEST
        public void Render()
        {
            var defaultColor = Console.ForegroundColor;

            Console.WriteLine(" ====================== Dealer =========================== ");
            Console.WriteLine("Game Room Status : {0} {1:F2} {2}", _currGameStatus._currStatus, _currGameStatus._accumTime, _currRoundStatus._currStatus);
            int deckMethod = 1;
            if (deckMethod == 1)
            {
                Console.Write("[FromShuffle]  ");
            }
            else if (deckMethod == 2)
            {
                Console.Write("[FromLastSaveData]  ");
            }
            else if (deckMethod == 3)
            {
                Console.Write("[FromTan]  ");
            }
            Console.WriteLine("DeckCount: {0}, TurnCount: {1}/8", 53 - _deckIndex, _turnCount);
            //Console.Write("History : ");
            //foreach (int tc in _gameOverHistory)
            //{
            //    Console.Write(" {0}", tc);
            //}
            //Console.WriteLine();

            Console.ForegroundColor = defaultColor;

            _boarder.Render();
            _rounder.Render();
            _playerManager.Render();
        }
#endif

        public void HandleGameStart(int playerId, int adType)
        {
            //_retryGame = false;
            //_adType = adType;
            _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP);
        }
        public void HandleGameRetry(int playerId, int adType)
        {
            //_retryGame = true;
            //_adType = adType;
            _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP);
        }

        public void HandleDeal9Cards(List<int> cards)
        {
            for (int i = 0; i < 3; i++) 
            {
                for (int j = 0; j < 3; j++)
                {
                    _boarder.PlaceCard(i + 1, j + 1, cards[i * 3 + j]);
                }
            }


            _OnDealCards?.Invoke(9, cards);

            //OnEventDeal9Cards(cards);
        }
        public void HandleTakeCards(int playerId, List<int> cards)
        {
            Player p = _playerManager.GetPlayer(playerId);
            p.AddCards(cards);

            _OnTakeCards?.Invoke(playerId, cards);
            //OnEventDeal2Cards(playerId, cards);
        }
        public void HandlePass(int playerId)
        {
            Player p = _playerManager.GetPlayer(playerId);
            p._hands.Clear();

            _OnPass?.Invoke(playerId);

            _currRoundStatus.SetNewStatus(RoundSequence.RS_PASSING);
            //OnEventDeal2Cards(playerId, cards);
        }
        public void HandleFirstPlace(int playerId, int y, int x, int card)
        {
            _boarder.PlaceCard(y, x, card);
            Player p = _playerManager.GetPlayer(playerId);
            bool success = p._hands.Remove(card);
            if (success == false)
            {
#if LOGIC_CONSOLE_TEST
                Debugger.Break();
#else
                UnityEngine.Debug.Log($"Check HandleFirstPlace playerId {playerId}  card {card}");
                UnityEngine.Debug.Break();
#endif
            }

            _OnPlaceCard?.Invoke(playerId, y, x, card);


            _currRoundStatus.SetNewStatus(RoundSequence.RS_FIRST_CARD_PLACING);
            //OnEventDeal2Cards(playerId, cards);
        }
        public void HandleSecondPlace(int playerId, int y, int x, int card, int rank)
        {
            _boarder.PlaceCard(y, x, card);

            Player p = _playerManager.GetPlayer(playerId);
            bool success = p._hands.Remove(card);
            if (success == false)
            {
#if LOGIC_CONSOLE_TEST
                Debugger.Break();
#else
                UnityEngine.Debug.Log($"Check HandleFirstPlace playerId {playerId}  card {card}");
                UnityEngine.Debug.Break();
#endif
            }

            _OnPlaceCard?.Invoke(playerId, y, x, card);
            _OnAttackDamage?.Invoke(playerId, rank);

            _currRoundStatus.SetNewStatus(RoundSequence.RS_SECOND_CARD_PLACING);
            //OnEventDeal2Cards(playerId, cards);
        }
    }
}
