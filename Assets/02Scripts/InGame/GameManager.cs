using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Gun[] GunWeapons;

    public int HighScore;
    public int CurrentScore;

    public TextMeshProUGUI HighScoreTMP;
    public TextMeshProUGUI CurrentScoreTMP;
    [SerializeField]
    private int m_currentWeaponIndex = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SwitchWeapon(m_currentWeaponIndex);
    }

    private void Update()
    {
        Score();
        OnClickWeaponKey();
    }

    private void Score()
    {
        if(CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
        }
        //HighScoreTMP.text = HighScore.ToString();
        CurrentScoreTMP.text = CurrentScore.ToString();
    }

    private void OnClickWeaponKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
    }

    private void SwitchWeapon(int newIndex)
    {
        GunWeapons[m_currentWeaponIndex].gameObject.SetActive(false);
        GunWeapons[newIndex].gameObject.SetActive(true);
        m_currentWeaponIndex = newIndex;
    }
}
