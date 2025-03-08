using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class StatusTimer<T>
    {
        public float _accumTime = 0.0f;
        public T _currStatus;

        public void SetNewStatus(T t)
        {
            _accumTime = 0.0f;
            _currStatus = t;
        }
    }

    public struct Point2
    {
        public int X { get; }
        public int Y { get; }

        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Point2 other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public struct BeautyResult
    {
        public int index;
        public int cardFirst;
        public int cardSecond;
        public int rank;
        public void Clear()
        {
            cardFirst = 0;
            cardSecond = 0;
            rank = 0;
        }
    }
}

    public struct GameRoomInfo
    {
        public int tempo;  // 1(slow)/2(normal)/3(fast)/4(very fast)
        public int gameMode; // 1(single)/2(practice)
        public int practiceLevel;

        public void Init(int gameMode, int tempo, int practiceLevel)
        {
            this.gameMode = gameMode;
            this.tempo = tempo;
            this.practiceLevel = practiceLevel;
        }
    }
}
