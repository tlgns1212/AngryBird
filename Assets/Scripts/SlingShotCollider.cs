using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotCollider : MonoBehaviour
{
    [SerializeField] private LayerMask _slingShotAreaMask;

    public bool IsWithinSlingShotArea(){
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if(Physics2D.OverlapPoint(worldPos, _slingShotAreaMask)){
            return true;
        }

        return false;
    }
}
