using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Engine
{
    public class GameManager
    {
        private readonly BoardManager   boardManager;
        private IBlockProvider          m_BlockProvider;

        public BoardManager             BoardManager => boardManager;
        public GameStats                GameStats => boardManager.GameStats;
        public Block                    ActiveBlock => boardManager.ActiveBlock;
        public Block                    PreviousBlock => boardManager.PreviousBlock;
        public bool                     IsSoftDrop => BoardManager.IsSoftDrop;
        public bool                     IsGameOver { get; private set;}

        //////////////////////////////////////////////////////////////////////////
        public GameManager(int height, int width, IBoardEventHandler eventHandler = null,
            IBlockProvider blockProvider = null)
        {
            var gameBoard = new bool[height][];
            for (var i = 0; i < height; i++) gameBoard[i] = new bool[width];

            boardManager = new BoardManager(gameBoard, null, new GameStats(), eventHandler);
            BlockProvider = blockProvider;
        }

        public IBlockProvider BlockProvider
        {
            get => m_BlockProvider;
            set
            {
                // if null set random
                m_BlockProvider = value ?? new BlockProviderRandom();
            }
        }

        public void OnGameLoopStep()
        {
            // spawn empty block or move down
            if (boardManager.ActiveBlock == null)
            {
                if (boardManager.SpawnBlock(m_BlockProvider.SpawnBlock()) == null)
                {
                    IsGameOver = true;
                    boardManager.EventHandler?.OnGameOver();
                }

                return;
            }

            boardManager.Move(Move.Down);
            GameStats.GameStep();
        }

        public void CollapseRows(List<int> rows, bool sendEvent = true, bool addStats = true)
        {
            boardManager.CollapseRows(rows, sendEvent, addStats);
        }

        public void CollapseRows(params int[] rows)
        {
            boardManager.CollapseRows(rows.ToList());
        }

        public bool MoveBlock(Move move)
        {
            var result = boardManager.Move(move);
            if (result)
                GameStats.GameAction();
            return result;
        }
    }
}