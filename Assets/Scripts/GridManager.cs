using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public float minX, maxX, minY, maxY;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static bool InBounds(Vector2 pos)
    {
        return pos.x >= instance.minX && pos.x <= instance.maxX && pos.y >= instance.minY && pos.y <= instance.maxY;
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f), new Vector3(maxX - minX, maxY - minY));
    }

#endif
}
