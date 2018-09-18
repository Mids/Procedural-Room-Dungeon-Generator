using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ooparts.dungen.Dungeons
{
	public class BSPDungeon : Dungeon
	{
		public IntVector2 MapSize;
		public IntVector2 MinRoomSize;
		public BSPRoom RoomPrefab;

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
		public void GenerateRoom(IntVector2 size, IntVector2 coordinates)
		{
			var room = Instantiate(RoomPrefab);
			room.Init(size, coordinates);
			room.Generate();
		}

		// March until connected to room or corridor
		public IEnumerator ConnectRoomsWithCorridors()
		{
			yield return null;
		}

		// Put root node to split
		// Succeeded - Split them recursively and return true
		// Failed - Generate Room and return false
		public void SplitPartition(BSPTreeNode node)
		{
			// The partition cannot be split.
			if (node.PartitionSize < _minPartitionSize * 2)
			{
				// Generate Random Room here.
				var roomSize = node.PartitionSize - 2;
				roomSize.x = Random.Range(MinRoomSize.x, roomSize.x);
				roomSize.z = Random.Range(MinRoomSize.z, roomSize.z);

				var minRoomCoordinates = node.Coordinates + 1;
				var maxRoomCoordinates = node.Coordinates + node.PartitionSize - roomSize - 1;
				var roomCoordinates = new IntVector2(
					Random.Range(minRoomCoordinates.x, maxRoomCoordinates.x),
					Random.Range(minRoomCoordinates.z, maxRoomCoordinates.z)
				);

				GenerateRoom(roomSize, roomCoordinates);

				return;
			}

			// Can be split
			if (node.PartitionSize.x > node.PartitionSize.z || node.PartitionSize.z < _minPartitionSize.z * 2)
			{
				// Split X
				var gap = node.PartitionSize.x - _minPartitionSize.x * 2;
				Assert.IsFalse(gap < 0, "Gap must be positive");
				var rInt = Random.Range(0, gap);

				// LNode
				var LNodeSize = new IntVector2(_minPartitionSize.x + rInt, node.PartitionSize.z);
				var LNodeCoordinates = node.Coordinates;
				node.LNode = GenerateNode(LNodeSize, LNodeCoordinates);
				Assert.IsNotNull(node.LNode, "LNode is not generated");

				// RNode
				var RNodeSize = new IntVector2(_minPartitionSize.x + gap - rInt, node.PartitionSize.z);
				var RNodeCoordinates = new IntVector2(node.Coordinates.x + LNodeSize.x, node.Coordinates.z);
				node.RNode = GenerateNode(RNodeSize, RNodeCoordinates);

				Assert.IsNotNull(node.RNode, "RNode is not generated");
			}
			else
			{
				// Split Z
				var gap = node.PartitionSize.z - _minPartitionSize.z * 2;

				Assert.IsFalse(gap < 0, "Gap must be positive");
				var rInt = Random.Range(0, gap);

				// LNode
				var LNodeSize = new IntVector2(node.PartitionSize.x, _minPartitionSize.z + rInt);
				var LNodeCoordinates = node.Coordinates;
				node.LNode = GenerateNode(LNodeSize, LNodeCoordinates);
				Assert.IsNotNull(node.LNode, "LNode is not generated");

				// RNode
				var RNodeSize = new IntVector2(node.PartitionSize.z, _minPartitionSize.z + gap - rInt);
				var RNodeCoordinates = new IntVector2(node.Coordinates.x, node.Coordinates.z + LNodeSize.z);
				node.RNode = GenerateNode(RNodeSize, RNodeCoordinates);
				Assert.IsNotNull(node.RNode, "RNode is not generated");
			}

			// Split child nodes
			SplitPartition(node.LNode);
			SplitPartition(node.RNode);
		}

		// Generate Tree node if it is available.
		private BSPTreeNode GenerateNode(IntVector2 size, IntVector2 coordinate)
		{
			return size > MinRoomSize ? new BSPTreeNode(size, coordinate) : null;
		}
	}
}