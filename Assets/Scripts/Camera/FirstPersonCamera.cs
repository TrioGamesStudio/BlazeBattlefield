using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : LocalCameraBase
{
    private Transform target;
    public float MouseSentivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private float mouseX;
    private float mouseY;
    private Vector2 lookInput;
    private void LateUpdate()
    {
  
        if (target == null)
        {
            return;
        }
       

        transform.position = target.position;
        lookInput.y = Mathf.Clamp(lookInput.y, -70f, 70f);
        transform.rotation = Quaternion.Euler(-lookInput.y, lookInput.x, 0);
        //target.transform.rotation = Quaternion.Euler(0, lookInput.x, 0);
        if (target != null)
            target.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    public override void Looking(Vector2 direction)
    {
        lookInput += direction * MouseSentivity;
    }

    public override void RegisterInput(InputReader inputReader)
    {
        inputReader.LookAction += Looking;
    }
    public override void UnRegisterInput(InputReader inputReader)
    {
        inputReader.LookAction -= Looking;
    }
    public override void SetTarget(Transform target)
    {
        this.target = target;
    }


}
public abstract class LocalCameraBase : MonoBehaviour, ISetupInput, IPlayerCamera
{
    public abstract void Looking(Vector2 direction);
    public abstract void SetTarget(Transform target);
    public abstract void RegisterInput(InputReader inputReader);
    public abstract void UnRegisterInput(InputReader inputReader);
}