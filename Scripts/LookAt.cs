using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform tr;

    void Update() { transform.LookAt(tr); }
}
