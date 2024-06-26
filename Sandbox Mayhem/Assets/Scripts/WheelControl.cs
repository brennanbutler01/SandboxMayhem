using UnityEngine;

public class WheelControl : MonoBehaviour
{
    Vector3 position;
    Quaternion rotation;
    
    [HideInInspector] public WheelCollider wheelCollider;
    public Transform wheelModel;
    public bool steerable;
    
    // Effects 
    public ParticleSystem sandParticlesPrefab;
    private ParticleSystem sandParticles;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();

        if (sandParticlesPrefab)
        {
            sandParticles = Instantiate(sandParticlesPrefab, transform, false);
        
            //0.15 is approximately wheel center
            sandParticles.transform.localPosition = new Vector3(0.15f, -wheelCollider.radius, 0);
            sandParticles.Stop();
        }
    }

    void Update()
    {
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;
        
        if (sandParticles)
        {
            if (wheelCollider.isGrounded && 
                Mathf.Abs(wheelCollider.rpm) > 30 &&
                wheelCollider.GetGroundHit(out var hit) && hit.collider.CompareTag("Terrain"))
            {
                float effectsRotation = wheelCollider.rpm > 0 ? -180 : 0;
                sandParticles.transform.localRotation = Quaternion.Euler(0, effectsRotation, 0);
                
                if (sandParticles.isStopped)
                {
                    sandParticles.Play();
                }
            }
            else 
            {
                if (sandParticles.isPlaying)
                {
                    var main = sandParticles.main;
                    main.simulationSpeed = 8f; // stop quickly
                    sandParticles.Stop();
                }
            }
        }
    }
}