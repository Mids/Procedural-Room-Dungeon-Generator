using System.Collections;
using System.Collections.Generic;
using ooparts.dungen.Dungeons;
using UnityEngine;

namespace ooparts.dungen
{
	public class Corridor : MonoBehaviour
	{
		public IntVector2 Coordinates; // Rooms[1].x , Rooms[0].z
		public float Length;
		public Room[] Rooms = new Room[2];
		public Tile TilePrefab;
		public List<Triangle> Triangles = new List<Triangle>();
		public GameObject WallPrefab;

		private Map _map;
		private List<Tile> _tiles;
		private GameObject _tilesObject;
		private GameObject _wallsObject;

		public void Init(Map map)
		{
			_map = map;
		}

		public IEnumerator Generate()
		{
			transform.localPosition *= RoomMapManager.TileSize;
			_tilesObject = new GameObject("Tiles");
			_tilesObject.transform.parent = transform;
			_tilesObject.transform.localPosition = Vector3.zero;

			// Separate Corridor to room
			MoveStickedCorridor();

			_tiles = new List<Tile>();
			var start = Rooms[0].Coordinates.x + Rooms[0].Size.x / 2;
			var end = Coordinates.x;
			if (start > end)
			{
				var temp = start;
				start = end;
				end = temp;
			}

			for (var i = start; i <= end; i++)
			{
				var newTile = CreateTile(new IntVector2(i, Coordinates.z));
				if (newTile) _tiles.Add(newTile);
			}

			start = Rooms[1].Coordinates.z + Rooms[1].Size.z / 2;
			end = Coordinates.z;
			if (start > end)
			{
				var temp = start;
				start = end;
				end = temp;
			}

			for (var i = start; i <= end; i++)
			{
				var newTile = CreateTile(new IntVector2(Coordinates.x, i));
				if (newTile) _tiles.Add(newTile);
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
			if (_map.GetTileType(coordinates) == TileType.Empty)
				_map.SetTileType(coordinates, TileType.Corridor);
			else
				return null;

			var newTile = Instantiate(TilePrefab);
			newTile.Coordinates = coordinates;
			newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
			newTile.transform.parent = _tilesObject.transform;
			newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - Coordinates.x + 0.5f, 0, coordinates.z - Coordinates.z + 0.5f);
			return newTile;
		}

		private void MoveStickedCorridor()
		{
			var correction = new IntVector2(0, 0);

			if (Rooms[0].Coordinates.x == Coordinates.x + 1)
				correction.x = 2;
			else if (Rooms[0].Coordinates.x + Rooms[0].Size.x == Coordinates.x)
				correction.x = -2;
			else if (Rooms[0].Coordinates.x == Coordinates.x)
				correction.x = 1;
			else if (Rooms[0].Coordinates.x + Rooms[0].Size.x == Coordinates.x + 1) correction.x = -1;


			if (Rooms[1].Coordinates.z == Coordinates.z + 1)
				correction.z = 2;
			else if (Rooms[1].Coordinates.z + Rooms[1].Size.z == Coordinates.z)
				correction.z = -2;
			else if (Rooms[1].Coordinates.z == Coordinates.z)
				correction.z = 1;
			else if (Rooms[1].Coordinates.z + Rooms[1].Size.z == Coordinates.z + 1) correction.z = -1;

			Coordinates += correction;
			transform.localPosition += RoomMapManager.TileSize * new Vector3(correction.x, 0f, correction.z);
		}

		public IEnumerator CreateWalls()
		{
			_wallsObject = new GameObject("Walls");
			_wallsObject.transform.parent = transform;
			_wallsObject.transform.localPosition = Vector3.zero;

			foreach (var tile in _tiles)
			foreach (var direction in MapDirections.Directions)
			{
				var coordinates = tile.Coordinates + direction.ToIntVector2();
				if (_map.GetTileType(coordinates) == TileType.Wall)
				{
					var newWall = Instantiate(WallPrefab);
					newWall.name = "Wall (" + coordinates.x + ", " + coordinates.z + ")";
					newWall.transform.parent = _wallsObject.transform;
					newWall.transform.localPosition = RoomMapManager.TileSize * _map.CoordinatesToPosition(coordinates) - transform.localPosition;
					newWall.transform.localRotation = direction.ToRotation();
					newWall.transform.localScale *= RoomMapManager.TileSize;
				}
			}

			yield return null;
		}
	}
}