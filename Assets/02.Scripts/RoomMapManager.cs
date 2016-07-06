using UnityEngine;
using System.Collections;

public class RoomMapManager : MonoBehaviour
{
	public Map mapPrefap;
	private Map mapInstance;

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
		StartCoroutine(mapInstance.Generate());
	}

	private void RestartGame()
	{
		StopAllCoroutines();
		Destroy(mapInstance.gameObject);
		BeginGame();
	}
}
