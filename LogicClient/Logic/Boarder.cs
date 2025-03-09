using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class Boarder
    {

        public const int BOARD_COL = 5; // 가로
        public const int BOARD_ROW = 5;
        protected int[,] _slots = new int[BOARD_COL, BOARD_ROW]; // zero base
        protected int[,] _slotCaches = new int[BOARD_COL, BOARD_ROW]; // zero base

        int[] _checkPos = { PackYX(0, 0), PackYX(0, 1), PackYX(0, 2), PackYX(0, 3), PackYX(0, 4), PackYX(1, 0), PackYX(2, 0), PackYX(3, 0) };
        int[][] _candisPos = new int[8][]; 
        
        List<int>[] _candis = new List<int>[8];


        public void Init()
        {
        }

        public int[,] GetCachedSlot()
        {
            Array.Copy(_slots, _slotCaches, _slots.Length);
            return _slotCaches;
        }

        bool IsValidIndex(int y, int x)
        {
            return IsValidIndex(_slots, y, x);
        }

        static bool IsValidIndex(int[,] clonedSlot, int y, int x)
        {
            return (y >= 0 && x >= 0 && y < clonedSlot.GetLength(0) && x < clonedSlot.GetLength(1));
        }


        public bool PlaceCard(int y, int x, int c)
        {
            return PlaceCard(_slots, y, x, c);
        }

        public bool PlaceCard(int[,] clonedSlot, int y, int x, int c)
        {
            if (IsValidIndex(clonedSlot, y, x) == false)
                return false;
            if (clonedSlot[y, x] != 0)
                return false;

            clonedSlot[y, x] = c;

            return true;
        }

        static public int PackYX(int y, int x)
        {
            int maskUp = 0xff00;
            int maskDown = 0x00ff;
            int yx = (((y << 8) & maskUp) | (x & maskDown));
            return yx;
        }
        static public int UnpackY(int yx)
        {
            int maskUp = 0xff00;
            //int maskDown = 0x00ff;
            int y = yx & maskUp;
            y = y >> 8;
            return y;
        }
        static public int UnpackX(int yx)
        {
            //int maskUp = 0xff00;
            int maskDown = 0x00ff;
            int x = (yx & maskDown);
            return x;
        }


        public List<int>[] GetCandis()
        {
            for (int i = 0; i < _candisPos.Length; i++)
            {
                for (int j = 0; j < _candisPos[0].Length; j++)
                {
                    int y = UnpackY(_candisPos[i][j]);
                    int x = UnpackX(_candisPos[i][j]);
                    int c = _slots[y, x];
                    _candis[i].Add(c);
                }
            }
            return _candis;
        }

        public void OnCleanUp()
        {
            int length = _slots.Length;
            Array.Clear(_slots, 0, _slots.Length);
        }


#if LOGIC_CONSOLE_TEST
        public void Render()
        {
            int yx = PackYX(3, 4);
            int yy = UnpackY(yx);
            int xx = UnpackX(yx);
            Console.WriteLine(" ======================= Boarder ========================== ");
            var defaultColor = Console.ForegroundColor;

            Console.Write("         ");
            for (int x = 0; x < _slots.GetLength(1); x++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" {0, 2} ", x);
            }
            Console.WriteLine("");

            for (int y = 0; y < _slots.GetLength(0); y++)
            {
                Console.Write("{0, 2} {1, 4}  ", y, PackYX(y, 0));
                for (int x = 0; x < _slots.GetLength(1); x++)
                {
                    int c = _slots[y, x];
                    if (c == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  . ");
                    }
                    else
                        Card.DrawCard(c);

                }
                Console.WriteLine();
            }
            //Console.WriteLine(" Island: {0}, Set: {1}, ActiveLevel: {2}", GetIslandCount(), GetSetCount(), _currActiveLevel);

            Console.ForegroundColor = defaultColor;
            Console.WriteLine(" ================================================================== ");
        }
#endif

    }
}
