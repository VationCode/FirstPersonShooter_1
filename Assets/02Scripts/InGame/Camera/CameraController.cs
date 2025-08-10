using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform;
    [Tooltip("마우스 감도")]
    public float Sensitivity = 1f;
    public float MinXAngle = -60f;
    public float MaxXAngle = 60f;
    public float SmoothSpeed = 30f;

    private static float m_rotationX = 0f;
    private static float m_rotationY = 0f;

    private void Start()
    {
        LoadSettings();
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

        Quaternion _targetRotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, SmoothSpeed * Time.deltaTime);
    }
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SmoothSpeed"))
        {
            SmoothSpeed = PlayerPrefs.GetFloat("SmoothSpeed");
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            Sensitivity = PlayerPrefs.GetFloat("Sensitivity");

        }
    }
}
