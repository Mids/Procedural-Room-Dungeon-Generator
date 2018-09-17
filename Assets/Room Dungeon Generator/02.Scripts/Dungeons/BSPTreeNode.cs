using System.Collections;
using System.Collections.Generic;
using ooparts.dungen;
using UnityEngine;


namespace ooparts.dungen.Dungeons
{
	public class BSPTreeNode
	{
		public IntVector2 PartitionSize;
		public BSPTreeNode LNode;
		public BSPTreeNode RNode;
		public Room room; // TODO: need to check
	}
}