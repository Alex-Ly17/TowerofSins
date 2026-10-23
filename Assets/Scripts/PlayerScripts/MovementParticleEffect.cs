using UnityEngine;

public class MovementParticleEffect : MonoBehaviour
{
    public GameObject dustPrefab;
    public Transform feet;
    public float speedThreshold = 3f;
    Rigidbody rb;
    public void Start()
    {
        // connect rigid body
        rb = GetComponent<Rigidbody>();
    }

    public void DustParticle()
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
