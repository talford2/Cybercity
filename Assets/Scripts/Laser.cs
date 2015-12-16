using UnityEngine;

public class Laser : MonoBehaviour
{
    public float MissileLength = 6f;
    public float MissileSpeed = 150f;
    public float MissileDamage = 5f;

    public GameObject HitEffectPrefab;

    public GameObject HitDecalPrefab;

    private bool _isLive;
    private LineRenderer _lineRenderer;
    private Light _light;
    private bool _hasHit;

    private GameObject _owner;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private float _initSpeed;
    private Vector3 _hitPosition;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _light = GetComponentInChildren<Light>();
        Stop();
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    public void Update()
    {
        if (_isLive)
        {
            var displacement = (_initSpeed + MissileSpeed)*Time.deltaTime;
            if (!_hasHit)
            {
                var missileRay = new Ray(transform.position, transform.forward);
                RaycastHit missileHit;
                if (Physics.Raycast(missileRay, out missileHit, displacement))
                {
                    Debug.Log("HIT: " + missileHit.collider.name);
                    if (missileHit.collider.gameObject != _owner)
                    {
                        _hasHit = true;
                        _hitPosition = missileHit.point;

                        //killable.SendMessageUpwards("Hit", new HitEffectParameters { Position = missileHit.point, Normal = missileHit.normal });

                        // TODO: Should pull this effect from a pool or something...
                        if (HitEffectPrefab != null)
                        {
                            var hitEffectInstance = Instantiate(HitEffectPrefab);
                            hitEffectInstance.transform.position = missileHit.point;
                            hitEffectInstance.transform.forward = missileHit.normal;
                        }

                        if (HitDecalPrefab != null)
                        {
                            var hitDecal = Instantiate(HitDecalPrefab);
                            hitDecal.transform.position = missileHit.point;
                            hitDecal.transform.SetParent(missileHit.collider.gameObject.transform);
                            hitDecal.transform.forward = missileHit.normal;
                        }
                    }
                }
            }

            transform.position += transform.forward*displacement;
        }
    }

    public void LateUpdate()
    {
        if (_isLive)
        {
            UpdateLineRenderer();
        }
    }

    public void UpdateLineRenderer()
    {
        var headPosition = transform.position;
        var tailPosition = transform.position - transform.forward*MissileLength;

        var tailDotProd = Vector3.Dot(tailPosition - _shootFrom, transform.forward);
        var headHitDotProd = Vector3.Dot(headPosition - _hitPosition, transform.forward);

        if (_hasHit && headHitDotProd > 0f)
        {
            headPosition = _hitPosition;
            var tailHitDotProd = Vector3.Dot(tailPosition - _hitPosition, transform.forward);
            if (tailHitDotProd > 0f)
            {
                Stop();
                return;
            }
        }

        if (tailDotProd < 0f)
        {
            tailPosition = _shootFrom;
        }

        _lineRenderer.SetPosition(0, tailPosition);
        _lineRenderer.SetPosition(1, headPosition);
    }

    public void Stop()
    {
        _isLive = false;
        _lineRenderer.enabled = false;
        _light.enabled = false;
        _hasHit = false;
    }

    public void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        _isLive = true;
        _lineRenderer.enabled = true;
        _light.enabled = true;
        _shootFrom = shootFrom;
        _initVelocity = initVelocity;
        _initSpeed = _initVelocity.magnitude;
        _hasHit = false;
        transform.position += initVelocity*Time.deltaTime;
        transform.forward = direction;
        UpdateLineRenderer();
    }
}
