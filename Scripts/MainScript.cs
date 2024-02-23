using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YG;

public class MainScript : MonoBehaviour
{
    [Header("ECONOMICS")]
    [SerializeField] private ScriptMode _scriptMode = ScriptMode.Menu;

    [SerializeField] private float _time = 120;
    [SerializeField] private int _score = 0;

    [SerializeField] private Text timeText;
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject _endGamePanel;

    [Header("GAME")]

    [SerializeField] private List<FishController> fc;
    [SerializeField] private Transform mobilePoint;
    [SerializeField] private AudioSource sound;

    [Header("FISH SPAWN")]
    [SerializeField] private GameObject[] _spawners;
    [SerializeField] private fishList _fishList;
    [SerializeField] private Transform _despawnPosition;
    [SerializeField] private float[] _randTime;
    [Space]
    [SerializeField] private int _fish;

    int i, savedRecord;
    float[] tempTime;
    RandomSpawn rs;

    void Start()
    {
        Time.timeScale = 1;
        if (PlayerPrefs.HasKey("Record")) savedRecord = PlayerPrefs.GetInt("Record");
        savedRecord = YandexGame.savesData.localRecord;
        rs = new RandomSpawn(_fishList);
        tempTime = new float[_randTime.Length];
        for (int i = 0; i < tempTime.Length; i++)
            tempTime[i] = _randTime[i];

        if (Application.isMobilePlatform)
        {
            var cam = Camera.main.gameObject;
            cam.transform.position = mobilePoint.position;
            cam.transform.rotation = mobilePoint.rotation;
        }
    }

    void FixedUpdate()
    {
        if (_scriptMode == ScriptMode.Game)
            GameUpdate();
        else if (_scriptMode == ScriptMode.Menu)
            MenuUpdate();
    }

    private void GameUpdate()
    {
        _time -= Time.deltaTime;
        if (_time > 0)
        {
            int min = (int)_time / 60;
            int sec = (int)_time - min * 60;
            timeText.text = "" + min + " : " + sec;
            if (min < 10) timeText.text = " 0" + min + " : " + sec;
            if (sec < 10) timeText.text = " " + min + " : 0" + sec;
            if (sec < 10 && min < 10) timeText.text = " 0" + min + " : 0" + sec;
            MenuUpdate();
        }
        else if (_time < 0)
        {
            _endGamePanel.SetActive(true);
            if (_score > savedRecord)
            {
                YandexGame.savesData.localRecord = _score;
                YandexGame.NewLeaderboardScores("ScoreLead", _score);
                PlayerPrefs.SetInt("Record", _score);
            }

            PlayerPrefs.SetInt("Scores", PlayerPrefs.GetInt("Scores") + _score);
            YandexGame.savesData.sumScore += _score;
            YandexGame.SaveProgress();
            Time.timeScale = 0;
        }
    }

    private void MenuUpdate()
    {
        SpawnFish();
        try
        {
            foreach (var f in fc)
                if (f.gameObject.activeInHierarchy) f.UpdateFish();
        }
        catch { }
    }

    private void SpawnFish()
    {
        _randTime[i] -= Time.deltaTime;
        _fish = GameObject.FindGameObjectsWithTag("Respawn").Length;

        if (_fish < 5 && _randTime[i] <= 0)
        {
            int randomSpawn = Random.Range(0, _spawners.Length - 1);
            int randomFish = rs.RandomNumber();

            var F = Instantiate(_fishList.GetFishes()[randomFish], _spawners[randomSpawn].transform.position, _spawners[randomSpawn].transform.rotation);
            var fishc = F.GetComponent<FishController>();
            fishc.SetAudio(_fishList.GetClips()[randomFish]);

            fc.Add(F.GetComponent<FishController>());

            i = Random.Range(0, _randTime.Length - 1);
            _randTime[i] = tempTime[i];
        }

        try
        {
            if (fc.Count > _spawners.Length)
                foreach (var f in fc)
                    if (!f.gameObject.activeInHierarchy) DestroyFish(f);
        }
        catch { }
    }

    private void DestroyFish(FishController go)
    {
        fc.Remove(go);
        Destroy(go.gameObject);
    }

    public void AddScore(int sc)
    {
        _score += sc;
        scoreText.text = "" + _score;
    }
    
    public void PlayMusic(AudioClip clip)
    {
        sound.clip = clip;
        sound.Play();
    }

    public Transform GetDespawnPos() { return _despawnPosition; }

    public void NextScene(int i) { SceneManager.LoadScene(i); }
    
    public void PauseGame() { Time.timeScale = 0; }

    public void ResumeGame() { Time.timeScale = 1; }

    private enum ScriptMode
    {
        Menu,
        Game
    }
}