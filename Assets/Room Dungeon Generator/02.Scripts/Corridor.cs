using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using ooparts.dungen;

namespace ooparts.dungen
{
	public class Corridor : MonoBehaviour
	{
		private GameObject _tilesObject;
		private GameObject _wallsObject;
		public Tile TilePrefab;
		public GameObject WallPrefab;

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
			transform.localPosition *= RoomMapManager.TileSize;
			_tilesObject = new GameObject("Tiles");
			_tilesObject.transform.parent = transform;
			_tilesObject.transform.localPosition = Vector3.zero;

			// Seperate Corridor to room
			MoveStickedCorridor();

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
			if (_map.GetTileType(coordinates) == TileType.Empty)
			{
				_map.SetTileType(coordinates, TileType.Corridor);
			}
			else
			{
				return null;
			}
			Tile newTile = Instantiate(TilePrefab);
			newTile.Coordinates = coordinates;
			newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
			newTile.transform.parent = _tilesObject.transform;
			newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - Coordinates.x + 0.5f, 0, coordinates.z - Coordinates.z + 0.5f);
			return newTile;
		}

		private void MoveStickedCorridor()
		{
			IntVector2 correction = new IntVector2(0, 0);

			if (Rooms[0].Coordinates.x == Coordinates.x + 1)
			{
				// left 2
				correction.x = 2;
			}
			else if (Rooms[0].Coordinates.x + Rooms[0].Size.x == Coordinates.x)
			{
				// right 2
				correction.x = -2;
			}
			else if (Rooms[0].Coordinates.x == Coordinates.x)
			{
				// left
				correction.x = 1;
			}
			else if (Rooms[0].Coordinates.x + Rooms[0].Size.x == Coordinates.x + 1)
			{
				// right
				correction.x = -1;
			}


			if (Rooms[1].Coordinates.z == Coordinates.z + 1)
			{
				// Bottom 2
				correction.z = 2;
			}
			else if (Rooms[1].Coordinates.z + Rooms[1].Size.z == Coordinates.z)
			{
				// Top 2
				correction.z = -2;
			}
			else if (Rooms[1].Coordinates.z == Coordinates.z)
			{
				// Bottom
				correction.z = 1;
			}
			else if (Rooms[1].Coordinates.z + Rooms[1].Size.z == Coordinates.z + 1)
			{
				// Top
				correction.z = -1;
			}

			Coordinates += correction;
			transform.localPosition += RoomMapManager.TileSize * new Vector3(correction.x, 0f, correction.z);
		}

		public IEnumerator CreateWalls()
		{
			_wallsObject = new GameObject("Walls");
			_wallsObject.transform.parent = transform;
			_wallsObject.transform.localPosition = Vector3.zero;

			foreach (Tile tile in _tiles)
			{
				foreach (MapDirection direction in MapDirections.Directions)
				{
					IntVector2 coordinates = tile.Coordinates + direction.ToIntVector2();
					if (_map.GetTileType(coordinates) == TileType.Wall)
					{
						GameObject newWall = Instantiate(WallPrefab);
						newWall.name = "Wall (" + coordinates.x + ", " + coordinates.z + ")";
						newWall.transform.parent = _wallsObject.transform;
						newWall.transform.localPosition = RoomMapManager.TileSize * _map.CoordinatesToPosition(coordinates) - transform.localPosition;
						newWall.transform.localRotation = direction.ToRotation();
						newWall.transform.localScale *= RoomMapManager.TileSize;
					}
				}
			}
			yield return null;
		}
	}
}