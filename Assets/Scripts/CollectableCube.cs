using UnityEngine;

public class CollectableCube : MonoBehaviour
{
    internal bool collected;
    private bool triggeredWithObstacle;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IObstacle obstacle) && !triggeredWithObstacle)
        {
            triggeredWithObstacle = true;
            obstacle.OnCollision(this);
        }
    }
    internal void SetPositionAndDestroy(Transform collidedTransform, float destroyTimer = 5f)
    {
        transform.parent = null;
        transform.position = new Vector3(transform.position.x, collidedTransform.position.y, transform.position.z);
        Destroy(GetComponent<BoxCollider>());
        Destroy(gameObject, destroyTimer);
    }
}
