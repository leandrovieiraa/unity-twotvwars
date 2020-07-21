using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
public class SpawnArea : MonoBehaviour
{
    public BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnDrawGizmosSelected()
    {
        // Display the area radius when selected
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}
