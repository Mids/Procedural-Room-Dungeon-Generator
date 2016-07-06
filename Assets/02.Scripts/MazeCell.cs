using UnityEngine;
using System.Collections;

public class MazeCell : MonoBehaviour {

    public IntVector2 coordinates;

	private int initializedEdgeCount;

	public MazeRoom room;

	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

	public MazeCellEdge GetEdge(MazeDirection direction)
	{
		return edges[(int) direction];
	}

	public void SetEdge(MazeDirection direction, MazeCellEdge edge)
	{
		edges[(int) direction] = edge;
		initializedEdgeCount += 1;
	}

	public bool IsFullyInitialized
	{
		get { return initializedEdgeCount == MazeDirections.Count; }
	}

	public MazeDirection RandomUninitalizedDirection
	{
		get
		{
			int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
			for (int i = 0; i < MazeDirections.Count; i++)
			{
				if (edges[i] == null)
				{
					if (skips == 0)
					{
						return (MazeDirection) i;
					}
					skips -= 1;
				}
			}

			throw new System.InvalidOperationException("MazeCell has no uninitialized direction left.");
		}
	}

	public void Initialize(MazeRoom room)
	{
		room.Add(this);
		transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
	}
}
