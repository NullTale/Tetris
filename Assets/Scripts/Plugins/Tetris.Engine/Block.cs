using System.Collections.Generic;

namespace Tetris.Engine
{
    using System;
    using System.Linq;

    public class Block
    {
        private BlockType               blockType;
        private int                     rotationIndex;
        private static readonly Random  Random = new Random();

        public Block() : this(new Position())
        {
        }
        public Block(BlockType type) : this(type, new Position())
        {
        }

        public Block(Position position) : this(GetRandomBlockType(), position)
        {
        }

        public Block(BlockType type, Position position)
        {
            rotationIndex = 0;
            Position = position;
            blockType = type;
            BlockMatrix = blockType.Rotation(rotationIndex);
            BlockMatrixSize = blockType.BlockDimension();
            BlockRotations = blockType.BlockRotations();
        }

        public bool[][]         BlockMatrix { get; private set; }
        public Position         Position { get; private set; }
        public int              RotationIndex => rotationIndex;

        public virtual bool     Placed { get; set; }
        public int              BlockMatrixSize { get; private set; }
        public int              BlockRotations { get; private set; }
        public BlockType        BlockType { get => blockType; set => blockType = value; }


        //////////////////////////////////////////////////////////////////////////
        public Block Move(Move move)
        {
            switch (move)
            {
                case Engine.Move.Left:
                    Position.Column--;
                    break;

                case Engine.Move.Right:
                    Position.Column++;
                    break;

                case Engine.Move.Down:
                    Position.Row--;
                    break;
                case Engine.Move.Up:
                    Position.Row ++;
                    break;

                case Engine.Move.RotateRight:
                    BlockMatrix = blockType.Rotation(++rotationIndex);
                    break;
                case Engine.Move.RotateLeft:
                    BlockMatrix = blockType.Rotation(--rotationIndex);
                    break;

                case Engine.Move.Fall:
                case Engine.Move.None:
                    break;

                default: throw new NotImplementedException(move.ToString("G"));
            }

            return this;
        }

        public Block Clone()
        {
            return new Block(blockType, new Position { Column = Position.Column, Row = Position.Row })
                {
                    BlockMatrix = BlockMatrix,
                    rotationIndex = rotationIndex,
                    BlockRotations = BlockRotations,
                    BlockMatrixSize = BlockMatrixSize
                };
        }

        public List<Position> GetCoords()
        {
            // convert block to cubes world coords
            var result = new List<Position>(4);

            for (var x = 0; x < BlockMatrix.GetLength(0); ++ x)
            for (var y = 0; y < BlockMatrix[x].GetLength(0); ++ y)
                if (BlockMatrix[x][y])
                    result.Add(new Position(){ Column = y + Position.Column, Row = x + Position.Row });
        
            return result;
        }

        internal void Merge(Block block)
        {
            BlockMatrix = block.BlockMatrix;
            BlockMatrixSize = block.BlockMatrixSize;
            BlockRotations = block.BlockRotations;
            Position = new Position { Column = block.Position.Column, Row = block.Position.Row };
            blockType = block.blockType;
        }

        public static BlockType GetRandomBlockType()
        {
            var blockTypes = Enum.GetValues(typeof(BlockType)).Cast<BlockType>().ToArray();

            return blockTypes[Random.Next(0, blockTypes.Length)];
        }
    }
}
