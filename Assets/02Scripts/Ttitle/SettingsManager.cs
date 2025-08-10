using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
    public Button NormalDifficultyButton;
    public Button HardDifficultyButton;

    public Slider SmoothSpeedSlider;
    public Slider SensitivitySlider;

    public Button SaveButton;
    [SerializeField]
    private float m_timeBetweenWaves;
    [SerializeField]
    private int m_zombiePerWave;


    [SerializeField] 
    float m_smoothSpeed;
    [SerializeField] 
    float m_sensitivity;

    public Button Map1Button;
    public Button Map2Button;
    public Button ContinueButton;

    private string m_selectedMap;
    private void Awake()
    {
        LoadSettings();

        NormalDifficultyButton.onClick.AddListener(SetNormalDifficulty);
        HardDifficultyButton.onClick.AddListener(SetHardDifficulty);

        Map1Button.onClick.AddListener(() => SelectMap("InGame"));

        if(Map2Button != null)
            Map2Button.onClick.AddListener(() => SelectMap("InGame2"));

        ContinueButton.onClick.AddListener(LoadSelectedMap);

        SaveButton.onClick.AddListener(SaveSettings);
    }
    private void Start()
    {
        SmoothSpeedSlider.minValue = 1f;
        SmoothSpeedSlider.maxValue = 10f;
        m_smoothSpeed = 10f;

        SensitivitySlider.minValue = 1f;
        SensitivitySlider.maxValue = 10f;
        m_sensitivity = 2f;

        SetNormalDifficulty();
    }

    public void LoadSettings()
    {
        if(PlayerPrefs.HasKey("TimeBetweenWaves"))
        {
            m_timeBetweenWaves = PlayerPrefs.GetFloat("TimeBetweenWaves");
        }

        if(PlayerPrefs.HasKey("ZombiesPerWave"))
        {
            m_zombiePerWave = PlayerPrefs.GetInt("ZombiesPerWave");
        }

        if(PlayerPrefs.HasKey("SmoothSpeed"))
        {
            m_smoothSpeed = PlayerPrefs.GetFloat("SmoothSpeed");
            SmoothSpeedSlider.value = m_smoothSpeed;
        }

        if(PlayerPrefs.HasKey("Sensitivity"))
        {
            m_sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            SensitivitySlider.value = m_sensitivity;

        }
        if (PlayerPrefs.HasKey("SelectedMap"))
        {
            m_selectedMap = PlayerPrefs.GetString("SelectedMap");
        }
    }

    public void SaveSettings()
    {
        m_smoothSpeed = SmoothSpeedSlider.value;
        m_sensitivity = SensitivitySlider.value;

        PlayerPrefs.SetFloat("TimeBetweenWaves", m_timeBetweenWaves);
        PlayerPrefs.SetInt("ZombiesPerWave",m_zombiePerWave);
        PlayerPrefs.SetFloat("SmoothSpeed", m_smoothSpeed);
        PlayerPrefs.SetFloat("Sensitivity", m_sensitivity);

        PlayerPrefs.Save();
    }
    private void SetNormalDifficulty()
    {
        m_timeBetweenWaves = 10;
        m_zombiePerWave = 4;
    }

    private void SetHardDifficulty()
    {
        m_timeBetweenWaves = 7;
        m_zombiePerWave = 8;
    }

    private void SelectMap(string mapName)
    {
        m_selectedMap = mapName;

        PlayerPrefs.SetString("SelectedMap", m_selectedMap);
    }

    public void LoadSelectedMap()
    {
        SceneManager.LoadScene(m_selectedMap);
    }
}
