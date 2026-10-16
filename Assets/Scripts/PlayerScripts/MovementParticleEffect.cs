using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    public GameObject dustPrefab;
    public Transform feet;
    public float speedThreshold = 3f;
    Rigidbody rb;
    void Start()
    {
        // connect rigid body
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // check method every frame
        DustParticle();
    }

    void DustParticle()
    {
        // if moving fast, create particle
        if (rb.linearVelocity.magnitude > speedThreshold)
        {
            // particle creation
            GameObject dustObject = Instantiate(dustPrefab, feet.position, Quaternion.identity);

            // particle removal
            Destroy(dustObject, 1.5f);
        }
    }
}
