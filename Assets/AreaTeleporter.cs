using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, West, East, South };

public class AreaTeleporter : MonoBehaviour {
    
    public Direction direction;
    public AreaTeleporter destination;
    public Area currentArea;
    public bool isUsable;

    public AreaTeleporter(Direction direction, Area currentArea)
    {
        this.isUsable = true;
        this.direction = direction;
        this.currentArea = currentArea;

    }
    public void Set(Direction direction, Area currentArea)
    {
        this.isUsable = true;
        this.direction = direction;
        this.currentArea = currentArea;

    }
    public void SetDestination(AreaTeleporter destination)
    {
        this.destination = destination;
    }
	void OnTriggerEnter(Collider other)
    {
        if (!this.isUsable) return;
        destination.isUsable = false;
        Debug.Log("IN");
        Vector3 newPosition = other.transform.position;
        switch (direction)
        {
            case Direction.North:
                newPosition.z = destination.transform.position.z;
                break;
            case Direction.South:
                newPosition.z = destination.transform.position.z;
                break;
            case Direction.East:
                newPosition.x = destination.transform.position.x;
                break;
            case Direction.West:
                newPosition.x = destination.transform.position.x;
                break;
        }
        other.transform.position = newPosition;
        
        //Set current area of teleported object to current object of teleporter
    }
    void OnTriggerExit()
    {
        Debug.Log("OUT");
        this.isUsable = true;
    }
}
