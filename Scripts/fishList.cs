using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/fishList", order = 1)]

public class fishList : ScriptableObject
{
    [SerializeField] private GameObject[] fishes;
    [SerializeField] private double[] percents;
    [SerializeField] private AudioClip[] clips;

    public double[] GetPercents() { return percents; }

    public GameObject[] GetFishes() { return fishes; }

    public AudioClip[] GetClips() { return clips; }
}