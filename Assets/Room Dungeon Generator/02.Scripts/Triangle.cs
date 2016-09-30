using System;
using UnityEngine;
using System.Collections.Generic;
using ooparts.dungen;

namespace ooparts.dungen
{
	public class Triangle
	{
		public List<Room> Rooms = new List<Room>();
		public List<Corridor> Corridors = new List<Corridor>();

		private Vector3 _circumcenter = Vector3.zero;
		private float _radius;

		public Triangle(Room r1, Room r2, Room r3)
		{
			Rooms.Add(r1);
			Rooms.Add(r2);
			Rooms.Add(r3);

			Corridors.Add(r1.CreateCorridor(r2));
			Corridors[0].Triangles.Add(this);
			Corridors.Add(r2.CreateCorridor(r3));
			Corridors[1].Triangles.Add(this);
			Corridors.Add(r3.CreateCorridor(r1));
			Corridors[2].Triangles.Add(this);
		}

		/// <summary>
		/// Is in circumcircle
		/// </summary>
		/// <param name="room"> The point that be checked </param>
		/// <returns> True if the point is in this triangle's circumcircle </returns>
		public bool IsContaining(Room room)
		{
			// Save calculated circumcenter.
			if (_circumcenter == Vector3.zero)
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

				_circumcenter = new Vector3((d * aux1 - b * aux2) / div, 0, (a * aux2 - c * aux1) / div);
				_radius = Mathf.Sqrt((_circumcenter.x - vertexs[0].x) * (_circumcenter.x - vertexs[0].x) + (_circumcenter.z - vertexs[0].z) * (_circumcenter.z - vertexs[0].z));
			}

			if (Vector3.Distance(room.transform.localPosition, _circumcenter) > _radius)
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
}