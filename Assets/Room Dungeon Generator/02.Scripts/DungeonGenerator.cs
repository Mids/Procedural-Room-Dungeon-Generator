using System.Collections;
using System.Collections.Generic;
using ooparts.dungen.Dungeons;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	public TileDungeon2D[] TileDungeon2Ds;

	// Use this for initialization
	void Start()
	{
		Instantiate(TileDungeon2Ds[0]);
	}

	// Update is called once per frame
	void Update()
	{
	}
}