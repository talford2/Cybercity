using UnityEngine;

public class Sparks : MonoBehaviour
{
    public float Lifetime;
    public Light SparksLight;
    public float SparksLightIntensity;
    public AnimationCurve LightIntensityCurve;

    private float cooldown;

	void Start ()
	{
	    cooldown = Lifetime;
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
