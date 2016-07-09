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


	private List<Room> _rooms;

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

			foreach (Room room in _rooms)
			{
				if (room != roomInstance)
				{
					roomInstance.CheckAdjacent(room, RoomSize.Min + 2);
				}
			}
		}

		// Show Adjacent Rooms
		foreach (Room room in _rooms)
		{
			foreach (Room adjacentRoom in room.AdjacentRooms)
			{
				Debug.DrawLine(room.transform.position, adjacentRoom.transform.position, Color.white, 10);
			}
			yield return null;
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
				newRoom.name = "Room " +_rooms.Count+" ("+ coordinates.x + ", " + coordinates.z + ")";
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
}
