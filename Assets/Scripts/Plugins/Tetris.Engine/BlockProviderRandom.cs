namespace Tetris.Engine
{
    public class BlockProviderRandom : IBlockProvider
    {
        public Block SpawnBlock()
        {
            return new Block(Block.GetRandomBlockType());
        }
    }
}