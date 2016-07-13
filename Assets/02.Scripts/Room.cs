using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public Corridor CorridorPrefab;
	public IntVector2 Size;
	public IntVector2 Coordinates;
	public int Num;

	private GameObject _tilesObject;
	public Tile TilePrefab;
	private Tile[,] _tiles;

	public Dictionary<Room, Corridor> RoomCorridor = new Dictionary<Room, Corridor>();

	private Map _map;

	public void Init(Map map)
	{
		_map = map;
	}

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
				_tiles[x, z] = CreateTile(new IntVector2(Coordinates.x + x, Coordinates.z + z));
			}
		}
		yield return null;
	}

	private Tile CreateTile(IntVector2 coordinates)
	{
		if (_map.GetTileType(coordinates) == TileType.Empty)
		{
			_map.SetTileType(coordinates, TileType.Room);
		}
		else
		{
			Debug.LogError("Tile Conflict!");
		}
		Tile newTile = Instantiate(TilePrefab);
		newTile.Coordinates = coordinates;
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
		newTile.transform.parent = _tilesObject.transform;
		newTile.transform.localPosition = new Vector3(coordinates.x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Coordinates.z - Size.z * 0.5f + 0.5f);
		return newTile;
	}

	public Corridor CreateCorridor(Room otherRoom)
	{
		// Don't create if already connected
		if (RoomCorridor.ContainsKey(otherRoom))
		{
			return RoomCorridor[otherRoom];
		}

		Corridor newCorridor = Instantiate(CorridorPrefab);
		newCorridor.name = "Corridor (" + otherRoom.Num + ", " + Num + ")";
		newCorridor.transform.parent = transform.parent;
		newCorridor.Coordinates = new IntVector2(Coordinates.x + Size.x / 2, otherRoom.Coordinates.z + otherRoom.Size.z / 2);
		newCorridor.transform.localPosition = new Vector3(newCorridor.Coordinates.x - _map.MapSize.x / 2, 0, newCorridor.Coordinates.z - _map.MapSize.z / 2);
		newCorridor.Rooms[0] = otherRoom;
		newCorridor.Rooms[1] = this;
		newCorridor.Length = Vector3.Distance(otherRoom.transform.localPosition, transform.localPosition);
		newCorridor.Init(_map);
		otherRoom.RoomCorridor.Add(this, newCorridor);
		RoomCorridor.Add(otherRoom, newCorridor);

		return newCorridor;
	}
}
