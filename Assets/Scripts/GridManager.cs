using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] private GridDot dotPrefab;

    public float minX, maxX, minY, maxY;

    private void Awake()
    {
        instance = this;

        for (int x = Mathf.CeilToInt(minX); x <= maxX; x++)
        {
            for (int y = Mathf.CeilToInt(minY); y <= maxY; y++)
            {
                Instantiate(dotPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
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
