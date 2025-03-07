using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


#if LOGIC_CONSOLE_TEST
#else
using UnityEngine;
#endif

namespace PokerH
{
    public class Dealer
    {

        UserManager _userManager = null;
        PlayerManager _playerManager = new();
        Rounder _rounder = new Rounder();
        Player _currTurnPlayer = null;

        GameRoomInfo _roomInfo;

        enum GameSequence { GS_NONE, GS_PREPARE, GS_PLAYER_SETTING, GS_GAME_START, GS_LOOP_START, GS_SHUFFLE, GS_DEAL_13, GS_ROUNDING, GS_RESULT, GS_CLEANUP, GS_ADMOBTIME, GS_RECHARGE_COIN, GS_CLEANUP_IMMEDI, GS_CLEANUP_LATE };
        enum RoundSequence { RS_NONE, RS_START, RS_LOOP_START, RS_TURN_START, RS_TAKE_2, RS_FIRST_CARD_DROP, RS_FIRST_CARD_DROPPING, RS_SECOND_CARD_DROP, RS_SECOND_CARD_DROPPING, RS_DAMAGE, RS_TURN_END, RS_WIN_CHECK, RS_ROUND_END_NOTHING }

        StatusTimer<GameSequence> _currGameStatus = new StatusTimer<GameSequence>();
        StatusTimer<RoundSequence> _currRoundStatus = new StatusTimer<RoundSequence>();

        List<int> _decks = null;
        int _deckIndex = 0;

        float DEAL_13_TIMER = 2.0f;
        float CARD_THROW_TIMER = 2.0f;
        int _turnCount;

        public void Init(UserManager users)
        {
            _userManager = users;

            _playerManager.Init();
            _rounder.Init();

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
            //OnEnterGameSequence(GameSequence.GS_PREPARE);
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
                        //_OnPrepare?.Invoke();

                        _currGameStatus.SetNewStatus(GameSequence.GS_PLAYER_SETTING);
                    }
                    break;
                case GameSequence.GS_PLAYER_SETTING:
                    {
                        //DoPlayerSetting(_playerManager);
                        bool userDirty = true;
                        if (userDirty)
                            _playerManager.ReconnectPlayers(_userManager);


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
                    _currGameStatus._accumTime += tick;
                    if (_currGameStatus._accumTime > DEAL_13_TIMER)
                    {
                        _currGameStatus.SetNewStatus(GameSequence.GS_ROUNDING);
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_START);
                    }
                    break;

                case GameSequence.GS_ROUNDING:
                    UpdateRound(tick);

                    break;
                case GameSequence.GS_RESULT:
                    _currGameStatus._accumTime += tick;

                    //bool allAi = _seatManager.IsAllAI();
                    //if (allAi && _currGameStatus._accumTime > 10.0f)

                    if (_currGameStatus._accumTime > 10.0f)
                    {
                        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP);
                    }

                    break;
                case GameSequence.GS_CLEANUP:
                    {

#if LOGIC_CONSOLE_TEST
                        _currGameStatus.SetNewStatus(GameSequence.GS_CLEANUP_IMMEDI);
#else
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
                        _currGameStatus.SetNewStatus(GameSequence.GS_LOOP_START);
                    }
                    break;
                case GameSequence.GS_CLEANUP_LATE:
                    {
                        _currGameStatus._accumTime += tick;

                        if (_currGameStatus._accumTime > 0.6f)
                        {
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
            //enum RoundSequence { RS_NONE, RS_START, RS_LOOP_START, RS_TURN_START, RS_TAKE_2,
            /// RS_FIRST_CARD_DROP, RS_FIRST_CARD_DROPPING, RS_SECOND_CARD_DROP, RS_SECOND_CARD_DROPPING, 
            /// RS_DAMAGE, RS_TURN_END, RS_WIN_CHECK, RS_ROUND_END_NOTHING }
            switch (_currRoundStatus._currStatus)
            {
                case RoundSequence.RS_START:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_LOOP_START);
                    }
                    break;
                case RoundSequence.RS_LOOP_START:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_START);
                    }
                    break;
                case RoundSequence.RS_TURN_START:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TAKE_2);
                    }
                    break;
                case RoundSequence.RS_TAKE_2:
                    //if (_currTurnPlayer != null)
                    //    _currTurnPlayer.Update(tick);
                    _currRoundStatus.SetNewStatus(RoundSequence.RS_FIRST_CARD_DROP);
                    break;
                case RoundSequence.RS_FIRST_CARD_DROP:
                    {
                        //if (_currTurnPlayer != null)
                        //    _currTurnPlayer.Update(tick);

                        //if (_rounder != null)
                        //    _rounder.FirstCardDrop(tick);
                    }

                    break;

                case RoundSequence.RS_FIRST_CARD_DROPPING:
                    _currRoundStatus._accumTime += tick;
                    if (_currRoundStatus._accumTime > CARD_THROW_TIMER)
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_SECOND_CARD_DROP);
                    }

                    break;
                case RoundSequence.RS_SECOND_CARD_DROP:
                    {
                        //if (_rounder != null)
                        //    _rounder.SecondCardDrop(tick);
                    }

                    break;

                case RoundSequence.RS_SECOND_CARD_DROPPING:
                    _currRoundStatus._accumTime += tick;
                    if (_currRoundStatus._accumTime > CARD_THROW_TIMER)
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_DAMAGE);
                    }

                    break;
                case RoundSequence.RS_DAMAGE:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_TURN_END);
                    }
                    break;

                case RoundSequence.RS_TURN_END:
                    {
                        _currRoundStatus.SetNewStatus(RoundSequence.RS_WIN_CHECK);
                    }
                    break;
                case RoundSequence.RS_WIN_CHECK:
                    _currRoundStatus.SetNewStatus(RoundSequence.RS_LOOP_START);
                    break;
                default:
                    break;

            }
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
                        FileInfo fileInfo = new FileInfo(filePath);
                        string value = "";

                        if (fileInfo.Exists)
                        {
                            _decks.Clear();
                            StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
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

                    // check Deal Miss
                    bool tryagain = true;
                    while (tryagain == true)
                    {
                        for (int i = 0; i < _decks.Count; i++)
                        {
                            int temp = _decks[i];
                            int randomIndex = UnityEngine.Random.Range(0, _decks.Count);
                            _decks[i] = _decks[randomIndex];
                            _decks[randomIndex] = temp;
                        }

                        bool dealmiss = false;
                        for (int i = 0; i < 5; i++)
                        {
                            int point = 0;
                            for (int j = 0; j < 10; j++)
                            {
                                int index = i * 10 + j;
                                int c = _decks[index];
                                if (Card.IsJoker(c))
                                    continue;

                                if (Card.IsPointCard(c))
                                    point++;
                            }

                            if (point == 0)
                            {
                                dealmiss = true;
                                break;
                            }
                        }

                        if (dealmiss == false)
                        {
                            tryagain = false;
                        }
                    }
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
            Console.WriteLine("DeckCount: {0}, TurnCount: {1}", 53 - _deckIndex, _turnCount);
            //Console.Write("History : ");
            //foreach (int tc in _gameOverHistory)
            //{
            //    Console.Write(" {0}", tc);
            //}
            //Console.WriteLine();

            Console.ForegroundColor = defaultColor;

            //_rounder.Render();
            _playerManager.Render();
        }
#endif

    }
}
