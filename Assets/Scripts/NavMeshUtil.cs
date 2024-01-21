using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtil
{
    public static Vector3 RandomNavmeshLocation(Vector3 origin, float radius, int areaMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, areaMask))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
