using System.Collections;
using System.Collections.Generic;
using ooparts.dungen.Dungeons;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	public Dungeon[] Dungeons;

	// Use this for initialization
	void Start()
	{
		Instantiate(Dungeons[0]);
	}

	// Update is called once per frame
	void Update()
	{
	}
}