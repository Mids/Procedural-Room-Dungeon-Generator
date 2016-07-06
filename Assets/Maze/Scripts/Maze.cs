using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{

    public IntVector2 size;

    public MazeCell cellPrefab;

    private MazeCell[,] cells;

    public float generationStepDelay;

	public MazePassage passagePrefab;
	public MazeWall[] wallPrefabs;
	public MazeDoor doorPrefab;

	public MazeRoomSettings[] roomSettings;
	private List<MazeRoom> rooms = new List<MazeRoom>();

	[Range(0f, 1f)] public float doorProbability;
	
	public MazeCell GetCell(IntVector2 coordinates)
	{
		return cells[coordinates.x, coordinates.z];
	}

    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[size.x, size.z];

	    List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);

        while (activeCells.Count > 0)
        {
	        yield return delay;
			Debug.Log("Loading.. Active Count : " + activeCells.Count);
			DoNextGenerationStep(activeCells);
	        if (activeCells.Count > 4)
			{
				DoNextGenerationStep(activeCells);
				DoNextGenerationStep(activeCells);
				DoNextGenerationStep(activeCells);
			}
		}
		Debug.Log("Done");

    }

	private void DoFirstGenerationStep(List<MazeCell> activeCells)
	{
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1));
		activeCells.Add(newCell);
	}

	private void DoNextGenerationStep(List<MazeCell> activeCells)
	{
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized)
		{
			activeCells.RemoveAt(currentIndex);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitalizedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
		if (ContainsCoordinates(coordinates))
		{
			MazeCell neighbor = GetCell(coordinates);
			if (neighbor == null)
			{
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex)
			{
				CreatePassageInSameRoom(currentCell, neighbor, direction);
			}
			else
			{
				CreateWall(currentCell, neighbor, direction);
			}
		}
		else
		{
			CreateWall(currentCell, null, direction);
		}
	}

	private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
	{
		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
		MazePassage passage = Instantiate(prefab);
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(prefab);
		if (passage is MazeDoor)
		{
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
		}
		else
		{
			otherCell.Initialize(cell.room);
		}
		passage.Initialize(otherCell, cell,direction.GetOpposite());
	}

	private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
	{
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]);
		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null)
		{
			wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]);
			wall.Initialize(otherCell,cell,direction.GetOpposite());
		}
	}

    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab);
        cells[coordinates.x, coordinates.z] = newCell;
	    newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
	    newCell.transform.localPosition = new Vector3(coordinates.x - size.x*0.5f + 0.5f, 0f,
		    coordinates.z - size.z*0.5f + 0.5f);
	    return newCell;
    }

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

	private MazeRoom CreateRoom(int indexToExclude)
	{
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude)
		{
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}

	private void CreatePassageInSameRoom(MazeCell cell, MazeCell otherCell, MazeDirection direction)
	{
		MazePassage passage = Instantiate(passagePrefab);
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab);
		passage.Initialize(otherCell, cell,direction.GetOpposite());
		if (cell.room != otherCell.room)
		{
			MazeRoom roomToAssimilate = otherCell.room;
			cell.room.Assimilate(roomToAssimilate);
			rooms.Remove(roomToAssimilate);
			Destroy(roomToAssimilate);
		}
	}
}
