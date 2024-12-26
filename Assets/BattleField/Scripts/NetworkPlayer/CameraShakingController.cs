using UnityEngine;

public class CameraShakingController : MonoBehaviour
{
    [SerializeField] bool enable = true;
    [SerializeField, Range(0, 0.1f)] float amplitude = 0.015f; // 0.015f
    [SerializeField, Range(0, 30)] float frequency = 10; // 10f

    [SerializeField] Transform _camera = null;
    [SerializeField] Transform cameraHolder = null;
    [SerializeField] Transform localCamera;
    float toggleSpeed = 2.0f;
    Vector3 startPos;
    [SerializeField] CharacterController characterController;

    private void Awake() {
        startPos = _camera.localPosition;
    }

    private void Update() {
        if(!enable) return;

        CheckMotion();
        ResetPosition();
        //_camera.LookAt(FocusTarget());
        localCamera.LookAt(FocusTarget());
    }

    void PlayMotion(Vector3 motion){
        _camera.localPosition += motion;
    }

    void CheckMotion() {
        float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

        if(speed < toggleSpeed) return;
        if(!characterController.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    Vector3 FootStepMotion() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }


    void ResetPosition() {
        if(_camera.localPosition == startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, startPos, 1 * Time.deltaTime);
    }

    Vector3 FocusTarget() {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }

}
