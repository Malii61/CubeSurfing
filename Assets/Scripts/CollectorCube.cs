using System;
using UnityEngine;

public class CollectorCube : MonoBehaviour
{
    public static CollectorCube Instance { get; private set; }
    [SerializeField] Transform mainCube;
    private CubeController cubeController;
    public event EventHandler<OnCubeCollectedEventArgs> OnCubeCollected;
    public class OnCubeCollectedEventArgs : EventArgs
    {
        public Transform cubeTransform;
        public CollectableCube collectedCube;
    }
    public event EventHandler OnCubeDropped;
    public event EventHandler<OnCoinCollectedEventArgs> OnCoinCollected;
    public class OnCoinCollectedEventArgs : EventArgs
    {
        public int coinAmount;
    }
    public event EventHandler OnFinished;
    public event EventHandler OnGameOver;
    private int cubeCount = 0;

    private int collidedObstacleAmount; 
    private int collidedGoldMultiplierAmount = 1;
    private void Awake()
    {
        Instance = this;
        cubeController = mainCube.GetComponent<CubeController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out CollectableCube cube))
        {
            if (cube.collected)
                return;
            if (cube.transform.childCount > 0)
            {
                int childCount = cube.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    CollectableCube childCube = cube.transform.GetChild(0).GetComponent<CollectableCube>();
                    SetCubeParentToMainCube(childCube);
                }
            }
            SetCubeParentToMainCube(cube);
        }
        else if (other.CompareTag("Coin"))
        {
            OnCoinCollected?.Invoke(this, new OnCoinCollectedEventArgs { coinAmount = 1 });
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("FinishLine"))
        {
            OnFinished?.Invoke(this, EventArgs.Empty);
        }
        else if (other.TryGetComponent(out GoldTreasure treasure))
        {
            treasure.Interact();
            OnCoinCollected?.Invoke(this, new OnCoinCollectedEventArgs { coinAmount = treasure.GetTreasureAmount() });
        }

        if (other.transform.TryGetComponent(out CubeRestrictor restrictor))
        {
            // main cube is out of the plane borders
            if (restrictor.restrict == CubeRestrictor.Restrict.right)
            {
                // cube is out of the borders from right side
                cubeController.SetClampPosition(CubeController.PositionClamp.right);
            }
            else
            {
                // cube is out of the borders from left side
                cubeController.SetClampPosition(CubeController.PositionClamp.left);
            }
        }
    }

    private void SetCubeParentToMainCube(CollectableCube collectedCube)
    {
        cubeCount++;
        collectedCube.collected = true;
        collectedCube.transform.parent = mainCube;
        collectedCube.transform.localPosition = new Vector3(0, -cubeCount, 0);
        OnCubeCollected?.Invoke(this, new OnCubeCollectedEventArgs
        {
            cubeTransform = collectedCube.transform,
            collectedCube = collectedCube
        });
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out WallObstacle obstacle))
        {
            for (int i = 0; i < collidedObstacleAmount; i++)
            {
                OnCubeDropped?.Invoke(this, EventArgs.Empty);
            }
            collidedObstacleAmount = 0;
        }
        if (other.transform.TryGetComponent(out CubeRestrictor restrictor))
        {
            // main cube is in the plane borders
            cubeController.SetClampPosition(CubeController.PositionClamp.none);
        }
    }

    private bool IsRunOutOfCubes()
    {
        return mainCube.transform.childCount < 3;
    }

    public void OnCollidedWithObstacle()
    {
        CheckGameOver();
        cubeCount--;
        collidedObstacleAmount++; 
    }
    public void OnCollidedWithGoldMultiplier()
    {
        CheckGameFinished();
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        cubeCount--;
        collidedGoldMultiplierAmount++;
    }
    internal void OnCollidedWithFire()
    {
        CheckGameOver();
        OnCubeDropped?.Invoke(this, EventArgs.Empty);
    }
    public int GetGoldMultiplier()
    {
        return collidedGoldMultiplierAmount;
    }
    public Transform GetPosition()
    {
        return transform;
    }
    public void DropCubeManually()
    {
        CheckGameOver();
        cubeCount--;
        OnCubeDropped?.Invoke(this, EventArgs.Empty);
    }
    public void CheckGameOver()
    {
        if (IsRunOutOfCubes())
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }
    public void CheckGameFinished()
    {
        if (IsRunOutOfCubes())
        {
            OnFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
