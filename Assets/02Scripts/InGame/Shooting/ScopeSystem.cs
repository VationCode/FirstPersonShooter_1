using UnityEngine;

public class ScopeSystem : MonoBehaviour
{
    public GameObject PlayerUICanvas;
    public GameObject ScopeCanvas;
    public float ZoomMagnification = 2f;    // πË¿≤
    public Camera[] Cameras;
    private bool m_isScoped = false;
    private float m_originalFOV;

    private void Update()
    {
        if (GameManager.Instance.CurrentWeaponIndex == 1) Scope();
    }

    private void Scope()
    {
        // ¡‹ ƒ—∞Ì ≤Ù±‚
        if(Input.GetMouseButtonDown(1))
        {
            m_isScoped = !m_isScoped;
            ScopeCanvas.SetActive(m_isScoped);
            PlayerUICanvas.SetActive(!m_isScoped);
            if (m_isScoped)
            {
                Cameras[0].gameObject.SetActive(false);
                Cameras[1].gameObject.SetActive(true);

                m_originalFOV = Cameras[1].fieldOfView;
                Cameras[1].fieldOfView /= ZoomMagnification;
            }
            else
            {
                Cameras[0].gameObject.SetActive(true);
                Cameras[1].gameObject.SetActive(false);

                Cameras[1].fieldOfView = m_originalFOV;
            }
        }

        // ¡‹ πË¿≤
        if(m_isScoped)
        {
            float _zoom = Input.GetAxis("Mouse ScrollWheel");
            Cameras[1].fieldOfView -= _zoom * ZoomMagnification * 10f;
            Cameras[1].fieldOfView = Mathf.Clamp(Cameras[1].fieldOfView,10f, 80f);
        }
    }
}
