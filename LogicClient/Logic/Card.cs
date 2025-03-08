using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class Card
    {
        //  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13
        // 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26
        // 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39
        // 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52
        static readonly int MAX_CARD_NUMBER_COUNT = 13; // 1(2), 2(3), 9(T), 10(J), 11(Q), 12(K), 13(A) ... 13
        static readonly int MAX_CARD_SUIT_COUNT = 4;   // 1(S), 2(D), 3(H), 4(C)
        static readonly int MAX_CARD_FULL_COUNT = MAX_CARD_NUMBER_COUNT * MAX_CARD_SUIT_COUNT; // 13 * 4 = 52 { 1... 52 }
        static readonly int MAX_CARD_JOKER_COUNT = 1;
        public static readonly int MAX_CARD_COUNT = MAX_CARD_FULL_COUNT + MAX_CARD_JOKER_COUNT; // 52 + 1 = 53
        public static readonly int JOKER1 = MAX_CARD_FULL_COUNT + 1; // 53

        public static readonly int AceNum = 13; // Spade Ace
        public static readonly int SpadeAce = 13; // Spade Ace
        public static readonly int DiaAce = 26; // Diamo Ace
        public static readonly int HeartAce = 39; // Heart Ace
        public static readonly int CloverAce = 52; // Clover Ace

        static public bool IsValidNumber(int c)
        {
            if (c < 1)
                return false;
            if (c > MAX_CARD_COUNT)
                return false;

            return true;
        }
        static public bool IsJoker(int c)
        {
            if (c == JOKER1)
                return true;

            return false;
        }

        static public int GetCardNum(int n)
        {
            int base0 = n - 1;
            int cardNum = (base0 % MAX_CARD_NUMBER_COUNT) + 1;
            return cardNum;
        }
        static public int GetCardSuit(int n)
        {
            if (IsValidNumber(n) == false)
                return 0;
            if (IsJoker(n))
                return 0;
            int base0 = n - 1;
            int cardColor = (base0 / MAX_CARD_NUMBER_COUNT) + 1;
            return cardColor;
        }
        static public int GetSuitAce(int suit)
        {
            int ace = suit * MAX_CARD_NUMBER_COUNT;

            if (IsValidNumber(ace) == false)
                return 0;
            return ace;
        }
        static public int GetCardFromSuitNum(int suit, int num)
        {
            if (num == 0) return 0;
            return (suit - 1) * MAX_CARD_NUMBER_COUNT + num;
        }

        static public string GetSuitName(int suit)
        {
            string[] suits = { "None", "Spade", "Diamond", "Heart", "Clover", "NoTrump" };

            return suits[suit];
        }

#if LOGIC_CONSOLE_TEST
        static public void DrawSuit(int cardColor)
        {
            var defaultColor = Console.ForegroundColor;

            if (cardColor == 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (cardColor == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            if (cardColor == 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (cardColor == 4)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            if (cardColor == 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            string[] suits = { "N", "Spade", "Dia", "Heart", "Clover" };

            Console.Write("{0, 1} ", suits[cardColor]);

            Console.ForegroundColor = defaultColor;
        }


        static public void DrawCard(int n)
        {
            var defaultColor = Console.ForegroundColor;

            if (n == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("   ");
                Console.ForegroundColor = defaultColor;
                return;
            }

            if (IsValidNumber(n) == false)
                return;

            if (IsJoker(n) == true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Jo ");
                Console.ForegroundColor = defaultColor;
                return;
            }

            int cardNum = GetCardNum(n);
            int cardColor = GetCardSuit(n);

            if (cardColor == 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (cardColor == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            if (cardColor == 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (cardColor == 4)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            string[] suits = { "N", "S", "D", "H", "C" };
            string[] nums = { "N", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };

            Console.Write("{0, 1}{1, 1} ", suits[cardColor], nums[cardNum]);
            Console.ForegroundColor = defaultColor;
        }
#endif
    }
}
