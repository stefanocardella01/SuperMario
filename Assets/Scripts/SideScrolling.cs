using UnityEngine;

public class SideScrolling : MonoBehaviour
{

    private Transform player;

    private void Awake()
    {

        player = GameObject.FindWithTag("Player").transform;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(player.position.x, cameraPosition.x);
        transform.position = cameraPosition;
    }
}
