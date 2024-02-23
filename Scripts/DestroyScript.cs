using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    [SerializeField] private string Tag;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tag))  Destroy(gameObject); 
    }
}