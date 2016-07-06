using UnityEngine;
using System.Collections;

public class MazeDoor : MazePassage
{

	public Transform hinge;

	private MazeDoor OtherSideOfDoor
	{
		get
		{
			return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
		}
	}

	public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
	{
		base.Initialize(cell, otherCell, direction);
		if (OtherSideOfDoor != null)
		{
			hinge.localScale = new Vector3(-1f, 1f, 1f);
			Vector3 p = hinge.localPosition;
			p.x = -p.x;
			hinge.localPosition = p;
		}
	}
}
