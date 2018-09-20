using System;
using UnityEngine;

namespace ooparts.dungen
{
	[Serializable]
	public struct IntVector2
	{
		public int x, z;

		public static IntVector2 Zero = new IntVector2(0, 0);

		public IntVector2(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public static IntVector2 operator +(IntVector2 a, IntVector2 b)
		{
			a.x += b.x;
			a.z += b.z;
			return a;
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b)
		{
			a.x -= b.x;
			a.z -= b.z;
			return a;
		}

		public static Vector3 operator +(Vector3 a, IntVector2 b)
		{
			a.x += b.x;
			a.z += b.z;
			return a;
		}

		public static bool operator >(IntVector2 a, IntVector2 b)
		{
			return a.x > b.x && a.z > b.z;
		}

		public static bool operator <(IntVector2 a, IntVector2 b)
		{
			return a.x < b.x && a.z < b.z;
		}

		public static IntVector2 operator +(IntVector2 a, int b)
		{
			IntVector2 result;
			result.x = a.x + b;
			result.z = a.z + b;
			return result;
		}

		public static IntVector2 operator -(IntVector2 a, int b)
		{
			IntVector2 result;
			result.x = a.x - b;
			result.z = a.z - b;
			return result;
		}

		public static IntVector2 operator *(IntVector2 a, int b)
		{
			IntVector2 result;
			result.x = a.x * b;
			result.z = a.z * b;
			return result;
		}

		public static IntVector2 operator /(IntVector2 a, int b)
		{
			IntVector2 result;
			result.x = a.x / b;
			result.z = a.z / b;
			return result;
		}

		public Vector3 GetVector3()
		{
			return new Vector3(x, 0, z);
		}
	}
}