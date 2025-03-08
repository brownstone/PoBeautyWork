using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class NetworkSender
    {
        PokerBeautyBase _room;

        private NetworkSender() { }
        private static NetworkSender _instance = null;
        public static NetworkSender Instance
        {
            get
            {
                if (_instance == null) _instance = new NetworkSender();
                return _instance;
            }
        }

        public void Init(PokerBeautyBase room)
        {
            _room = room;
        }
        public void SendGameStart(int playerId, int adType)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleGameStart(playerId, adType);
            }
        }
        public void SendGameRetry(int playerId, int adType)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleGameRetry(playerId, adType);
            }
        }

        public void SendDeal9Cards(List<int> cards)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleDeal9Cards(cards);
            }
        }
        public void SendDeal2Cards(int playerId, List<int> cards)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleDeal2Cards(playerId, cards);
            }
        }

    }
}
