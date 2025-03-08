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
        public void HandleDeal2Cards(int playerId, List<int> cards)
        {
            _dealer.HandleDeal2Cards(playerId, cards);
        }

        #endregion
    }
}
