using System;
using System.Collections.Generic;

namespace Tetris.Engine
{
    using System.Linq;
    
    public interface IBoardEventHandler
    {
        void OnCollapseRows(List<int> rows);
        void OnBlockSpawned(Block block);
        void OnLockBlock(Block block);
        void OnUnvalidMove(Move move);
        void OnMove(Block block, Move move);
        void OnGameOver();
    }
    
    public class BoardManager
    {
        private readonly int        m_Rows;
        private readonly int        m_Columns;
        
        public virtual Block                    ActiveBlock { get; private set; }
        public Block                            PreviousBlock { get; private set; }
        public virtual bool[][]                 GameBoard { get; private set; }
        public virtual GameStats                GameStats { get; }
        public IBoardEventHandler               EventHandler { get; private set; }
        public bool                             IsSoftDrop { get; private set; }
        public bool                             IsClear => GameBoard[0].All(n => n == false);   // board cleared if first row is empty
        public int                              NumberOfColumns => m_Columns;
        public int                              NumberOfRows => m_Rows;

        //////////////////////////////////////////////////////////////////////////
        public BoardManager(bool[][] gameBoard) : this (gameBoard, null)
        {
        }

        public BoardManager(bool[][] gameBoard, Block activeBlock)
        {
            GameBoard = gameBoard;
            m_Rows = gameBoard.GetLength(0);
            ActiveBlock = activeBlock;
            m_Columns = gameBoard[0].Length;
            GameStats = new GameStats();
        }

        internal BoardManager(bool[][] gameBoard, Block activeBlock, GameStats gameStats, IBoardEventHandler eventHandler = null)
        {
            GameBoard = gameBoard;
            m_Rows = gameBoard.GetLength(0);
            ActiveBlock = activeBlock;
            m_Columns = gameBoard[0].Length;
            GameStats = gameStats;
            EventHandler = eventHandler;
        }

        public bool CanSpawnBlock()
        {
            if (ActiveBlock != null)
            {
                return false;
            }
            //var leftSpawnArea = (this.columns - 4) / 2;

            for (var row = m_Rows - 1; row >= m_Rows -2; row--)
            {
                // top two rows must be clean
                if (GameBoard[row].Any(x => x))
                    return false;
            }

            return true;
        }

        public Block SpawnBlock()
        {
            return SpawnBlock(Block.GetRandomBlockType());
        }

        public Block SpawnBlock(BlockType type)
        {
            return SpawnBlock(new Block(type), true);
        }
        
        public Block SpawnBlock(Block block, bool autoPosition = true)
        {
            if (block == null)
                return null;

            // calculate position if required
            if (autoPosition)
            {
                block.Position.Column = (m_Columns - block.BlockType.BlockDimension()) / 2;
                block.Position.Row = m_Rows - block.BlockType.BlockDimension();
            }

            if (CanSpawnBlock() == false)
                return null;
            

            ActiveBlock = block;
            GameStats.NewSpawn();
            EventHandler?.OnBlockSpawned(ActiveBlock);
            
            return ActiveBlock;
        }

        public BoardManager CheckBoard()
        {
            var clearedRows = new List<int>();
            for (var rowIndex = 0; rowIndex < m_Rows; rowIndex++)
                if (IsRowFull(rowIndex))
                    clearedRows.Add(rowIndex);

            CollapseRows(clearedRows);

            return this;
        }

        public bool[][] GetBoard()
        {
            return GameBoard;
        }

        public bool Move(Move move)
        {
            // move must be set
            if (move == Engine.Move.None)
                return true;

            // must have active block
            if (ActiveBlock == null)
                return false;

            // implement fall
            if (move == Engine.Move.Fall)
            {
                var collisionHeight = int.MaxValue;
                foreach (var coord in ActiveBlock.GetCoords())
                {
                    var steps = 0;
                    for (var y = coord.Row - 1; y >= 0; y--)
                    {
                        if (GameBoard[y][coord.Column] == false)
                            steps ++;
                        else
                            break;
                    }

                    collisionHeight = Math.Min(steps, collisionHeight);
                }
                ActiveBlock.Position.Row -= collisionHeight;

                EventHandler?.OnMove(ActiveBlock, move);

                LockBlock();
                CheckBoard();
                IsSoftDrop = false;
                return true;
            }

            var tempMove = ActiveBlock.Clone().Move(move);
            var validMove = CheckBlock(tempMove);

            // try to solve rotation offset
            if (validMove == false && (move == Engine.Move.RotateLeft || move == Engine.Move.RotateRight))
            {
                var solved = _TrySolveRotation(ref tempMove);
                if (solved != null)
                {
                    tempMove = solved;
                    validMove = true;
                }
            }

            // if un valid move down lock block
            if (validMove == false && move == Engine.Move.Down)
            {
                LockBlock();
                CheckBoard();
                IsSoftDrop = false;
                return true;
            }

            // move is un valid
            if (validMove == false)
            {
                EventHandler?.OnUnvalidMove(move);
                return false;
            }

            // apply move
            ActiveBlock = tempMove;
            IsSoftDrop = _IsSoftDrop(ActiveBlock);
            
            EventHandler?.OnMove(ActiveBlock, move);
            return true;
        }

        public bool IsValidMove(Move move)
        {
            var tempMove = ActiveBlock.Clone();
            tempMove.Move(move);

            return !CheckBlock(tempMove);
        }

        public int GetCollisionHeight(Block block)
        {
            var height = int.MaxValue;
            foreach (var coord in block.GetCoords())
            {
                if (coord.Row < 0)
                    continue;
                if (coord.Row >= NumberOfRows)
                    continue;

                if (coord.Column < 0)
                    continue;
                if (coord.Column >= NumberOfColumns)
                    continue;

                var distance = 0;
                for (var row = coord.Row; row >= 0; row--)
                {
                    if (GameBoard[row][coord.Column])
                        break;

                    distance ++;
                }

                height = Math.Min(height, distance);
            }

            return height;
        }

        internal bool CheckBlock(Block block)
        {
            for (var row = block.Position.Row; row < block.Position.Row + block.BlockMatrixSize; row++)
            for (var column = block.Position.Column; column < block.Position.Column + block.BlockMatrixSize; column++)
            {
                if (!block.BlockMatrix[row - block.Position.Row][column - block.Position.Column])
                    continue;

                if (row < 0)
                    return false;

                if (row >= m_Rows)
                    return false;

                if (column < 0)
                    return false;

                if (column >= m_Columns)
                    return false;

                if (GameBoard[row][column])
                    return false;
            }

            return true;
        }

        internal void LockBlock()
        {
            if (ActiveBlock == null)
                return;

            PreviousBlock = ActiveBlock;
            ActiveBlock = null;

            for (var row = PreviousBlock.Position.Row; row < PreviousBlock.Position.Row + PreviousBlock.BlockMatrixSize && row < m_Rows; row++)
            {
                for (var column = PreviousBlock.Position.Column; column < PreviousBlock.Position.Column + PreviousBlock.BlockMatrixSize && column < m_Columns; column++)
                {
                    if (row < 0)
                        continue;

                    if (row >= m_Rows)
                        continue;

                    if (column < 0)
                        continue;

                    if (column >= m_Columns)
                        continue;

                    if (GameBoard[row][column])
                        continue;

                    GameBoard[row][column] = GameBoard[row][column] | PreviousBlock.BlockMatrix[row - PreviousBlock.Position.Row][column - PreviousBlock.Position.Column];
                }
            }
            
            // Send event
            EventHandler?.OnLockBlock(PreviousBlock);
        }

        internal bool IsRowFull(int row)
        {
            return GameBoard[row].All(x => x);
        }

        internal void CollapseRows(List<int> rowList, bool sendEvent = true, bool addStats = true)
        {
            for (var n = 0; n < rowList.Count; n++)
                _CollapseRow(rowList[n] - n);

            // add stats & send event
            if (rowList.Count != 0)
            {
                if (sendEvent)
                    GameStats.NewRowClearings(rowList.Count);
                if (addStats)
                    EventHandler?.OnCollapseRows(rowList);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public IEnumerable<Position> GetBoardBlocks()
        {
            for (var row = 0; row < GameBoard.GetLength(0); row++)
            for (var column = 0; column < GameBoard[row].GetLength(0); column++)
                if (GameBoard[row][column])
                    yield return new Position(row, column);
        }

        private BoardManager _CollapseRow(int rowToCollapse)
        {
            // Move rows down in array, which deletes the current row
            for (var rowIndex = rowToCollapse; rowIndex < m_Rows -1; rowIndex++)
            {
                GameBoard[rowIndex] = GameBoard[rowIndex + 1];
            }

            // Make sure top line is cleared
            GameBoard[m_Rows -1] = new bool[m_Columns];

            return this;
        }

        private bool _IsSoftDrop(Block block)
        {
            foreach (var coord in block.GetCoords().Select(n => new Position(n.Row - 1, n.Column)))
            {
                // most bottom position
                if (coord.Row < 0)
                    return true;

                // has adjacent down block
                if (GameBoard[coord.Row][coord.Column])
                    return true;
            }

            return false;
        }

        private Block _TrySolveRotation(ref Block tempMove)
        {
            var block = tempMove.Clone();

            // move extra left
            if (CheckBlock(block.Move(Engine.Move.Left)))
                return block;
            else
                block.Move(Engine.Move.Right);

            // move extra right
            if (CheckBlock(block.Move(Engine.Move.Right)))
                return block;
            else
                block.Move(Engine.Move.Left);

            // if I block perform double move
            if (tempMove.BlockType == BlockType.I)
            {
                // right right
                if (CheckBlock(block.Move(Engine.Move.Right).Move(Engine.Move.Right)))
                    return block;
                else
                    block.Move(Engine.Move.Left).Move(Engine.Move.Left);
                
                // left left
                if (CheckBlock(block.Move(Engine.Move.Left).Move(Engine.Move.Left)))
                    return block;
            }

            return null;
        }
    }
}
