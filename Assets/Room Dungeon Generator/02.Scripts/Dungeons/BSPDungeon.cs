using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ooparts.dungen.Dungeons
{
	public class BSPDungeon : Dungeon
	{
		public IntVector2 MapSize;
		public IntVector2 MinRoomSize;

		private IntVector2 _minPartitionSize;
		private BSPTreeNode _rootNode;

		private void Start()
		{
			// Partition Size is 1 tile larger than room size for padding
			_rootNode = GenerateNode(MapSize + 2, IntVector2.Zero - 1);
			_minPartitionSize = MinRoomSize + 2;
			SplitPartition(_rootNode);
		}

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
		// Succeeded - Split them recursively and return true
		// Failed - Generate Room and return false
		public bool SplitPartition(BSPTreeNode node)
		{
			if (node.PartitionSize.x > node.PartitionSize.z && node.PartitionSize.x > _minPartitionSize.x * 2)
			{
				// Split X
				int gap = node.PartitionSize.x - (_minPartitionSize.x * 2);
				Assert.IsTrue(gap > 0, "Gap must be positive");
				int rInt = Random.Range(0, gap);

				// LNode
				IntVector2 LNodeSize = new IntVector2(_minPartitionSize.x + rInt, node.PartitionSize.z);
				IntVector2 LNodeCoordinates = node.Coordinates;
				node.LNode = GenerateNode(LNodeSize, LNodeCoordinates);
				Assert.IsNotNull(node.LNode, "LNode is not generated");

				// RNode
				IntVector2 RNodeSize = new IntVector2(_minPartitionSize.x + gap - rInt, node.PartitionSize.z);
				IntVector2 RNodeCoordinates = new IntVector2(node.Coordinates.x + LNodeSize.x, node.Coordinates.z);
				node.RNode = GenerateNode(RNodeSize, RNodeCoordinates);
				Assert.IsNotNull(node.RNode, "RNode is not generated");

				return true;
			}
			else if (node.PartitionSize.z > _minPartitionSize.z * 2)
			{
				// Split Z
				int gap = node.PartitionSize.z - (MinRoomSize.z * 2 + 4);
				Assert.IsTrue(gap > 0, "Gap must be positive");
				int rInt = Random.Range(0, gap);

				// LNode
				IntVector2 LNodeSize = new IntVector2(_minPartitionSize.x, node.PartitionSize.z + rInt);
				IntVector2 LNodeCoordinates = node.Coordinates;
				node.LNode = GenerateNode(LNodeSize, LNodeCoordinates);
				Assert.IsNotNull(node.LNode, "LNode is not generated");

				// RNode
				IntVector2 RNodeSize = new IntVector2(_minPartitionSize.x, node.PartitionSize.z + gap - rInt);
				IntVector2 RNodeCoordinates = new IntVector2(node.Coordinates.x, node.Coordinates.z + LNodeSize.z);
				node.RNode = GenerateNode(RNodeSize, RNodeCoordinates);
				Assert.IsNotNull(node.RNode, "RNode is not generated");

				return true;
			}

			// Generate Room if the partition cannot be split.

			return false;
		}

		// Generate Tree node if it is available.
		private BSPTreeNode GenerateNode(IntVector2 size, IntVector2 coordinate)
		{
			return size > MinRoomSize ? new BSPTreeNode(size, coordinate) : null;
		}
	}
}