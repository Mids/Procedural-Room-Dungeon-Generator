using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public IntVector2 Size;
	public IntVector2 Coordinates;

	private GameObject _tilesObject;
	public Tile TilePrefab;
	private Tile[,] _tiles;

	public Dictionary<Room, Corridor> RoomCorridor = new Dictionary<Room, Corridor>();


	public IEnumerator Generate()
	{
		_tilesObject = new GameObject("Tiles");
		_tilesObject.transform.parent = transform;
		_tilesObject.transform.localPosition = Vector3.zero;

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
		newTile.transform.parent = _tilesObject.transform;
		newTile.transform.localPosition = new Vector3(coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Size.z * 0.5f + 0.5f);
		return newTile;
	}
}
