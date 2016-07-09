using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public IntVector2 Size;
	public IntVector2 Coordinates;

	GameObject tilesObject;
	public Tile TilePrefab;
	private Tile[,] _tiles;

	public List<Room> AdjacentRooms;


	public IEnumerator Generate()
	{
		AdjacentRooms = new List<Room>();

		tilesObject = new GameObject("Tiles");
		tilesObject.transform.parent = transform;
		tilesObject.transform.localPosition = Vector3.zero;

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
		newTile.transform.parent = tilesObject.transform;
		newTile.transform.localPosition = new Vector3(coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Size.z * 0.5f + 0.5f);
		return newTile;
	}

	public void CheckAdjacent(Room otherRoom, int maxLength)
	{
		IntVector2 minusCoordinates = otherRoom.Coordinates - Coordinates;
		IntVector2 minusSize = otherRoom.Size - Size;
		float x = Mathf.Abs(minusCoordinates.x + minusSize.x * 0.5f);
		float z = Mathf.Abs(minusCoordinates.z + minusSize.z * 0.5f);
		IntVector2 plusSize = otherRoom.Size + Size;
		float avgSizeX = plusSize.x * 0.5f;
		float avgSizeZ = plusSize.z * 0.5f;

		if (x < avgSizeX + maxLength &&
			z < avgSizeZ + maxLength && 
			(x < avgSizeX || z < avgSizeZ))
		{
			foreach (Room adjacentRoom in AdjacentRooms)
			{
				adjacentRoom.
			}
			AdjacentRooms.Add(otherRoom);
			otherRoom.AdjacentRooms.Add(this);
		}
	}
}
