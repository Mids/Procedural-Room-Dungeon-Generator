using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public IntVector2 Size;
	public IntVector2 Coordinates;

	public Tile TilePrefab;
	private Tile[,] _tiles;

	public float MinLength; // Usually minimum size of the room

	public List<Room> AdjacentRooms;


	public IEnumerator Generate()
	{
		AdjacentRooms = new List<Room>();

		_tiles = new Tile[Size.x, Size.z];
		for (int x = 0; x < Size.x; x++)
		{
			for (int z = 0; z < Size.z; z++)
			{
				CreateTile(new IntVector2(x, z));
			}
		}
		yield return null;
	}

	private Tile CreateTile(IntVector2 coordinates)
	{
		Tile newTile = Instantiate(TilePrefab);
		_tiles[coordinates.x, coordinates.z] = newTile;
		newTile.Coordinates = coordinates;
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
		newTile.transform.parent = transform;
		newTile.transform.localPosition = new Vector3(coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Size.z * 0.5f + 0.5f);
		return newTile;
	}

	public void CheckAdjacent(Room otherRoom)
	{
		if (Mathf.Abs(otherRoom.Coordinates.x - Coordinates.x + (otherRoom.Size.x - Size.x) * 0.5f) <= (otherRoom.Size.x + Size.x) * 0.5f + MinLength &&
				Mathf.Abs(otherRoom.Coordinates.z - Coordinates.z + (otherRoom.Size.z - Size.z) * 0.5f) <= (otherRoom.Size.z + Size.z) * 0.5f + MinLength)
		{
			AdjacentRooms.Add(otherRoom);
			otherRoom.AdjacentRooms.Add(this);
		}
	}
}
