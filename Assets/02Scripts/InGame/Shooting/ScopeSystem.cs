using UnityEngine;

public class ScopeSystem : MonoBehaviour
{
    public GameObject PlayerUICanvas;
    public GameObject ScopeCanvas;
    public float ZoomMagnification = 2f;    // πË¿≤
    public Camera MainCamera;
    public Camera ScopeCamera;
    private bool m_isScoped = false;
    private float m_originalFOV;

    private void Update()
    {
        if (GameManager.Instance.Guns[1].gameObject.activeInHierarchy == true) Scope();
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
                MainCamera.gameObject.SetActive(false);
                ScopeCamera.gameObject.SetActive(true);

                ScopeCamera.enabled = true;
                m_originalFOV = ScopeCamera.fieldOfView;
                ScopeCamera.fieldOfView /= ZoomMagnification;
            }
            else
            {
                MainCamera.gameObject.SetActive(true);
                ScopeCamera.gameObject.SetActive(false);
                
                ScopeCamera.enabled = false;
                ScopeCamera.fieldOfView = m_originalFOV;
            }
        }

        // ¡‹ πË¿≤
        if(m_isScoped)
        {
            float _zoom = Input.GetAxis("Mouse ScrollWheel");
            ScopeCamera.fieldOfView -= _zoom * ZoomMagnification * 10f;
            ScopeCamera.fieldOfView = Mathf.Clamp(ScopeCamera.fieldOfView,10f, 80f);
        }
    }
}
