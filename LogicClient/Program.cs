using System;

namespace PokerH // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static int renderCount = 0;

        static void Main(string[] args)
        {
            const int WAIT_TICK = 1000 / 30;
            Console.CursorVisible = false;
            PokerBeautyGame theGame = new PokerBeautyGame();
            theGame.Init();

            int lastTick = System.Environment.TickCount;

            theGame.Update(0.1f);
            theGame.Render();

            while (true)
            {
                #region 프레임 관리
                int currentTick = System.Environment.TickCount;
                int elapsedTick = currentTick - lastTick;

                if (elapsedTick < WAIT_TICK)
                    continue;
                lastTick = currentTick;
                #endregion
                theGame.Update(elapsedTick / 1000.0f);

                renderCount++;
                if (renderCount % 10 == 0)
                    theGame.Render();
            }
        }
    }
}