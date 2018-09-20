using UnityEngine;

namespace ooparts.dungen.Dungeons
{
	public class BSPRoom : MonoBehaviour
	{
		public RoomSetting Setting;
		public Tile TilePrefab;

		private IntVector2 _coordinates;
		private IntVector2 _size;
		private Tile[,] _tiles;
		private GameObject _tilesObject;
		private TileDungeon2D _dungeon;

		public void Init(IntVector2 size, IntVector2 coordinates, TileDungeon2D dungeon)
		{
			_size = size;
			_coordinates = coordinates;
			_dungeon = dungeon;
		}

		public void Generate()
		{
			transform.SetPositionAndRotation(new Vector3(_coordinates.x, 0, _coordinates.z), new Quaternion());
			// Create parent object
			_tilesObject = new GameObject("Tiles");
			_tilesObject.transform.parent = transform;
			_tilesObject.transform.localPosition = Vector3.zero;

			_tiles = new Tile[_size.x, _size.z];
			for (var x = 0; x < _size.x; x++)
			for (var z = 0; z < _size.z; z++)
				_tiles[x, z] = CreateTile(new IntVector2(_coordinates.x + x, _coordinates.z + z));
		}

		private Tile CreateTile(IntVector2 coordinates)
		{
			if (_dungeon.GetTileType(coordinates) == TileType.Empty)
				_dungeon.SetTileType(coordinates, TileType.Room);
			else
				Debug.LogError("Tile Conflict!");

			var newTile = Instantiate(TilePrefab);
			newTile.Coordinates = coordinates;
			newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
			newTile.transform.parent = _tilesObject.transform;
			newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - _coordinates.x, 0f, coordinates.z - _coordinates.z);
			newTile.transform.GetChild(0).GetComponent<Renderer>().material = Setting.floor;
			return newTile;
		}
	}
}