using UnityEngine;
using UnityEngine.UI;
using YG;

public class MenuDataControl : MonoBehaviour
{
    [SerializeField] private int compareScores = 200;
    [SerializeField] private Text text, recordText;
    [SerializeField] private GameObject blockObject, musicOffObject, musicOnObject;
    [SerializeField] private Button but;

    private bool firstPlayScene = true, music = true;
    private int Scores = 0;

    private void Start()
    {
        YandexGame.LoadProgress();
        if (YandexGame.SDKEnabled)
        {
            Scores = YandexGame.savesData.sumScore;
            music = YandexGame.savesData.music;
        }
        else
        {
            if (PlayerPrefs.HasKey("Scores")) Scores = PlayerPrefs.GetInt("Scores");
            else PlayerPrefs.SetInt("Scores", 0);
        }
        ActivateSound(music);
    }

    private void ActivateSound(bool b)
    {
        musicOffObject.SetActive(!b);
        musicOnObject.SetActive(b);
        AudioListener.volume = b ? 0.5f : 0;

        if (YandexGame.SDKEnabled)
        {
            YandexGame.savesData.music = music;
            YandexGame.SaveProgress();
        }
    }

    public void SwitchSound()
    {
        music = !music;
        ActivateSound(music);
    }

    public void CheckLevel()
    {
        if (Scores >= compareScores)
        {
            blockObject.SetActive(false);
            but.interactable = true;
        } 
        else text.text = new string(Scores + "/" + compareScores);
    }

    public void LoadLeaders()
    {
        if (firstPlayScene)
        {
            if (YandexGame.initializedLB)
            {
                var lb = FindObjectOfType<LeaderboardYG>().GetComponent<LeaderboardYG>();
                lb.UpdateLB();
            }
            else
            {
                var l = FindObjectOfType<LeaderboardYG>().gameObject;
                l.SetActive(false);
                int rec = 0;
                if (PlayerPrefs.HasKey("Record")) rec = PlayerPrefs.GetInt("Record");
                recordText.text = "Рекорд: " + rec;
            }
            firstPlayScene = false;
        }
    }

    public void ExitGame() { Application.OpenURL("https://yandex.ru/games/?k50id=0100000026527301708_26527301708&yclid=17563281177987252223"); }
}