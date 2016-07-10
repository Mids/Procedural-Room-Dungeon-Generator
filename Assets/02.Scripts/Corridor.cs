using UnityEngine;
using System.Collections.Generic;

public class Corridor
{
	public Room[] Rooms = new Room[2];
	public List<Triangle> Triangles = new List<Triangle>();

	public float Length { get; private set; }

	private Corridor(Room r1, Room r2)
	{
		Rooms[0] = r1;
		Rooms[1] = r2;
		Length = Vector3.Distance(r1.transform.localPosition, r2.transform.localPosition);
	}

	public static Corridor GetCorridor(Room r1, Room r2)
	{
		if (r1.RoomCorridor.ContainsKey(r2))
		{
			return r1.RoomCorridor[r2];
		}

		Corridor newCorridor = new Corridor(r1, r2);
		r1.RoomCorridor.Add(r2, newCorridor);
		r2.RoomCorridor.Add(r1, newCorridor);

		return newCorridor;
	}

	public void Show()
	{
		Debug.DrawLine(Rooms[0].transform.localPosition, Rooms[1].transform.localPosition, Color.white, 5f);
	}
}
