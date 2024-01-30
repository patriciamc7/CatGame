using System.Collections.Generic;
using UnityEngine;

public class collisionManager : MonoBehaviour
{
    /// <summary> List of object that collides with limits. </summary>
    public List<GameObject> CollisionObjects = new List<GameObject>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.gameObject.layer == collision.gameObject.layer)
        {
            CollisionObjects.Add(collision.gameObject);
        }
    }
}
