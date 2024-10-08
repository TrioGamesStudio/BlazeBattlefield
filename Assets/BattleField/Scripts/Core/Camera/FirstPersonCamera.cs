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
        Looking(lookingDir);
    }

    private Vector2 lookingDir;
    public override void Looking(Vector2 direction)
    {
        lookInput += direction * MouseSentivity;
    }

    private void SetDirection(Vector2 direction)
    {
        lookingDir = direction;
    }
    public override void RegisterInput()
    {
        InputPlayerMovement.LookAction += SetDirection;
        InputCombatControl.Attack += TriggerAttack;
    }

    private void TriggerAttack(bool b)
    {
        Debug.Log("On Trigger attack");

    }
    public override void UnRegisterInput()
    {
        InputPlayerMovement.LookAction -= SetDirection;
        InputCombatControl.Attack -= TriggerAttack;
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
    public abstract void RegisterInput();
    public abstract void UnRegisterInput();
}