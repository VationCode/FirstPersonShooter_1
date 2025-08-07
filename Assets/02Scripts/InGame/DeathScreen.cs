using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public Image TargetImage;
    public TextMeshProUGUI TargetText;
    public float Duration = 5f;
    public bool IsShowDeadScreen = false;
    private float m_targetAplha = 1f;
    private float m_startAlpha;
    private float m_elapsedTime = 0f;   //경과시간

    private void Start()
    {
        m_startAlpha = 0;
        Color _startColorAlpha = TargetImage.color;
        _startColorAlpha.a = 0;
        TargetImage.color = _startColorAlpha;

        Color _startTextAlpha = TargetText.color;
        _startTextAlpha.a = 0;
        TargetText.color = _startTextAlpha;

        m_startAlpha = TargetImage.color.a;
    }

    private void Update()
    {
        Fade();
    }

    private void Fade()
    {
        if(IsShowDeadScreen)
        {
            if (m_elapsedTime < Duration)
            {
                float _newAlpha = Mathf.Lerp(m_startAlpha, m_targetAplha, m_elapsedTime/Duration);
                // Image Fade
                Color _newColor = TargetImage.color;
                _newColor.a = _newAlpha;
                TargetImage.color = _newColor;

                // Text Fade
                Color _newTextAlpha = TargetText.color;
                _newTextAlpha.a = _newAlpha;
                TargetText.color = _newTextAlpha;

                m_elapsedTime += Time.deltaTime;
            }

            else
            {
                Time.timeScale = 0f;
            }
        }
    }
}
