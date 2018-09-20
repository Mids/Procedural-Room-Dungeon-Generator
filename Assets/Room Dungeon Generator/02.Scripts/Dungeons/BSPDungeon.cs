using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ooparts.dungen.Dungeons
{
	public class BSPDungeon : TileDungeon2D
	{
		private IntVector2 _minPartitionSize;
		private BSPTreeNode _rootNode;
		public IntVector2 MinRoomSize;
		public int Padding = 1;
		public BSPRoom RoomPrefab;

		private void Start()
		{
			// Partition Size is 1 tile larger than room size for padding
			_rootNode = GenerateNode(MapSize + 2 * Padding, IntVector2.Zero - Padding);
			_minPartitionSize = MinRoomSize + 2 * Padding;
			StartCoroutine(SplitPartition(_rootNode));
		}

		// Generate Room for the Partition
		public void GenerateRoom(IntVector2 size, IntVector2 coordinates)
		{
			var room = Instantiate(RoomPrefab);
			room.Init(size, coordinates);
			room.Generate();
		}

		// Generate Corridor between two coordinates
		public void GenerateCorridor(IntVector2 left, IntVector2 right)
		{
		}

		// March until connected to room or corridor
		public IEnumerator ConnectRooms(BSPTreeNode node)
		{
			// Connect only if there are children
			if (node.LNode != null && node.RNode != null)
			{
				// Connect children first
				yield return ConnectRooms(node.LNode);
				yield return ConnectRooms(node.RNode);

				// 
				var LCenter = node.LNode.Coordinates + node.LNode.PartitionSize / 2;
				var RCenter = node.RNode.Coordinates + node.RNode.PartitionSize / 2;
			}

			yield return null;
		}

		// Put root node to split
		// Split them recursively or Generate Room
		public IEnumerator SplitPartition(BSPTreeNode node)
		{
			// The partition cannot be split.
			if (node.PartitionSize < _minPartitionSize * 2)
			{
				// Generate Random Room here.
				var roomSize = node.PartitionSize - 2 * Padding;
				roomSize.x = Random.Range(MinRoomSize.x, roomSize.x);
				roomSize.z = Random.Range(MinRoomSize.z, roomSize.z);

				var minRoomCoordinates = node.Coordinates + Padding;
				var maxRoomCoordinates = node.Coordinates + node.PartitionSize - roomSize - Padding;
				var roomCoordinates = new IntVector2(
					Random.Range(minRoomCoordinates.x, maxRoomCoordinates.x),
					Random.Range(minRoomCoordinates.z, maxRoomCoordinates.z)
				);

				GenerateRoom(roomSize, roomCoordinates);

				// For Debug
				var leftBot = node.Coordinates.GetVector3();
				var rightTop = (node.Coordinates + node.PartitionSize).GetVector3();
				var leftTop = new Vector3(leftBot.x, 0, rightTop.z);
				var rightBot = new Vector3(rightTop.x, 0, leftBot.z);
				Debug.DrawLine(leftBot, leftTop, Color.red, 3);
				Debug.DrawLine(leftTop, rightTop, Color.red, 3);
				Debug.DrawLine(rightTop, rightBot, Color.red, 3);
				Debug.DrawLine(rightBot, leftBot, Color.red, 3);

				yield return null;
			}
			else if (node.PartitionSize.x > node.PartitionSize.z || node.PartitionSize.z < _minPartitionSize.z * 2)
			{
				// Can be split
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

				// Split child nodes
				yield return SplitPartition(node.LNode);
				yield return SplitPartition(node.RNode);
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
				var RNodeSize = new IntVector2(node.PartitionSize.x, _minPartitionSize.z + gap - rInt);
				var RNodeCoordinates = new IntVector2(node.Coordinates.x, node.Coordinates.z + LNodeSize.z);
				node.RNode = GenerateNode(RNodeSize, RNodeCoordinates);
				Assert.IsNotNull(node.RNode, "RNode is not generated");

				// Split child nodes
				yield return SplitPartition(node.LNode);
				yield return SplitPartition(node.RNode);
			}
		}

		// Generate Tree node if it is available.
		private BSPTreeNode GenerateNode(IntVector2 size, IntVector2 coordinate)
		{
			return size > MinRoomSize ? new BSPTreeNode(size, coordinate) : null;
		}
	}
}