using UnityEngine;

namespace ooparts.dungen.Dungeons
{
	public enum TileType
	{
		Empty,
		Room,
		Corridor,
		Wall
	}

	public abstract class TileDungeon2D : MonoBehaviour
	{
		public int TileSize = 1;
		public IntVector2 MapSize;

		private TileType[,] _tileTypes;

		// Use this for initialization
		protected virtual void Start()
		{
			_tileTypes = new TileType[MapSize.x, MapSize.z];
		}

		// Update is called once per frame
		private void Update()
		{
		}

		public void SetTileType(IntVector2 coordinates, TileType tileType)
		{
			SetTileType(coordinates.x, coordinates.z, tileType);
		}

		public void SetTileType(int x, int z, TileType tileType)
		{
			_tileTypes[x, z] = tileType;
		}

		public TileType GetTileType(IntVector2 coordinates)
		{
			return GetTileType(coordinates.x, coordinates.z);
		}

		public TileType GetTileType(int x, int z)
		{
			return _tileTypes[x, z];
		}
	}
}