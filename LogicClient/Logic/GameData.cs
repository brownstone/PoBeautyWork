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
