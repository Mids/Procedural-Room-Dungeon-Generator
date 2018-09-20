using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ooparts.dungen.Dungeons;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ooparts.dungen
{
	[Serializable]
	public struct MinMax
	{
		public int Min;
		public int Max;
	}

	public class Map : TileDungeon2D
	{
		[HideInInspector] public int RoomCount;
		public Room RoomPrefab;
		public RoomSetting[] RoomSettings;
		[HideInInspector] public MinMax RoomSize;

		private List<Corridor> _corridors;
		private bool _hasPlayer;
		private List<Room> _rooms;

		public IntVector2 RandomCoordinates
		{
			get { return new IntVector2(Random.Range(0, MapSize.x), Random.Range(0, MapSize.z)); }
		}

		// Big enough to cover the map
		private Triangle LootTriangle
		{
			get
			{
				Vector3[] vertexs =
				{
					RoomMapManager.TileSize * new Vector3(MapSize.x * 2, 0, MapSize.z),
					RoomMapManager.TileSize * new Vector3(-MapSize.x * 2, 0, MapSize.z),
					RoomMapManager.TileSize * new Vector3(0, 0, -2 * MapSize.z)
				};

				var tempRooms = new Room[3];
				for (var i = 0; i < 3; i++)
				{
					tempRooms[i] = Instantiate(RoomPrefab);
					tempRooms[i].transform.localPosition = vertexs[i];
					tempRooms[i].name = "Loot Room " + i;
					tempRooms[i].Init(this);
				}

				return new Triangle(tempRooms[0], tempRooms[1], tempRooms[2]);
			}
		}

		// Generate Rooms and Corridors
		public IEnumerator Generate()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			{
				_rooms = new List<Room>();

				// Generate Rooms
				for (var i = 0; i < RoomCount; i++)
				{
					var roomInstance = CreateRoom();
					if (roomInstance == null)
					{
						RoomCount = _rooms.Count;
						Debug.Log("Cannot make more rooms!");
						Debug.Log("Created Rooms : " + RoomCount);
						break;
					}

					roomInstance.Setting = RoomSettings[Random.Range(0, RoomSettings.Length)];
					StartCoroutine(roomInstance.Generate());

					// Generate Player or Monster
					if (_hasPlayer)
					{
						yield return roomInstance.CreateMonsters();
					}
					else
					{
						yield return roomInstance.CreatePlayer();
						_hasPlayer = true;
					}

					yield return null;
				}

				Debug.Log("Every rooms are generated");

				// Delaunay Triangulation
				yield return BowyerWatson();

				// Minimal Spanning Tree
				yield return PrimMST();
				Debug.Log("Every rooms are minimally connected");

				// Generate Corridors
				foreach (var corridor in _corridors)
				{
					StartCoroutine(corridor.Generate());
					yield return null;
				}

				Debug.Log("Every corridors are generated");

				// Generate Walls
				yield return WallCheck();
				foreach (var room in _rooms) yield return room.CreateWalls();

				foreach (var corridor in _corridors) yield return corridor.CreateWalls();

				Debug.Log("Every walls are generated");
			}

			stopwatch.Stop();
			print("Done in :" + stopwatch.ElapsedMilliseconds / 1000f + "s");
		}

		private IEnumerator WallCheck()
		{
			for (var x = 0; x < MapSize.x; x++)
			for (var z = 0; z < MapSize.z; z++)
				if (GetTileType(x, z) == TileType.Empty && IsWall(x, z))
					SetTileType(x, z, TileType.Wall);

			yield return null;
		}

		private bool IsWall(int x, int z)
		{
			for (var i = x - 1; i <= x + 1; i++)
			{
				if (i < 0 || i >= MapSize.x) continue;

				for (var j = z - 1; j <= z + 1; j++)
				{
					if (j < 0 || j >= MapSize.z || i == x && j == z) continue;

					if (GetTileType(i, j) == TileType.Room || GetTileType(i, j) == TileType.Corridor) return true;
				}
			}

			return false;
		}

		private Room CreateRoom()
		{
			Room newRoom = null;

			// Try as many as we can.
			for (var i = 0; i < RoomCount * RoomCount; i++)
			{
				var size = new IntVector2(Random.Range(RoomSize.Min, RoomSize.Max + 1), Random.Range(RoomSize.Min, RoomSize.Max + 1));
				var coordinates = new IntVector2(Random.Range(1, MapSize.x - size.x), Random.Range(1, MapSize.z - size.z));
				if (!IsOverlapped(size, coordinates))
				{
					newRoom = Instantiate(RoomPrefab);
					_rooms.Add(newRoom);
					newRoom.Num = _rooms.Count;
					newRoom.name = "Room " + newRoom.Num + " (" + coordinates.x + ", " + coordinates.z + ")";
					newRoom.Size = size;
					newRoom.Coordinates = coordinates;
					newRoom.transform.parent = transform;
					var position = CoordinatesToPosition(coordinates);
					position.x += size.x * 0.5f - 0.5f;
					position.z += size.z * 0.5f - 0.5f;
					position *= RoomMapManager.TileSize;
					newRoom.transform.localPosition = position;
					newRoom.Init(this);
					break;
				}
			}

			if (newRoom == null) Debug.LogError("Too many rooms in map!! : " + _rooms.Count);

			return newRoom;
		}

		private bool IsOverlapped(IntVector2 size, IntVector2 coordinates)
		{
			foreach (var room in _rooms)
				// Give a little space between two rooms
				if (Mathf.Abs(room.Coordinates.x - coordinates.x + (room.Size.x - size.x) * 0.5f) < (room.Size.x + size.x) * 0.7f &&
				    Mathf.Abs(room.Coordinates.z - coordinates.z + (room.Size.z - size.z) * 0.5f) < (room.Size.z + size.z) * 0.7f)
					return true;

			return false;
		}

		private IEnumerator BowyerWatson()
		{
			var triangulation = new List<Triangle>();

			var loot = LootTriangle;
			triangulation.Add(loot);

			foreach (var room in _rooms)
			{
				var badTriangles = new List<Triangle>();

				foreach (var triangle in triangulation)
					if (triangle.IsContaining(room))
						badTriangles.Add(triangle);

				var polygon = new List<Corridor>();
				foreach (var badTriangle in badTriangles)
				foreach (var corridor in badTriangle.Corridors)
				{
					if (corridor.Triangles.Count == 1)
					{
						polygon.Add(corridor);
						corridor.Triangles.Remove(badTriangle);
						continue;
					}

					foreach (var triangle in corridor.Triangles)
					{
						if (triangle == badTriangle) continue;

						// Delete Corridor which is between two bad triangles.
						if (badTriangles.Contains(triangle))
						{
							corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
							corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
							Destroy(corridor.gameObject);
						}
						else
						{
							polygon.Add(corridor);
						}

						break;
					}
				}

				// Delete Bad Triangles
				for (var index = badTriangles.Count - 1; index >= 0; --index)
				{
					var triangle = badTriangles[index];
					badTriangles.RemoveAt(index);
					triangulation.Remove(triangle);
					foreach (var corridor in triangle.Corridors) corridor.Triangles.Remove(triangle);
				}

				foreach (var corridor in polygon)
				{
					// TODO: Edge sync
					var newTriangle = new Triangle(corridor.Rooms[0], corridor.Rooms[1], room);
					triangulation.Add(newTriangle);
				}
			}

			yield return null;

			for (var index = triangulation.Count - 1; index >= 0; index--)
				if (triangulation[index].Rooms.Contains(loot.Rooms[0]) || triangulation[index].Rooms.Contains(loot.Rooms[1]) ||
				    triangulation[index].Rooms.Contains(loot.Rooms[2]))
					triangulation.RemoveAt(index);

			foreach (var room in loot.Rooms)
			{
				var deleteList = new List<Corridor>();
				foreach (var pair in room.RoomCorridor) deleteList.Add(pair.Value);

				for (var index = deleteList.Count - 1; index >= 0; index--)
				{
					var corridor = deleteList[index];
					corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
					corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
					Destroy(corridor.gameObject);
				}

				Destroy(room.gameObject);
			}
		}

		private IEnumerator PrimMST()
		{
			var connectedRooms = new List<Room>();
			_corridors = new List<Corridor>();

			connectedRooms.Add(_rooms[0]);

			while (connectedRooms.Count < _rooms.Count)
			{
				var minLength = new KeyValuePair<Room, Corridor>();
				var deleteList = new List<Corridor>();

				foreach (var room in connectedRooms)
				foreach (var pair in room.RoomCorridor)
				{
					if (connectedRooms.Contains(pair.Key)) continue;

					if (minLength.Value == null || minLength.Value.Length > pair.Value.Length) minLength = pair;
				}

				// Check Unnecessary Corridors.
				foreach (var pair in minLength.Key.RoomCorridor)
					if (connectedRooms.Contains(pair.Key) && minLength.Value != pair.Value)
						deleteList.Add(pair.Value);

				// Delete corridors
				for (var index = deleteList.Count - 1; index >= 0; index--)
				{
					var corridor = deleteList[index];
					corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
					corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
					deleteList.RemoveAt(index);
					Destroy(corridor.gameObject);
				}

				connectedRooms.Add(minLength.Key);
				_corridors.Add(minLength.Value);
			}

			yield return null;
		}

		public Vector3 CoordinatesToPosition(IntVector2 coordinates)
		{
			return new Vector3(coordinates.x - MapSize.x * 0.5f + 0.5f, 0f, coordinates.z - MapSize.z * 0.5f + 0.5f);
		}
	}
}