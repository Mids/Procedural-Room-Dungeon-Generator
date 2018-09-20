using UnityEngine;

namespace ooparts.dungen
{
	public class RoomMapManager : MonoBehaviour
	{
		public static int TileSize = 1;
		public int TileSizeFactor = 1;

		public Map mapPrefap;

		public int MapSizeX;
		public int MapSizeZ;
		public int MaxRooms;
		public int MaxRoomSize;
		public int MinRoomSize;

		private Map mapInstance;

		private void Start()
		{
			TileSize = TileSizeFactor;
			BeginGame();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space)) RestartGame();
		}

		private void BeginGame()
		{
			mapInstance = Instantiate(mapPrefap);
			mapInstance.RoomCount = MaxRooms;
			mapInstance.MapSize = new IntVector2(MapSizeX, MapSizeZ);
			mapInstance.RoomSize.Min = MinRoomSize;
			mapInstance.RoomSize.Max = MaxRoomSize;
			TileSize = TileSizeFactor;

			StartCoroutine(mapInstance.Generate());
		}

		private void RestartGame()
		{
			StopAllCoroutines();
			Destroy(mapInstance.gameObject);
			BeginGame();
		}
	}
}