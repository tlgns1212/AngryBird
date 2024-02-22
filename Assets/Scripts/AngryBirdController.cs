using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryBirdController : MonoBehaviour
{
    [SerializeField] private AudioClip[] _hitClips;

    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;

    private bool _isLaunched;
    private bool _shouldFaceVelDirection;

    private AudioSource _audioSource;

    private void Awake() 
    {
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start() 
    {
        _rb.isKinematic = true;
        _circleCollider.enabled = false;
    }

    public void LaunchBird(Vector2 dir, float force) 
    {
        _rb.isKinematic = false;
        _circleCollider.enabled = true;

        _rb.AddForce(dir * force, ForceMode2D.Impulse);
        _isLaunched = true;
        _shouldFaceVelDirection = true;
    }

    private void FixedUpdate() {
        if (!_isLaunched || !_shouldFaceVelDirection) return;
        transform.right = _rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        _shouldFaceVelDirection = false;
        SoundManager.instance.PlayRandomClip(_hitClips, _audioSource);
        Destroy(this);
    }
}
