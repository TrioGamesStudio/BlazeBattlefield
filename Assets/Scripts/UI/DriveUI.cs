using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriveUI : MonoBehaviour
{
    [SerializeField] private Vector2 moveDirection;
    private float moveX,moveY;
    private float leftSide = 0;
    private float rightSide = 0;
    private float backSide = 0;
    private float fowardSide = 0;

    public void SetTrigger(DriveDirection driveDirection,bool isTrigger)
    {
        switch (driveDirection)
        {
            case DriveDirection.Foward:
                fowardSide = isTrigger ? 1 : 0;
                break;
            case DriveDirection.Back:
                backSide = isTrigger ? -1 : 0;
                break;
            case DriveDirection.Left:
                leftSide = isTrigger ? -1 : 0;
                break;
            case DriveDirection.Right:
                rightSide = isTrigger ? 1 : 0;
                break;
        }
        Debug.Log($"Set DriveDirection: {driveDirection.ToString()} to trigger state {isTrigger}");
    }

    private void Update()
    {
        moveDirection = new Vector2(fowardSide + backSide, leftSide + rightSide);
    }

  
}

public enum DriveDirection
{
    Foward,
    Back,
    Left,
    Right
}