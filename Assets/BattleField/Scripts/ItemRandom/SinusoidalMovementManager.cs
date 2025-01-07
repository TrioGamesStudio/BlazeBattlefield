using System.Collections.Generic;
using UnityEngine;

public class SinusoidalMovementManager : MonoBehaviour
{
    public static SinusoidalMovementManager instance;

    [SerializeField] private List<SinusoidalMovement> sinusoidalMovements = new();
    [SerializeField] private bool allowUpdatePosition = true;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (allowUpdatePosition == false) return;

        foreach(var item in sinusoidalMovements)
        {
            item.UpdateSinPosition(Time.deltaTime);
        }
    }

    public void Register(GameObject movingObject)
    {
        if(movingObject.TryGetComponent(out SinusoidalMovement movement))
        {
            if(sinusoidalMovements.Contains(movement) == false)
            {
                sinusoidalMovements.Add(movement);
            }
        }
        else
        {
            NotifyNotValid(movingObject);
        }
    }

    public void UnRegister(GameObject movingObject)
    {
        if (movingObject.TryGetComponent(out SinusoidalMovement movement))
        {
            if (sinusoidalMovements.Contains(movement))
            {
                sinusoidalMovements.Remove(movement);
            }
        }
        else
        {
            NotifyNotValid(movingObject);
        }
    }
    private void NotifyNotValid(GameObject movingObject)
    {
        Debug.Log($"This object:{movingObject.name} not have SinusoidalMovement Component", movingObject.gameObject);
    }
   
}