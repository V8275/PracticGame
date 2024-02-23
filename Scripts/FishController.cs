using UnityEngine;

public class FishController : MonoBehaviour
{
    [SerializeField] private float _speed = 0.02f;
    [SerializeField] private float _jumpForce = 0.5f;
    [SerializeField] private int _score = 1;
    [SerializeField] private AudioClip sound;
    [SerializeField] private GameObject _physModel;

    private MainScript _mc;
    private Transform _despPos;
    private Transform _physStart;

    void Awake()
    {
        _mc = FindObjectOfType<MainScript>().GetComponent<MainScript>();
        _despPos = _mc.GetDespawnPos();

        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.right);

        if (Physics.Raycast(ray, out hit, 10))
        {
            Debug.DrawRay(transform.position, -transform.right);
            var obj = hit.collider.gameObject;
            if (obj.CompareTag("Fish")) _speed = obj.GetComponent<FishController>().GetSpeed(); 
        }
    }

    public void UpdateFish()
    {
        transform.position = new Vector3(transform.position.x - _speed, transform.position.y, transform.position.z);
        if (transform.position.x < _despPos.position.x) gameObject.SetActive(false);
    }

    private void DestroyFish()
    {
        _physStart = GameObject.Find("SpawnPhys").transform;
        _mc.AddScore(_score);
        _mc.PlayMusic(sound);
        var fish = Instantiate(_physModel, _physStart.position, _physStart.rotation);
        fish.transform.parent = null;
        fish.GetComponent<Rigidbody>().AddForce(_physStart.forward * _jumpForce, ForceMode.Impulse);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) { if (other.gameObject.CompareTag("Player")) DestroyFish(); }

    public float GetSpeed() { return _speed; }

    public void SetAudio(AudioClip clip) { sound = clip; }
}