using UnityEngine;
using System.Collections;

public class LevelCamera : MonoBehaviour
{
    public float LevelBoundNorthSouth;
    public float LevelBoundEastWest;
    public float MoveSpeed;

    private data.cardinalPoints currentRotation = data.cardinalPoints.SOUTH;
    private Vector3 southEastPosition = new Vector3(0,315,0);
    private Vector3 westSouthPosition = new Vector3(0,45,0);
    private Vector3 northWestPosition = new Vector3(0,135,0);
    private Vector3 eastNorthPosition = new Vector3(0, 225, 0);

    public void PanCamera(data.cardinalPoints moveDirection)
    {
        if (moveDirection == data.cardinalPoints.NORTH)
        {
            transform.parent.position += transform.parent.forward * (MoveSpeed * Time.deltaTime);
        }
        else if(moveDirection == data.cardinalPoints.SOUTH)
        {
            transform.parent.position += -(transform.parent.forward) * (MoveSpeed * Time.deltaTime);
        }
        else if (moveDirection == data.cardinalPoints.EAST)
        {
            transform.parent.position += transform.parent.right * (MoveSpeed * Time.deltaTime);
        }
        else if (moveDirection == data.cardinalPoints.WEST)
        {
            transform.parent.position += -(transform.parent.right) * (MoveSpeed * Time.deltaTime);
        }
    }

    public void RotateCamera(data.cardinalPoints leftEdge, data.cardinalPoints rightEdge)
    {
        if (leftEdge == data.cardinalPoints.SOUTH && rightEdge == data.cardinalPoints.EAST)
        {
            transform.parent.eulerAngles = southEastPosition;
            currentRotation = data.cardinalPoints.SOUTH;
        }
        else if (leftEdge == data.cardinalPoints.WEST && rightEdge == data.cardinalPoints.SOUTH)
        {
            transform.parent.eulerAngles = westSouthPosition;
            currentRotation = data.cardinalPoints.WEST;
        }
        else if (leftEdge == data.cardinalPoints.NORTH && rightEdge == data.cardinalPoints.WEST)
        {
            transform.parent.eulerAngles = northWestPosition;
            currentRotation = data.cardinalPoints.NORTH;
        }
        else if (leftEdge == data.cardinalPoints.EAST && rightEdge == data.cardinalPoints.NORTH)
        {
            transform.parent.eulerAngles = eastNorthPosition;
            currentRotation = data.cardinalPoints.EAST;
        }
        else
        {
            Debug.Log("Incorrect Camera Rotation");
        }
    }

    public data.cardinalPoints GetCurrentRotation()
    {
        return currentRotation;
    }
}