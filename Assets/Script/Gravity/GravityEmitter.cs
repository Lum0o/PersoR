using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class GravityEmitter : MonoBehaviour
{
    [SerializeField] private bool customMass;
    [SerializeField, EnableIf("customMass")] private float mass;
    [SerializeField] private float density = 2000;
    
    [SerializeField, EnableIf("false")] private double _g;
    [SerializeField, EnableIf("false")] private float _fmax;
    public double g => _g;
    //public double Fmax => _fmax;
    
    private double _influenceZoneRay;
    private SphereCollider _influenceZone;
    private float _ray;
    
    void Start()
    {
        _influenceZone = GetComponent<SphereCollider>();
        _influenceZone.includeLayers = LayerMask.GetMask("Gravity");
        _influenceZone.tag = "GravityEmitter";
        _influenceZone.isTrigger = true;
        
        
        Vector3 scale = transform.lossyScale;
        
        //get an average ray to compute a g
        _ray = (scale.x + scale.y + scale.z) / 6;
        _fmax = _ray * GlobalConstants.RayScaleMax;
        _influenceZone.radius = _fmax;
        
        //If there is no custom mass, compute one using general formula.
        if (!customMass)
        {
            mass = (4.0f / 3.0f) * Mathf.PI * scale.x * scale.y * scale.z * density * GlobalConstants.EmmiterMassScaling;
        }
        
        _g = GlobalConstants.G * (mass / Mathf.Pow(_ray, 2));
    }


    public Vector3 GetVelocityApplied(GravityBody rigidBody)
    {
        Vector3 direction = rigidBody.transform.position - transform.position;
        float distance = direction.magnitude;
        double strength = GlobalConstants.G * (mass * rigidBody.Mass) 
            / Mathf.Pow(distance, 2) 
            * Mathf.Exp(- Mathf.Pow(distance / _fmax, GlobalConstants.n));
        
        return direction.normalized * (float)strength;
    }
}
