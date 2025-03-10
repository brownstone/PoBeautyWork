using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class Rounder
    {

        Boarder _boarder = null;
        PoBeautyRule _rule = new PoBeautyRule();

        //BoardPlacer _bp = new BoardPlacer();
        BeautyResult _topBeautyResult = new BeautyResult();
        bool _haveResult = false;
        bool _sendPass = false;
        bool _sendFirstCard = false;
        bool _sendSecondCard = false;

        float _delayTime = 0f;

        public void Init(Boarder boarder)
        {
            _boarder = boarder;
            _rule.Init();
        }

        public void OnTurnStart(Player player)
        {
            _delayTime = 1.0f;
            _topBeautyResult.Clear();
            _haveResult = false;
            _sendPass = false;
            _sendFirstCard = false;
            _sendSecondCard = false;
        }

        public void Think(Player player, float tick)
        {
            if (player._isAI == false)
                return;

            if (_delayTime > 0.0f)
            {
                _delayTime -= tick;
                return;
            }
            _delayTime = 0.3f;
            if (_sendFirstCard)
                return;
            if (_sendPass)
                return;

            _rule.SetUpCandis(_boarder.GetCachedSlot());
            _rule.SetUpBeautys(player);

            _topBeautyResult = _rule.GetTopBeauty();
            _haveResult = true;

            if (_topBeautyResult.rank == 0)
            {
                NetworkSender.Instance.SendPass(player._playerId);
                _sendPass = true;
            }
            else
            {
                int y = GetFirstY(_topBeautyResult.index);
                int x = GetFirstX(_topBeautyResult.index);

                NetworkSender.Instance.SendFirstPlace(player._playerId, y, x, _topBeautyResult.cardFirst);
                _sendFirstCard = true;
            }
        }


        //public void FirstCardThrow(Player player, float tick)
        //{
        //    if (player._isAI == false)
        //        return;

        //    if (_sendFirstCard)
        //        return;

        //    _rule.SetUpCandis(_boarder.GetCachedSlot());
        //    _rule.SetUpBeautys(player);

        //    _topBeautyResult = _rule.GetTopBeauty();
        //    _haveResult = true;

        //    int y = GetFirstY(_topBeautyResult.index);
        //    int x = GetFirstX(_topBeautyResult.index);

        //    NetworkSender.Instance.SendFirstPlace(player._playerId, y, x, _topBeautyResult.cardFirst);
        //    _sendFirstCard = true;
        //}

        public void SecondCardChoice(Player player, float tick)
        {
            if (player._isAI == false)
                return;

            if (_delayTime > 0.0f)
            {
                _delayTime -= tick;
                return;
            }
            if (_sendSecondCard)
                return;


            int y = GetSecondY(_topBeautyResult.index);
            int x = GetSecondX(_topBeautyResult.index);

            NetworkSender.Instance.SendSecondPlace(player._playerId, y, x, _topBeautyResult.cardSecond);
            _sendSecondCard = true;
        }

        public void OnCleanUp()
        {
            _topBeautyResult.Clear();
            _haveResult = false;
            _sendPass = false;
            _sendFirstCard = false;
            _sendSecondCard = false;
        }

        int GetFirstY(int index)
        {
            int y = 0;
            if (index == 0)                y = 0;
            else if (index == 1)           y = 0;
            else if (index == 2)           y = 0;
            else if (index == 3)           y = 0;
            else if (index == 4)           y = 0;
            else if (index == 5)           y = 1;
            else if (index == 6)           y = 2;
            else if (index == 7)           y = 3;

            return y;
        }
        int GetFirstX(int index)
        {
            int x = 0;
            if (index == 0)                 x = 0;
            else if (index == 1)            x = 1;
            else if (index == 2)            x = 2;
            else if (index == 3)            x = 3;
            else if (index == 4)            x = 4;
            else if (index == 5)            x = 4;
            else if (index == 6)            x = 4;
            else if (index == 7)            x = 4;

            return x;
        }
        int GetSecondY(int index)
        {
            int y = 0;
            if (index == 0)                y = 4;
            else if (index == 1)           y = 4;
            else if (index == 2)           y = 4;
            else if (index == 3)           y = 4;
            else if (index == 4)           y = 4;
            else if (index == 5)           y = 1;
            else if (index == 6)           y = 2;
            else if (index == 7)           y = 3;

            return y;
        }
        int GetSecondX(int index)
        {
            int x = 0;
            if (index == 0)                x = 4;
            else if (index == 1)           x = 1;
            else if (index == 2)           x = 2;
            else if (index == 3)           x = 3;
            else if (index == 4)           x = 0;
            else if (index == 5)           x = 0;
            else if (index == 6)           x = 0;
            else if (index == 7)           x = 0;

            return x;
        }

#if LOGIC_CONSOLE_TEST
        public void Render()
        {
            var defaultColor = Console.ForegroundColor;

            if (_haveResult)
            {
                string[] ranks = { "High Card", "One Pair", "Two Pairs", "Triful", "Straight", "Flush", "Full House", "Four Card", "Straight Flush", "Royal Flush" };
                if (_topBeautyResult.rank == 0)
                {
                    Console.WriteLine("   Damage PASS ");
                }
                else
                    Console.WriteLine($"   Damage {ranks[_topBeautyResult.rank]}");
            }
            else
            {
                Console.WriteLine();
            }

            Console.ForegroundColor = defaultColor;
        }
#endif


    }

    public class PoBeautyRule
    {

        static readonly int CHECK_COUNT = 8;

        int[] _checkPos = { Boarder.PackYX(0, 0), Boarder.PackYX(0, 1), Boarder.PackYX(0, 2), Boarder.PackYX(0, 3), Boarder.PackYX(0, 4), Boarder.PackYX(1, 0), Boarder.PackYX(2, 0), Boarder.PackYX(3, 0) };
        int[][] _candisPos = new int[CHECK_COUNT][];

        List<int>[] _candis = new List<int>[CHECK_COUNT];
        BeautyResult[] _beautyResults = new BeautyResult[CHECK_COUNT];
        BeautyResult _topBeauty = new BeautyResult();

        List<int> _cards = new List<int>(5);

        // thinking
        int[,] _mm = new int[5, 15]; // 1 base   A, 2, ... K, A
                                     // 0, A, 2, 3, 4, 5, 6, 7, 8, 9, T, J, Q, K, A
                                     // 0, 1, 2,                        11,12,13,14 

        public void Init()
        {
            for (int i = 0; i < _candis.Length; i++)
            {
                _candis[i] = new List<int>();
            }

            //  
            //   0       123        4      555
            //    0      123       4       666
            //     0     123      4        777
            //
            for (int i = 0; i < _candisPos.Length; i++)
            {
                if (i == 0)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 1), Boarder.PackYX(2, 2), Boarder.PackYX(3, 3) };
                else if (i == 1)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 1), Boarder.PackYX(2, 1), Boarder.PackYX(3, 1) };
                else if (i == 2)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 2), Boarder.PackYX(2, 2), Boarder.PackYX(3, 2) };
                else if (i == 3)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 3), Boarder.PackYX(2, 3), Boarder.PackYX(3, 3) };
                else if (i == 4)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 3), Boarder.PackYX(2, 2), Boarder.PackYX(3, 1) };
                else if (i == 5)
                    _candisPos[i] = new int[] { Boarder.PackYX(1, 1), Boarder.PackYX(1, 2), Boarder.PackYX(1, 3) };
                else if (i == 6)
                    _candisPos[i] = new int[] { Boarder.PackYX(2, 1), Boarder.PackYX(2, 2), Boarder.PackYX(2, 3) };
                else if (i == 7)
                    _candisPos[i] = new int[] { Boarder.PackYX(3, 1), Boarder.PackYX(3, 2), Boarder.PackYX(3, 3) };
                else
                {

                }
            }
        }

        public void SetUpCandis(int[,] boarderSlots)
        {
            for (int i = 0; i < CHECK_COUNT; i++)
            {
                _candis[i].Clear();
            }

            for (int i = 0; i < CHECK_COUNT; i++)
            {
                int check_y = Boarder.UnpackY(_checkPos[i]);
                int check_x = Boarder.UnpackX(_checkPos[i]);
                if (boarderSlots[check_y, check_x] > 0)
                    continue;

                {
                    for (int j = 0; j < _candisPos[0].Length; j++)
                    {
                        int y = Boarder.UnpackY(_candisPos[i][j]);
                        int x = Boarder.UnpackX(_candisPos[i][j]);
                        int c = boarderSlots[y, x];
                        _candis[i].Add(c);
                    }
                }
            }
        }

        public void SetUpBeautys(Player player)
        {
            for (int i = 0; i < CHECK_COUNT; i++)
            {
                _beautyResults[i].Clear();


                if (_candis[i].Count == 0)
                    continue;

                BeautyResult topRank = new BeautyResult();
                topRank.Clear();


                for (int j = 0; j < 4; j++)
                {
                    for (int k = j + 1; k < 4; k++)
                    {
                        _cards.Clear();

                        int firstHandCard = player._hands[j];
                        int secondHandCard = player._hands[k];

                        _cards.Add(_candis[i][0]);
                        _cards.Add(_candis[i][1]);
                        _cards.Add(_candis[i][2]);
                        _cards.Add(firstHandCard);
                        _cards.Add(secondHandCard);

                        int rank = GetRank(_cards);

                        if (rank > topRank.rank)
                        {
                            topRank.index = i;
                            topRank.rank = rank;
                            topRank.cardFirst = firstHandCard;
                            topRank.cardSecond = secondHandCard;
                        }
                    }
                }
                _beautyResults[i] = topRank;
            }

            _topBeauty.Clear();
            bool haveRank = false;
            for (int i = 0; i < CHECK_COUNT; i++)
            {
                if (_beautyResults[i].rank > _topBeauty.rank)
                {
                    haveRank = true;
                    _topBeauty = _beautyResults[i];
                }
            }

            if (haveRank == false)
            {

            }
        }

        public BeautyResult GetTopBeauty()
        {
            return _topBeauty;
        }

        int GetRank(List<int> cards)
        {
            int jokerCount = 0;
            // cards to mm
            Array.Clear(_mm, 0, _mm.Length);
            foreach (int card in cards)
            {
                if (Card.IsJoker(card))
                {
                    jokerCount++;
                    continue;
                }

                int suit = Card.GetCardSuit(card);
                int num = Card.GetCardNum(card);

                int newnum = num + 1; // with Ace. 1 base.
                _mm[suit, newnum] = card;

                if (num == Card.AceNum)
                {
                    _mm[suit, 1] = card;
                }
            }

            bool royalFlush = IsRoyalFlush(jokerCount);
            if (royalFlush)
                return 9;

            bool straightFlush = IsStraightFlush(jokerCount);
            if (straightFlush)
                return 8;
            bool fourCard = IsFourCard(jokerCount);
            if (fourCard)
                return 7;

            bool fullHouse = IsFullHouse(jokerCount);
            if (fullHouse)
                return 6;
            bool flush = IsFlush(jokerCount);
            if (flush)
                return 5;
            bool straight = IsStraight(jokerCount);
            if (straight)
                return 4;
            bool triful = IsTriful(jokerCount);
            if (triful)
                return 3;
            bool twoPairs = IsTwoPairs(jokerCount);
            if (twoPairs)
                return 2;
            bool onePair = IsOnePair(jokerCount);
            if (onePair)
                return 1;
            //bool highCard = true;

            return 0;
        }

        bool IsRoyalFlush(int jokerCount)
        {
            for (int suit = 1; suit < _mm.GetLength(0); suit++)
            {
                int count = 0;
                if (_mm[suit, 10] > 0)
                    count++;
                if (_mm[suit, 11] > 0)
                    count++;
                if (_mm[suit, 12] > 0)
                    count++;
                if (_mm[suit, 13] > 0)
                    count++;
                if (_mm[suit, 14] > 0)
                    count++;

                if (count + jokerCount >= 5)
                {
                    return true;
                }
            }

            return false;
        }

        bool IsStraightFlush(int jokerCount)
        {
            for (int suit = 1; suit < _mm.GetLength(0); suit++)
            {
                for (int num = 1; num < _mm.GetLength(1) - 4; num++)
                {
                    int count = 0;
                    if (_mm[suit, num] > 0)
                        count++;
                    if (_mm[suit, num + 1] > 0)
                        count++;
                    if (_mm[suit, num + 2] > 0)
                        count++;
                    if (_mm[suit, num + 3] > 0)
                        count++;
                    if (_mm[suit, num + 4] > 0)
                        count++;

                    if (count + jokerCount >= 5)
                    {
                        return true;
                    }
                }

            }

            return false;
        }
        bool IsFourCard(int jokerCount)
        {
            for (int num = 2; num < _mm.GetLength(1); num++)
            {
                int count = 0;
                if (_mm[1, num] > 0)
                    count++;
                if (_mm[2, num] > 0)
                    count++;
                if (_mm[3, num] > 0)
                    count++;
                if (_mm[4, num] > 0)
                    count++;

                if (count + jokerCount >= 4)
                {
                    return true;
                }
            }


            return false;
        }

        bool IsFullHouse(int jokerCount)
        {
            int pairCount = 0;
            int firstPairCount = 0;
            int secondPairCount = 0;
            for (int num = 2; num < _mm.GetLength(1); num++)
            {
                int count = 0;
                if (_mm[1, num] > 0)
                    count++;
                if (_mm[2, num] > 0)
                    count++;
                if (_mm[3, num] > 0)
                    count++;
                if (_mm[4, num] > 0)
                    count++;

                if (count > 1)
                {
                    pairCount++;
                    if (pairCount == 1)
                        firstPairCount = count;
                    else if (pairCount == 2)
                        secondPairCount = count;
                }
            }

            if (firstPairCount + secondPairCount + jokerCount >= 5)
            {
                return true;
            }

            return false;
        }

        bool IsFlush(int jokerCount)
        {
            for (int suit = 1; suit < _mm.GetLength(0); suit++)
            {
                int count = 0;
                for (int num = 2; num < _mm.GetLength(1); num++)
                {
                    if (_mm[suit, num] > 0)
                        count++;
                }

                if (count + jokerCount >= 5)
                {
                    return true;
                }
            }

            return false;
        }
        bool IsStraight(int jokerCount)
        {
            for (int num = 1; num < _mm.GetLength(1) - 4; num++)
            {
                int count = 0;
                int num_0 = _mm[1, num + 0] + _mm[2, num + 0] + _mm[3, num + 0] + _mm[4, num + 0];
                int num_1 = _mm[1, num + 1] + _mm[2, num + 1] + _mm[3, num + 1] + _mm[4, num + 1];
                int num_2 = _mm[1, num + 2] + _mm[2, num + 2] + _mm[3, num + 2] + _mm[4, num + 2];
                int num_3 = _mm[1, num + 3] + _mm[2, num + 3] + _mm[3, num + 3] + _mm[4, num + 3];
                int num_4 = _mm[1, num + 4] + _mm[2, num + 4] + _mm[3, num + 4] + _mm[4, num + 4];
                if (num_0 > 0)
                    count++;
                if (num_1 > 0)
                    count++;
                if (num_2 > 0)
                    count++;
                if (num_3 > 0)
                    count++;
                if (num_4 > 0)
                    count++;

                if (count + jokerCount >= 5)
                {
                    return true;
                }

            }


            return false;
        }
        bool IsTriful(int jokerCount)
        {
            for (int num = 2; num < _mm.GetLength(1); num++)
            {
                int count = 0;
                if (_mm[1, num] > 0)
                    count++;
                if (_mm[2, num] > 0)
                    count++;
                if (_mm[3, num] > 0)
                    count++;
                if (_mm[4, num] > 0)
                    count++;

                if (count + jokerCount >= 3)
                {
                    return true;
                }
            }

            return false;
        }

        bool IsTwoPairs(int jokerCount)
        {
            int pairCount = 0;
            int firstPairCount = 0;
            int secondPairCount = 0;
            for (int num = 2; num < _mm.GetLength(1); num++)
            {
                int count = 0;
                if (_mm[1, num] > 0)
                    count++;
                if (_mm[2, num] > 0)
                    count++;
                if (_mm[3, num] > 0)
                    count++;
                if (_mm[4, num] > 0)
                    count++;

                if (count > 1)
                {
                    pairCount++;
                    if (pairCount == 1)
                        firstPairCount = count;
                    else if (pairCount == 2)
                        secondPairCount = count;
                }
            }

            if (firstPairCount + secondPairCount + jokerCount >= 4)
            {
                return true;
            }

            return false;
        }

        bool IsOnePair(int jokerCount)
        {
            for (int num = 2; num < _mm.GetLength(1); num++)
            {
                int count = 0;
                if (_mm[1, num] > 0)
                    count++;
                if (_mm[2, num] > 0)
                    count++;
                if (_mm[3, num] > 0)
                    count++;
                if (_mm[4, num] > 0)
                    count++;

                if (count + jokerCount > 1)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
