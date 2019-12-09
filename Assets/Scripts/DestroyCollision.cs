using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollision : MonoBehaviour
{
    public LayerMask destruirLayer;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.1f, destruirLayer);

        if(hit == true)
        {
            Destroy(this.gameObject);
        }        
    }
}
