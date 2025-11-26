using UnityEngine;

[ExecuteAlways]
public class OutOfWallSpawnPosition : Singleton<OutOfWallSpawnPosition>
{
    public float radius = 5f;
    public Color color = Color.cyan;
    public int segments = 64;
    public bool showDisk = true;

    private void OnDrawGizmos()
    {
        if (showDisk)
        {
            Gizmos.color = color;

            Vector3 prevPoint = transform.position + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;

                Vector3 nextPoint = transform.position +
                                    new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

                Gizmos.DrawLine(prevPoint, nextPoint);

                prevPoint = nextPoint;
            }
        }
    }

    public Vector3 RndomPosOnCircle()
    {
        Vector2 lCircle2D = Random.insideUnitCircle.normalized;

        return transform.position + new Vector3(lCircle2D.x, 0, lCircle2D.y).normalized * radius;

    }
}
