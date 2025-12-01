using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    [SerializeField] private bool customMass;
    [SerializeField, EnableIf("customMass")] private float mass;
    [SerializeField] private float density = 2000;
    [SerializeField] private Vector3 startVelocity;
    [SerializeField, HideInInspector] private Collider _collider;
    [SerializeField, HideInInspector] private Rigidbody _rigidbody;
    
    private List<GravityEmitter> _gravityEmitters = new();
    public float Mass => mass;

    private void OnValidate()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _collider.includeLayers |= LayerMask.GetMask("Gravity");
        Vector3 scale = transform.lossyScale;
        
        //If there is no custom mass, compute one using general formula.
        if (!customMass)
        {
            mass = (4.0f / 3.0f) * Mathf.PI * scale.x * scale.y * scale.z * density;
        }

        var emitters =  Physics.OverlapSphere(transform.position, 1);
        foreach (var other in emitters)
        {
            if (!other.gameObject.CompareTag("GravityEmitter")) return;
            var emitter = other.gameObject.GetComponent<GravityEmitter>();
            _gravityEmitters.Add(emitter);
        }
        
        _rigidbody.AddForce(startVelocity);
        print("[GravityBody] : " + _gravityEmitters.Count + " gravity emitters found !");
    }

    private void FixedUpdate()
    {
        foreach (var emitter in _gravityEmitters)
        {
            _rigidbody.AddForce(emitter.GetVelocityApplied(this));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("GravityEmitter"))
            return;

        var emitter = other.gameObject.GetComponent<GravityEmitter>();
        _gravityEmitters.Add(emitter);
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.CompareTag("GravityEmitter"))
            return;
        
        var emitter = other.gameObject.GetComponent<GravityEmitter>();
        _gravityEmitters.Remove(emitter);
    }
}
