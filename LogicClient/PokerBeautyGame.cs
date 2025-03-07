using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerH
{
    public class PokerBeautyGame
    {
        protected PokerBeautyBase _gameBase = new PokerBeautyBase();

        protected enum GameProgress { GP_NONE, GP_START, GP_GAMING, GP_END };
        protected GameProgress _currGameProgress = GameProgress.GP_NONE;
        private float _gameTimer = 0.0f;
        readonly float START_TERM_TIMER = 0.2f;

        int MyPlayerId { get; set; } = 1001;


        public void Init()
        {
            _currGameProgress = GameProgress.GP_START;
            OnEnterGameStatus(GameProgress.GP_START);
        }
        private void OnEnterGameStatus(GameProgress newProgress)
        {
            if (newProgress == GameProgress.GP_GAMING)
            {
                GameRoomInfo roomInfo = new GameRoomInfo()
                {
                    tempo = 1,
                    gameMode = 1,
                    practiceLevel = 0
                };
                _gameBase.Init();
                _gameBase.SetRoomInfo(roomInfo);
                _gameBase.AddFakeUser(MyPlayerId);


                // bind actions
                //_gameBase._dealer._OnPrepare += OnEventPrepare;
                //_gameBase._dealer._OnPlayerSetting += OnEventPlayerSetting;
                //_gameBase._dealer._OnGameReStart += OnEventGameReStart;


                _gameBase.StartGame();
            }
        }
        public void Update(float tick)
        {
            switch (_currGameProgress)
            {
                case GameProgress.GP_START:
                    {
                        _gameTimer += tick;
                        if (_gameTimer > START_TERM_TIMER)
                        {
                            OnEnterGameStatus(GameProgress.GP_GAMING);
                            _currGameProgress = GameProgress.GP_GAMING;
                        }
                        break;
                    }
                case GameProgress.GP_GAMING:
                    {
                        _gameBase.UpdateState(tick);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public void Render()
        {
            Console.SetCursorPosition(0, 0);
            var defaultColor = Console.ForegroundColor;

            Console.WriteLine(_currGameProgress);
            if (_currGameProgress == GameProgress.GP_START)
            {
                Console.WriteLine("Welcome TrickTaking Game");
            }
            else if (_currGameProgress == GameProgress.GP_GAMING)
            {
                _gameBase.Render();
            }

            Console.ForegroundColor = defaultColor;
        }

    }
}
