using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _speed = 0.01f;
    [SerializeField] private Transform fishZone;

    [SerializeField] private LineRenderer rend;
    Transform player;
    Camera cam;

    Vector3 point;
    Touch touch;

    void Start()
    {
        player = gameObject.transform;
        cam = Camera.main;
        rend = FindObjectOfType<LineRenderer>().GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        Ray ray;
        RaycastHit hit;

        if (Application.isMobilePlatform)
        {
            touch = Input.GetTouch(0);
            ray = cam.ScreenPointToRay(touch.position);
        }
        else ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer != 2)
                point = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
        else point = player.position;

        player.position = Vector3.MoveTowards(player.position, point, _speed);

        float dist = Vector3.Distance(player.position, fishZone.position);
        float pr = (dist / 0.0099f);
        Color c = new Color(1f/100f * pr, 0, 0, 1);
        rend.material.color = c;
    }
}