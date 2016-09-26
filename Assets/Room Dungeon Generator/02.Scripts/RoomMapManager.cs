using UnityEngine;
using System.Collections;

public class RoomMapManager : MonoBehaviour
{
	public Map mapPrefap;
	private Map mapInstance;

	public int MapSizeX;
	public int MapSizeZ;
	public int MaxRooms;
	public int MinRoomSize;
	public int MaxRoomSize;

	void Start()
	{
		BeginGame();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RestartGame();
		}
	}

	private void BeginGame()
	{
		mapInstance = Instantiate(mapPrefap);
		mapInstance.RoomCount = MaxRooms;
		mapInstance.MapSize = new IntVector2(MapSizeX, MapSizeZ);
		mapInstance.RoomSize.Min = MinRoomSize;
		mapInstance.RoomSize.Max = MaxRoomSize;

		StartCoroutine(mapInstance.Generate());
	}

	private void RestartGame()
	{
		StopAllCoroutines();
		Destroy(mapInstance.gameObject);
		BeginGame();
	}
}
