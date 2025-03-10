using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public partial class PokerBeautyBase
    {
        #region Handle Packet
        public void HandleGameStart(int playerId, int adType)
        {
            //_dealer.HandleGameStart(playerId, adType);
        }
        public void HandleGameRetry(int playerId, int adType)
        {
            //_dealer.HandleGameRetry(playerId, adType);
        }
        public void HandleDeal9Cards(List<int> cards)
        {
            _dealer.HandleDeal9Cards(cards);
        }
        public void HandleTakeCards(int playerId, List<int> cards)
        {
            _dealer.HandleTakeCards(playerId, cards);
        }
        public void HandlePass(int playerId)
        {
            _dealer.HandlePass(playerId);
        }
        public void HandleFirstPlace(int playerId, int y, int x, int card)
        {
            _dealer.HandleFirstPlace(playerId, y, x, card);
        }
        public void HandleSecondPlace(int playerId, int y, int x, int card)
        {
            _dealer.HandleSecondPlace(playerId, y, x, card);
        }

        #endregion
    }
}
