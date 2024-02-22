using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggieController : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.2f;
    [SerializeField] private GameObject _deadParticle;
    [SerializeField] private AudioClip _deathClip;

    private float _currentHealth;

    private void Awake() 
    {
        _currentHealth = _maxHealth;
    }

    public void Damaged(float damageAmount)
    {
        _currentHealth -= damageAmount;
        
        if(_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die(){
        GameManager.instance.RemovePiggie(this);

        Instantiate(_deadParticle, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(_deathClip, transform.position);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        float impactVelocity = other.relativeVelocity.magnitude;

    if(impactVelocity > _damageThreshold){
            Damaged(impactVelocity);
    }
    }
}
