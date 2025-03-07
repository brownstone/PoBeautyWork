using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{

    public class Player
    {
        public int _playerId;
        public string _nickname;
        public List<int> _hands = new();
        public bool _isAI;
        public bool _turn;
        public User _user;

        public void Init(int index)
        {
            _playerId = 10 + index;
            _isAI = true;

        }
        public void SetUser(User user)
        {
            _user = user;
            _playerId = user._id;
            _nickname = user._name;
            _isAI = false;
        }

        public void OnGameReStart()
        {
            //Clear();
            _turn = false;
            _hands.Clear();
        }
        public void AddCards(List<int> cards)
        {
            _hands.AddRange(cards);
        }
    }

    public class PlayerManager
    {
        public Player[] _players = new Player[2];

        public void Init()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                Player p = new Player();
                p.Init(i);
                _players[i] = p;
            }
        }

        public void ReconnectPlayers(UserManager userManager)
        {
            for (int i = 0; i < userManager._users.Count; i++)
            {
                User user = userManager._users[i];
                bool exist = Array.Exists<Player>(_players, e => e._playerId == user._id);

                if (exist)
                    continue;

                for (int j = 0; j < _players.Length; j++)
                {
                    Player p = _players[j];
                    if (p._isAI)
                    {
                        p.SetUser(user);
                        break;
                    }
                }
            }
        }

        public void OnGameReStart()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                Player p = _players[i];
                p.OnGameReStart();
            }
        }


        public Player GetPlayer(int playerId)
        {
            for (int i = 0; i < _players.Length; i++)
            {
                Player p = _players[i];
                if (p._playerId == playerId)
                {
                    return p;
                }
            }
#if LOGIC_CONSOLE_TEST
            Debugger.Break();
#else
            UnityEngine.Debug.Log($"GetPlayer failed! Invalid PlyerId {playerId}");
            UnityEngine.Debug.Break();
#endif
            return null;
        }
        public bool GetPlayerIds(int[] playerIds)
        {
            int index = 0;
            foreach (Player p in _players)
            {
                playerIds[index++] = p._playerId;
            }

            return true;
        }

#if LOGIC_CONSOLE_TEST
        public void Render()
        {
            Console.WriteLine(" ======================= Player Manager ========================== ");
            var defaultColor = Console.ForegroundColor;

            foreach (Player p in _players)
            {
                Console.Write("{0, 8}  ", (p != null) ? p._nickname : "None");

                if (p._hands.Count > 0)
                {
                    foreach (int c in p._hands)
                    {
                        Card.DrawCard(c);
                    }
                }

                Console.WriteLine();
            }
            Console.ForegroundColor = defaultColor;
            Console.WriteLine(" ================================================================== ");
        }
#endif
    }
}
