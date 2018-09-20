namespace ooparts.dungen.Dungeons
{
	public class BSPTreeNode
	{
		public IntVector2 Coordinates;
		public IntVector2 PartitionSize;

		public BSPTreeNode LNode;
		public BSPTreeNode RNode;
		public BSPRoom Room; // TODO: need to check

		public BSPTreeNode(IntVector2 size, IntVector2 coordinates)
		{
			PartitionSize = size;
			Coordinates = coordinates;
		}
	}
}