
namespace Tetris.Engine.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using Algorithms;
    using Extensions;


    public class Engine
    {
        private readonly IAlgorithm algorithm;

        public class MoveCalculation
        {
            private BoardManager    m_Manager;
            private Block           m_Block;
            private IAlgorithm      m_Algorithm;

            private int             m_Rotation;
            private int             m_Column;

            private Move            m_Move;
            public Move             Move
            {
                get
                {
                    if (m_Move == null)
                    {
                        var rowClearings = m_Manager.GameStats.TotalRowClearings;

                        var columnInitial = m_Manager.ActiveBlock.Position.Column;
                        // move down until collision
                        while (m_Manager.CheckBlock(m_Block.Move(Tetris.Engine.Move.Down))) {}

                        // restore previous block position
                        m_Manager.ActiveBlock.Merge(m_Block.Move(Tetris.Engine.Move.Up));

                        // place block & update board
                        m_Manager.LockBlock();
                        m_Manager.CheckBoard();

                        // instantiate move
                        m_Move = new Move
                        {
                            ColumnOffSet = m_Column - columnInitial,
                            Fitness = m_Manager.CanSpawnBlock()
                                ? m_Algorithm.CalculateFitness(m_Manager.GameBoard, m_Block.Position.Row, m_Manager.GameStats.TotalRowClearings - rowClearings)
                                : int.MaxValue,
                            IsValid = true,
                            Rotation = m_Rotation
                        };
                    }

                    return m_Move;
                }
            }

            public MoveCalculation(int rotation, int column, Block block, BoardManager manager, IAlgorithm algorithm)
            {
                m_Rotation = rotation;
                m_Column = column;
                m_Block = block;
                m_Manager = manager;
                m_Algorithm = algorithm;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public Engine(IAlgorithm algorithm)
        {
            this.algorithm = algorithm;
        }

        public Move GetNextMove(BoardManager manager)
        {
            if (manager.ActiveBlock == null)
                return new Move();

            var moves = GetMoves(manager);

            return moves.FirstOrDefault() ?? new Move();
        }

        public IOrderedEnumerable<Move> GetMoves(BoardManager manager, Block activeBlock)
        {
            var moves = new List<Move>();

            if (activeBlock == null)
                return moves.OrderByDescending(x => x.IsValid);
            activeBlock = activeBlock.Clone();

            // enumerate all possible block x positions with rotations, calculate their fitness
            var rowClearings = manager.GameStats.TotalRowClearings;
            for (var rotation = 0; rotation < activeBlock.BlockRotations; rotation++)
            for (var column = -2; column < manager.NumberOfColumns; column++)
            {
                var tempManager = new BoardManager(manager.GameBoard.DeepClone(), activeBlock.Clone(), manager.GameStats.Clone(), null);
                var tempBlock = tempManager.ActiveBlock.Clone();

                // set rotation
                for (var i = 0; i < rotation; i++)
                    tempBlock.Move(Tetris.Engine.Move.RotateRight);

                // set x position
                var columnOffset = column - activeBlock.Position.Column;

                // create move if appropriate position
                if (moveBlock())
                {
                    while (tempManager.CheckBlock(tempBlock.Move(Tetris.Engine.Move.Down))) {}

                    // restore previous block position
                    tempManager.ActiveBlock.Merge(tempBlock.Move(Tetris.Engine.Move.Up));

                    tempManager.LockBlock();
                    tempManager.CheckBoard();
                    var rowsCleared = tempManager.GameStats.TotalRowClearings - rowClearings;
                    moves.Add(new Move
                    {
                        ColumnOffSet = columnOffset,
                        Fitness = tempManager.CanSpawnBlock()
                            ? algorithm.CalculateFitness(tempManager.GameBoard, tempBlock.Position.Row, rowsCleared)
                            : int.MaxValue,
                        IsValid = true,
                        Rotation = rotation
                    });
                }

                /////////////////////////////////////
                bool moveBlock()
                {
                    var step = columnOffset > 0 ? 1 : -1;
                    var move = columnOffset > 0 ? Tetris.Engine.Move.Right : Tetris.Engine.Move.Left;
                    for (var n = 0; n != columnOffset; n += step)
                        if (tempManager.CheckBlock(tempBlock.Move(move)) == false)
                            return false;

                    return true;
                }
            }

            return moves.OrderByDescending(x => x.IsValid).ThenBy(x => x.Fitness);
        }
        public IOrderedEnumerable<Move> GetMoves(BoardManager manager)
        {
            return GetMoves(manager, manager.ActiveBlock);
        }
        
        public IEnumerable<MoveCalculation> GetMoveCalculations(BoardManager manager)
        {
            if (manager.ActiveBlock == null)
                yield break;

            var managerInstance = new BoardManager(manager.GameBoard.DeepClone(), manager.ActiveBlock.Clone(), manager.GameStats.Clone());

            // enumerate all possible block x positions with rotations, calculate their fitness
            for (var rotation = 0; rotation < managerInstance.ActiveBlock.BlockRotations; rotation++)
            for (var column = -2; column < managerInstance.NumberOfColumns; column++)
            {
                // clone block initial copy
                var tempBlock = managerInstance.ActiveBlock.Clone();

                // set rotation
                for (var i = 0; i < rotation; i++)
                    tempBlock.Move(Tetris.Engine.Move.RotateRight);

                // set x position
                var columnOffset = column - manager.ActiveBlock.Position.Column;

                // create move if appropriate position
                if (moveBlock())
                {
                    var tempManager = new BoardManager(managerInstance.GameBoard.DeepClone(),
                        managerInstance.ActiveBlock.Clone(), managerInstance.GameStats.Clone());

                    yield return new MoveCalculation(rotation, column, tempBlock, tempManager, algorithm);
                }

                /////////////////////////////////////
                bool moveBlock()
                {
                    var step = columnOffset > 0 ? 1 : -1;
                    var move = columnOffset > 0 ? Tetris.Engine.Move.Right : Tetris.Engine.Move.Left;
                    for (var n = 0; n != columnOffset; n += step)
                        if (managerInstance.CheckBlock(tempBlock.Move(move)))
                            return false;

                    return true;
                }
            }
        }
    }
}
