using UnityEngine;

public class FlayingObjectManager : MonoBehaviour
{
    public void DestroyAllFlyingObjects()
    {
        flyingObjectsScript[] flyingObjects =
            Object.FindObjectsByType<flyingObjectsScript>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (flyingObjectsScript obj in flyingObjects)
        {
            if (obj == null)
                continue;

            if (obj.CompareTag("bomb"))
            {
                obj.TriggerExplosion();

            }
            else
            {
                obj.StartToDestroy(Color.cyan);
            }
        }
    }
}