using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEditorUtils : MonoBehaviour
{
    [SerializeField] private float range;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,range);
    }
}
