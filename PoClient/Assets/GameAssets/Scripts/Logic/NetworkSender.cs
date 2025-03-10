using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public void SendTakeCards(int playerId, List<int> cards)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleTakeCards(playerId, cards);
            }
        }
        public void SendPass(int playerId)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandlePass(playerId);
            }
        }

        public void SendFirstPlace(int playerId, int y, int x, int card)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleFirstPlace(playerId, y, x, card);
            }
        }
        public void SendSecondPlace(int playerId, int y, int x, int card)
        {
            //if (bUsePacketQueue)
            //{
            //    SCInitialMelt pkt = new SCInitialMelt((ushort)PacketID.InitialMelt, playerId, hbtileSets);
            //    _room.AddPacketQueue(pkt);
            //}
            //else
            {
                _room.HandleSecondPlace(playerId, y, x, card);
            }
        }
    }
}
