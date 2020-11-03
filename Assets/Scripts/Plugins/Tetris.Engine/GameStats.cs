using System;

namespace Tetris.Engine
{
    public class GameStats
    {
        public GameStats()
        {
            OneRowClearings = 0;
            TwoRowsClearings = 0;
            ThreeRowsClearings = 0;
            FourRowsClearings = 0;
            TotalRowClearings = 0;
            BlocksSpawned = 0;
            GameSteps = 0;
            GameActions = 0;
        }

        public int OneRowClearings { get; private set; }
        public int TwoRowsClearings { get; private set; }
        public int ThreeRowsClearings { get; private set; }
        public int FourRowsClearings { get; private set; }
        public int TotalRowClearings { get; private set; }
        public int BlocksSpawned { get; private set; }
        public int Fitness => TotalRowClearings;
        public int GameSteps { get; private set; }
        public int GameActions { get; private set; }

        public void GameStep()
        {
            GameSteps ++;
        }

        public void GameAction()
        {
            GameActions ++;
        }

        public void NewSpawn()
        {
            BlocksSpawned++;
        }

        public void NewRowClearings(int clearedRows)
        {
            switch (clearedRows)
            {
                case 0: break;
                case 1:
                    OneRowClearings++;
                    break;
                case 2:
                    TwoRowsClearings++;
                    break;
                case 3:
                    ThreeRowsClearings++;
                    break;
                case 4:
                    FourRowsClearings++;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(clearedRows));
            }

            TotalRowClearings += clearedRows;
        }

        public GameStats Clone()
        {
            return new GameStats
            {
                OneRowClearings = OneRowClearings,
                TwoRowsClearings = TwoRowsClearings,
                ThreeRowsClearings = ThreeRowsClearings,
                FourRowsClearings = FourRowsClearings,
                TotalRowClearings = TotalRowClearings,
                BlocksSpawned = BlocksSpawned
            };
        }
    }
}