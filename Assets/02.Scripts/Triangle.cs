using System;
using UnityEngine;
using System.Collections.Generic;

public class Triangle
{
	public List<Room> Rooms = new List<Room>();
	public List<Corridor> Corridors = new List<Corridor>();

	public Triangle(Room r1, Room r2, Room r3)
	{
		Rooms.Add(r1);
		Rooms.Add(r2);
		Rooms.Add(r3);

		Corridors.Add(Corridor.GetCorridor(r1, r2));
		Corridors[0].Triangles.Add(this);
		Corridors.Add(Corridor.GetCorridor(r2, r3));
		Corridors[1].Triangles.Add(this);
		Corridors.Add(Corridor.GetCorridor(r3, r1));
		Corridors[2].Triangles.Add(this);
	}

	/// <summary>
	/// Is in circumcircle
	/// </summary>
	/// <param name="room"> The point that be checked </param>
	/// <returns> True if the point is in this triangle's circumcircle </returns>
	public bool IsContaining(Room room)
	{
		Vector3[] vertexs = new Vector3[3];
		for (int index = 0; index < Rooms.Count; index++)
		{
			vertexs[index] = Rooms[index].transform.localPosition;
		}

		float a = vertexs[1].x - vertexs[0].x;
		float b = vertexs[1].z - vertexs[0].z;
		float c = vertexs[2].x - vertexs[0].x;
		float d = vertexs[2].z - vertexs[0].z;

		float aux1 = a * (vertexs[0].x + vertexs[1].x) + b * (vertexs[0].z + vertexs[1].z);
		float aux2 = c * (vertexs[0].x + vertexs[2].x) + d * (vertexs[0].z + vertexs[2].z);
		float div = 2.0f * (a * (vertexs[2].z - vertexs[1].z) - b * (vertexs[2].x - vertexs[1].x));

		if (Math.Abs(div) < float.Epsilon)
		{
			Debug.LogError("Divided by Zero : " + div);
			return false;
		}

		Vector3 center = new Vector3((d * aux1 - b * aux2) / div, 0, (a * aux2 - c * aux1) / div);
		float radius = Mathf.Sqrt((center.x - vertexs[0].x) * (center.x - vertexs[0].x) + (center.z - vertexs[0].z) * (center.z - vertexs[0].z));

		if (Vector3.Distance(room.transform.localPosition, center) > radius)
		{
			return false;
		}

		return true;
	}


	public void Show()
	{
		foreach (Corridor corridor in Corridors)
		{
			corridor.Show();
		}
	}
}
