using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public partial class PokerBeautyBase
    {
        protected UserManager _userManager = new UserManager();
        public Dealer _dealer = new Dealer();


        public enum GameRoomStatus { GR_NONE, GR_START, GR_GAME, GR_END }
        public StatusTimer<GameRoomStatus> _currRoomStatus = new StatusTimer<GameRoomStatus>();

        public void Init()
        {
            NetworkSender.Instance.Init(this);

            _currRoomStatus.SetNewStatus(GameRoomStatus.GR_NONE);
            _dealer.Init(_userManager);
        }

        public void SetRoomInfo(GameRoomInfo roomInfo)
        {
            _dealer.SetRoomInfo(roomInfo);
        }

        public void AddFakeUser(int myPlayerId)
        {
            User user = new User();
            user.InitUser(myPlayerId, "brown");
            _userManager.AddUser(user);
        }

        public void StartGame()
        {
            _currRoomStatus.SetNewStatus(GameRoomStatus.GR_START);
            _dealer.StartGame();
        }

        public void UpdateState(float tick)
        {
            //UpdatePacket();

            switch (_currRoomStatus._currStatus)
            {
                case GameRoomStatus.GR_START:
                    {
                        _currRoomStatus._accumTime += tick;
                        if (_currRoomStatus._accumTime > 0.4f)
                        {
                            _currRoomStatus.SetNewStatus(GameRoomStatus.GR_GAME);
                        }
                        break;
                    }
                case GameRoomStatus.GR_GAME:
                    {
                        _dealer.Update(tick);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
#if LOGIC_CONSOLE_TEST
        public void Render()
        {
            Console.Clear();
            Console.WriteLine(" ================================================= ");
            Console.WriteLine("Game Room Status : {0} {1:F2}", _currRoomStatus._currStatus, _currRoomStatus._accumTime);

            _dealer.Render();
        }
#endif

    }
}
