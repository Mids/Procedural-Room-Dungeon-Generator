using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ooparts.dungen.Dungeons
{
	public class BSPDungeon : Dungeon
	{
		public IntVector2 MapSize;
		public IntVector2 MinRoomSize;

		private BSPTreeNode _rootNode = new BSPTreeNode();
		
		// Generate whole dungeon
		public IEnumerator Generate()
		{
			yield return null;
		}

		// Generate Tree from root node
		public IEnumerator GenerateTree()
		{
			yield return null;
		}

		// Generate Room for the Partition
		public IEnumerator GenerateRooms()
		{
			yield return null;
		}

		// March until connected to room or corridor
		public IEnumerator ConnectRoomsWithCorridors()
		{
			yield return null;
		}

		// Put root node to split
		// Succeeded - Split them recursively and return 1
		// Failed - Generate Room and return 0
		public IEnumerator SplitPartition(BSPTreeNode node)
		{
			yield return null;
		}

		// Generate Tree node if it is available.
		public BSPTreeNode GenerateNode(IntVector2 size)
		{
			if (size > MinRoomSize)
				return new BSPTreeNode(size);
			return null;
		}
	}
}