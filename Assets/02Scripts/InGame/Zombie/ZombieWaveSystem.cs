using TMPro;
using UnityEngine;

public class ZombieWaveSystem : MonoBehaviour
{
    public GameObject[] ZombiePrefabs;
    public Transform[] SpawnPoints;
    public float TimeBetweenWaves = 10f;

    public int LastWaveNum = 10;
    public TextMeshProUGUI WaveNumTMP;
    [SerializeField]
    private float m_waveTimer = 0f;
    public TextMeshProUGUI WaveTimerTMP;
    private int m_waveNumver = 1;
    public int ZombiesPerWave = 4;  //한 웨이브당

    private void Start()
    {
        WaveTimerTMP.text = "10";
        WaveNumTMP.text = m_waveNumver.ToString() + " / " + LastWaveNum.ToString();
    }

    private void Update()
    {
        if (m_waveNumver == LastWaveNum) return;

        m_waveTimer += Time.deltaTime;
        int _intvalue = Mathf.RoundToInt(m_waveTimer);
        WaveTimerTMP.text = (TimeBetweenWaves - _intvalue).ToString();
        if (m_waveTimer >= TimeBetweenWaves)
        {
            StartNewWave();
        }
    }

    private void StartNewWave()
    {
        m_waveTimer = 0;
        ZombiesPerWave += 2;

        float _minDistance = 4f;

        for(int i = 0; i < ZombiesPerWave; i++)
        {
            int _randomSpawnIndex = Random.Range(0, SpawnPoints.Length);
            
            Transform _spawnPoints = SpawnPoints[_randomSpawnIndex];

            GameObject _randomZobiePrefab = ZombiePrefabs[Random.Range(0, SpawnPoints.Length)];

            // 겹침 방지
            Vector3 _spawnPosion = _spawnPoints.position + Random.insideUnitSphere * _minDistance;

            _spawnPosion.y = _spawnPoints.position.y;

            Instantiate(_randomZobiePrefab, _spawnPosion, _spawnPoints.rotation);
        }
        m_waveNumver++;
        WaveNumTMP.text = m_waveNumver.ToString() + " / " + LastWaveNum.ToString();
    }
}
