using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform;
    public float Sensitivity = 2f;
    public float MinXAngle = -30f;
    public float MaxXAngle = 30f;
    public float MinYAngle = -360f;
    public float MaxYAngle = 360f;
    public float SmoothSpeed = 10f;

    private float m_rotationX = 0f;
    private float m_rotationY = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotationCam();
    }

    private void RotationCam()
    {
        float _mouseX = Input.GetAxis("Mouse X") * Sensitivity;
        float _mouseY = Input.GetAxis("Mouse Y") * Sensitivity;

        m_rotationX -= _mouseY;
        m_rotationY += _mouseX;

        m_rotationX = Mathf.Clamp(m_rotationX, MinXAngle, MaxXAngle);
        m_rotationY = Mathf.Clamp(m_rotationY, MinYAngle, MaxYAngle);

        Quaternion _targetRotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);
        // TODO : 플레이어 컨트롤러로 옮기기
        PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, _targetRotation, SmoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, SmoothSpeed * Time.deltaTime);
    }
}
