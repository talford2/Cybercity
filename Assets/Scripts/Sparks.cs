using UnityEngine;

public class Sparks : MonoBehaviour
{
    public float Lifetime;
    public Light SparksLight;
    public float SparksLightIntensity;
    public AnimationCurve LightIntensityCurve;
    public float PitchUp = 45f;

    private float cooldown;

	void Start ()
	{
	    cooldown = Lifetime;
        transform.forward = Quaternion.AngleAxis(-PitchUp, transform.right) * transform.forward;
	}
	
	void Update () {
	    if (cooldown >= 0f)
	    {
	        cooldown -= Time.deltaTime;
	        var cooldownFraction = Mathf.Clamp01(cooldown/Lifetime);
            SparksLight.intensity = SparksLightIntensity * LightIntensityCurve.Evaluate(1f-cooldownFraction);
	        if (cooldown < 0f)
	        {
	            Destroy(gameObject);
	        }
	    }
	}
}
