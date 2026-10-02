using UnityEngine;


public class DustSpawner : MonoBehaviour
{
    public GameObject DustExplosion;
    public Transform feet;
    public float speedThreshold = 3f;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (rb.linearVelocity.magnitude > speedThreshold)
        {
            Instantiate(DustExplosion, feet.position,
            Quaternion.identity);
        }
    }
}
