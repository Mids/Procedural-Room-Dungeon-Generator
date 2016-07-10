using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public struct MinMax
{
	public int Min;
	public int Max;
}

public class Map : MonoBehaviour
{
	public Room RoomPrefab;
	public int RoomCount;
	public IntVector2 MapSize;
	public MinMax RoomSize;
	public float GenerationStepDelay;

	private List<Triangle> _triangulation;
	private List<Room> _rooms;
	private List<Corridor> _connectedCorridors;

	// Generate Rooms and Corridors
	public IEnumerator Generate()
	{
		_rooms = new List<Room>();

		// Generate Rooms
		for (int i = 0; i < RoomCount; i++)
		{
			Room roomInstance = CreateRoom();
			if (roomInstance == null)
			{
				RoomCount = _rooms.Count;
				Debug.Log("Cannot make every rooms!");
				Debug.Log("Created Rooms : " + RoomCount);
				break;
			}
			StartCoroutine(roomInstance.Generate());
			yield return null;
		}

		yield return BowyerWatson();

		yield return PrimMST();
		Debug.Log("Every Rooms are minimally connected");

		foreach (Corridor corridor in _connectedCorridors)
		{
			corridor.Show();
		}

		// TODO: Corridor
	}

	private Room CreateRoom()
	{
		Room newRoom = null;

		// Try as many as we can.
		for (int i = 0; i < MapSize.x * MapSize.z; i++)
		{
			IntVector2 size = new IntVector2(Random.Range(RoomSize.Min, RoomSize.Max + 1), Random.Range(RoomSize.Min, RoomSize.Max + 1));
			IntVector2 coordinates = new IntVector2(Random.Range(0, MapSize.x - size.x), Random.Range(0, MapSize.z - size.z));
			if (!IsOverlapped(size, coordinates))
			{
				newRoom = Instantiate(RoomPrefab);
				_rooms.Add(newRoom);
				newRoom.name = "Room " + _rooms.Count + " (" + coordinates.x + ", " + coordinates.z + ")";
				newRoom.Size = size;
				newRoom.Coordinates = coordinates;
				newRoom.transform.parent = transform;
				newRoom.transform.localPosition = new Vector3(coordinates.x - MapSize.x * 0.5f + size.x * 0.5f, 0, coordinates.z - MapSize.z * 0.5f + size.z * 0.5f);
				break;
			}
		}

		if (newRoom == null)
		{
			Debug.LogError("Too many rooms in map!! : " + _rooms.Count);
		}

		return newRoom;
	}

	public IntVector2 RandomCoordinates
	{
		get
		{
			return new IntVector2(Random.Range(0, MapSize.x), Random.Range(0, MapSize.z));
		}
	}

	private bool IsOverlapped(IntVector2 size, IntVector2 coordinates)
	{
		foreach (Room room in _rooms)
		{
			// Give a little space between two rooms
			if (Mathf.Abs(room.Coordinates.x - coordinates.x + (room.Size.x - size.x) * 0.5f) <= (room.Size.x + size.x) * 0.6f &&
				Mathf.Abs(room.Coordinates.z - coordinates.z + (room.Size.z - size.z) * 0.5f) <= (room.Size.z + size.z) * 0.6f)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAdjacent(Room room1, Room room2)
	{

		return false;
	}

	// Big enough to cover the map
	private Triangle LootTriangle
	{
		get
		{
			Vector3[] vertexs = new Vector3[] {
			new Vector3(MapSize.x * 2, 0, MapSize.z),
			new Vector3(-MapSize.x * 2, 0, MapSize.z),
			new Vector3(0, 0, -2 * MapSize.z)};

			Room[] tempRooms = new Room[3];
			for (int i = 0; i < 3; i++)
			{
				tempRooms[i] = Instantiate(RoomPrefab);
				tempRooms[i].transform.localPosition = vertexs[i];
				tempRooms[i].name = "Loot Room " + i;
			}

			return new Triangle(tempRooms[0], tempRooms[1], tempRooms[2]);
		}
	}

	private IEnumerator BowyerWatson()
	{
		_triangulation = new List<Triangle>();

		Triangle loot = LootTriangle;
		_triangulation.Add(loot);

		foreach (Room room in _rooms)
		{
			List<Triangle> badTriangles = new List<Triangle>();

			foreach (Triangle triangle in _triangulation)
			{
				if (triangle.IsContaining(room))
				{
					badTriangles.Add(triangle);
				}
			}

			List<Corridor> polygon = new List<Corridor>();
			foreach (Triangle badTriangle in badTriangles)
			{
				foreach (Corridor corridor in badTriangle.Corridors)
				{
					if (corridor.Triangles.Count == 1)
					{
						polygon.Add(corridor);
						corridor.Triangles.Remove(badTriangle);
						continue;
					}

					foreach (Triangle triangle in corridor.Triangles)
					{
						if (triangle == badTriangle)
						{
							continue;
						}

						// Delete Corridor which is between two bad triangles.
						if (badTriangles.Contains(triangle))
						{
							corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
							corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
						}
						else
						{
							polygon.Add(corridor);
						}
						break;
					}
				}
			}

			// Delete Bad Triangles
			for (int index = badTriangles.Count - 1; index >= 0; --index)
			{
				Triangle triangle = badTriangles[index];
				badTriangles.RemoveAt(index);
				_triangulation.Remove(triangle);
				foreach (Corridor corridor in triangle.Corridors)
				{
					corridor.Triangles.Remove(triangle);
				}
			}

			foreach (Corridor corridor in polygon)
			{
				// TODO: Edge sync
				Triangle newTriangle = new Triangle(corridor.Rooms[0], corridor.Rooms[1], room);
				_triangulation.Add(newTriangle);
			}

			yield return null;
		}

		for (int index = _triangulation.Count - 1; index >= 0; index--)
		{
			if (_triangulation[index].Rooms.Contains(loot.Rooms[0]) || _triangulation[index].Rooms.Contains(loot.Rooms[1]) ||
				_triangulation[index].Rooms.Contains(loot.Rooms[2]))
			{
				_triangulation.RemoveAt(index);
			}
		}

		foreach (Room room in loot.Rooms)
		{
			Destroy(room.gameObject);
		}
	}

	private IEnumerator PrimMST()
	{
		List<Room> connectedRooms = new List<Room>();
		_connectedCorridors = new List<Corridor>();

		connectedRooms.Add(_rooms[0]);

		while (connectedRooms.Count < _rooms.Count)
		{
			KeyValuePair<Room, Corridor> minLength = new KeyValuePair<Room, Corridor>();
			List<Corridor> deleteList = new List<Corridor>();

			foreach (Room room in connectedRooms)
			{
				foreach (KeyValuePair<Room, Corridor> pair in room.RoomCorridor)
				{
					if (connectedRooms.Contains(pair.Key))
					{
						if (!_connectedCorridors.Contains(pair.Value))
						{
							deleteList.Add(pair.Value);
						}
						continue;
					}
					
					if (minLength.Value == null || minLength.Value.Length > pair.Value.Length)
					{
						minLength = pair;
					}
				}

				for (int index = deleteList.Count - 1; index >= 0; index--)
				{
					Corridor corridor = deleteList[index];
					corridor.Rooms[0].RoomCorridor.Remove(corridor.Rooms[1]);
					corridor.Rooms[1].RoomCorridor.Remove(corridor.Rooms[0]);
					deleteList.RemoveAt(index);
				}
			}
			connectedRooms.Add(minLength.Key);
			_connectedCorridors.Add(minLength.Value);
			
			yield return null;
		}
	}
}
