using ooparts.dungen;
using ooparts.dungen.Dungeons;
using UnityEngine;

public class BSPCorridor : MonoBehaviour
{
	public enum Orientation
	{
		Vertical,
		Horizontal
	}

	public int BroadFactor = 1; // TODO: How many tiles in width of the corridor
	public Tile TilePrefab;
	public RoomSetting Setting;

	private IntVector2 _coordinates;
	private IntVector2 _aCoordinate, _bCoordinate;
	private Orientation _orientation;
	private TileDungeon2D _dungeon;
	private Tile[] _tiles;
	private GameObject _tilesObject;

	public void Init(IntVector2 a, IntVector2 b, IntVector2 coordinates, TileDungeon2D dungeon)
	{
		if (Mathf.Abs(a.z - b.z) > Mathf.Abs(a.x - b.x))
		{
			// Vertical Corridor
			_orientation = Orientation.Vertical;

			// a is on bottom
			if (a.z > b.z)
			{
				_aCoordinate = b;
				_bCoordinate = a;
			}
			else
			{
				_aCoordinate = a;
				_bCoordinate = b;
			}
		}
		else
		{
			// Horizontal Corridor
			_orientation = Orientation.Horizontal;

			// b is on left
			if (a.x > b.x)
			{
				_aCoordinate = b;
				_bCoordinate = a;
			}
			else
			{
				_aCoordinate = a;
				_bCoordinate = b;
			}
		}

		_coordinates = coordinates;
		_dungeon = dungeon;
	}


	// Generate Straight Corridor
	public void GenerateStraight()
	{
		transform.SetPositionAndRotation(new Vector3(_aCoordinate.x, 0, _aCoordinate.z), new Quaternion());

		// Create parent object
		_tilesObject = new GameObject("Tiles");
		_tilesObject.transform.parent = transform;
		_tilesObject.transform.localPosition = Vector3.zero;


		if (_orientation == Orientation.Horizontal)
		{
			// Generate horizontal corridors
			var start = _aCoordinate.x;
			var end = _bCoordinate.x;

			while (true)
				// Move start and end to empty tile
				if (_dungeon.GetTileType(start, _aCoordinate.z) != TileType.Empty)
					start++;
				else if (_dungeon.GetTileType(end, _bCoordinate.z) != TileType.Empty)
					end--;
				else
					break;

			var numberOfTiles = end - start + 1;
			Debug.Log(numberOfTiles);
			_tiles = new Tile[numberOfTiles];

			for (var i = 0; i < numberOfTiles; i++) _tiles[i] = CreateTile(new IntVector2(start - _aCoordinate.x + i, 0));
		}
		else
		{
			// Generate vertical corridors
			var start = _aCoordinate.z;
			var end = _bCoordinate.z;

			while (true)
				// Move start and end to empty tile
				if (_dungeon.GetTileType(_aCoordinate.x, start) != TileType.Empty)
					start++;
				else if (_dungeon.GetTileType(_bCoordinate.x, end) != TileType.Empty)
					end--;
				else
					break;

			var numberOfTiles = end - start + 1;
			Debug.Log(numberOfTiles);
			_tiles = new Tile[numberOfTiles];

			for (var i = 0; i < numberOfTiles; i++) _tiles[i] = CreateTile(new IntVector2(0, start - _aCoordinate.z + i));
		}
	}

	private Tile CreateTile(IntVector2 coordinates)
	{
		if (_dungeon.GetTileType(coordinates) == TileType.Empty)
			_dungeon.SetTileType(coordinates, TileType.Corridor);
		else
			Debug.LogError("Tile Conflict!");

		var newTile = Instantiate(TilePrefab);
		newTile.Coordinates = coordinates;
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
		newTile.transform.parent = _tilesObject.transform;
		newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x, 0f, coordinates.z);
		newTile.transform.GetChild(0).GetComponent<Renderer>().material = Setting.floor;
		return newTile;
	}
}