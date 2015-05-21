using UnityEngine;
using System.Collections.Generic;

public class LevelCamera : MonoBehaviour
{
    /*
    //TODO use these values to create an area that the camera cannot move beyond
    public float LevelBoundNorthSouth;
    public float LevelBoundEastWest;
    public float LevelBoundUppr;
    public float LevelBoundDown;
    */
    public float PanSpeed = 10.0f;
    public float ZoomSpeed = 10.0f;

    //_currentRotation refers to the left edge
    private data.cardinalPoints _currentRotation = data.cardinalPoints.SOUTH;
    private Vector3 _southEastPosition = new Vector3(0,315,0);
    private Vector3 _westSouthPosition = new Vector3(0,45,0);
    private Vector3 _northWestPosition = new Vector3(0,135,0);
    private Vector3 _eastNorthPosition = new Vector3(0, 225, 0);

    public void PanCamera(data.cardinalPoints moveDirection)
    {
        if (moveDirection == data.cardinalPoints.NORTH)
        {
            transform.parent.position += transform.parent.forward * (PanSpeed * Time.deltaTime);
        }
        else if(moveDirection == data.cardinalPoints.SOUTH)
        {
            transform.parent.position += -(transform.parent.forward) * (PanSpeed * Time.deltaTime);
        }
        else if (moveDirection == data.cardinalPoints.EAST)
        {
            transform.parent.position += transform.parent.right * (PanSpeed * Time.deltaTime);
        }
        else if (moveDirection == data.cardinalPoints.WEST)
        {
            transform.parent.position += -(transform.parent.right) * (PanSpeed * Time.deltaTime);
        }
    }

    public void ZoomCamera(float axisMovement)
    {
        float direction = (axisMovement*ZoomSpeed);
        //transform.Translate(0, direction, 0);
        transform.position = transform.position+(transform.forward*direction);
    }

    public void RotateCamera(data.cardinalPoints leftEdge, data.cardinalPoints rightEdge)
    {
        if (leftEdge == data.cardinalPoints.SOUTH && rightEdge == data.cardinalPoints.EAST)
        {
            transform.parent.eulerAngles = _southEastPosition;
            _currentRotation = data.cardinalPoints.SOUTH;
        }
        else if (leftEdge == data.cardinalPoints.WEST && rightEdge == data.cardinalPoints.SOUTH)
        {
            transform.parent.eulerAngles = _westSouthPosition;
            _currentRotation = data.cardinalPoints.WEST;
        }
        else if (leftEdge == data.cardinalPoints.NORTH && rightEdge == data.cardinalPoints.WEST)
        {
            transform.parent.eulerAngles = _northWestPosition;
            _currentRotation = data.cardinalPoints.NORTH;
        }
        else if (leftEdge == data.cardinalPoints.EAST && rightEdge == data.cardinalPoints.NORTH)
        {
            transform.parent.eulerAngles = _eastNorthPosition;
            _currentRotation = data.cardinalPoints.EAST;
        }
        else
        {
            Debug.Log("Incorrect Camera Rotation");
        }
        Events.instance.Raise(new CameraChange(leftEdge, rightEdge));
    }

    public data.cardinalPoints GetCurrentRotation()
    {
        return _currentRotation;
    }
}