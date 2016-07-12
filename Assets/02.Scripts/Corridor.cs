using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Corridor : MonoBehaviour
{
	private GameObject _tilesObject;
	public Tile TilePrefab;

	public bool Activated = false;

	public Room[] Rooms = new Room[2];
	public List<Triangle> Triangles = new List<Triangle>();

	public float Length;
	public IntVector2 Coordinates; // Rooms[1].x , Rooms[0].z

	private Map _map;
	private List<Tile> _tiles;

	public void Init(Map map)
	{
		_map = map;
	}
	public IEnumerator Generate()
	{
		_tilesObject = new GameObject("Tiles");
		_tilesObject.transform.parent = transform;
		_tilesObject.transform.localPosition = Vector3.zero;

		_tiles = new List<Tile>();
		int start = Rooms[0].Coordinates.x + Rooms[0].Size.x / 2;
		int end = Coordinates.x;
		if (start > end)
		{
			int temp = start;
			start = end;
			end = temp;
		}
		for (int i = start; i <= end; i++)
		{
			Tile newTile = CreateTile(new IntVector2(i, Coordinates.z));
			if (newTile)
			{
				_tiles.Add(newTile);
			}
		}
		start = Rooms[1].Coordinates.z + Rooms[1].Size.z / 2;
		end = Coordinates.z;
		if (start > end)
		{
			int temp = start;
			start = end;
			end = temp;
		}
		for (int i = start; i <= end; i++)
		{
			Tile newTile = CreateTile(new IntVector2(Coordinates.x, i));
			if (newTile)
			{
				_tiles.Add(newTile);
			}
		}
		yield return null;
	}

	public void Show()
	{
		Debug.DrawLine(Rooms[0].transform.localPosition, transform.localPosition, Color.white, 3.5f);
		Debug.DrawLine(transform.localPosition, Rooms[1].transform.localPosition, Color.white, 3.5f);
	}

	private Tile CreateTile(IntVector2 coordinates)
	{
		if (_map.GetTileType(coordinates) == TileType.EMPTY)
		{
			_map.SetTileType(coordinates, TileType.CORRIDOR);
		}
		else
		{
			return null;
		}
		Tile newTile = Instantiate(TilePrefab);
		newTile.Coordinates = coordinates;
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
		newTile.transform.parent = _tilesObject.transform;
		newTile.transform.localPosition = new Vector3(coordinates.x - Coordinates.x + 0.5f, 0, coordinates.z - Coordinates.z + 0.5f);
		return newTile;
	}
}
