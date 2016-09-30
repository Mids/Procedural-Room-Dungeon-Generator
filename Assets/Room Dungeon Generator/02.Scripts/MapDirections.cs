using UnityEngine;
using System.Collections;
using ooparts.dungen;

namespace ooparts.dungen
{
	public enum MapDirection
	{
		North,
		East,
		South,
		West
	}

	public static class MapDirections
	{
		public const int Count = 4;

		public static readonly MapDirection[] Directions =
		{
			MapDirection.North,
			MapDirection.East,
			MapDirection.South,
			MapDirection.West
		};

		private static readonly IntVector2[] Vectors =
		{
			new IntVector2(0, 1),
			new IntVector2(1, 0),
			new IntVector2(0, -1),
			new IntVector2(-1, 0),
		};

		public static IntVector2 ToIntVector2(this MapDirection direction)
		{
			return Vectors[(int) direction];
		}

		private static readonly MapDirection[] Opposites =
		{
			MapDirection.South,
			MapDirection.West,
			MapDirection.North,
			MapDirection.East
		};

		public static MapDirection GetOpposite(this MapDirection direction)
		{
			return Opposites[(int) direction];
		}

		private static readonly Quaternion[] Rotations =
		{
			Quaternion.identity,
			Quaternion.Euler(0f, 90f, 0f),
			Quaternion.Euler(0f, 180f, 0f),
			Quaternion.Euler(0f, 270f, 0f)
		};

		public static Quaternion ToRotation(this MapDirection direction)
		{
			return Rotations[(int) direction];
		}
	}
}