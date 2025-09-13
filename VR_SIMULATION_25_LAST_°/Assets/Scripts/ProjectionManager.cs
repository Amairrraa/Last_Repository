using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectionManager : MonoBehaviour
{
    [SerializeField] private LineRenderer Line;
    [SerializeField] private int MaxPhysicsFrameIterations = 100;
    [SerializeField] private Transform ObstaclesParent;

    private Scene SimulationScene;
    private PhysicsScene PhysicsScene;
    private readonly Dictionary<Transform, Transform> SpawnedObjects = new Dictionary<Transform, Transform>();

    private void Start()
    {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene()
    {
        SimulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        PhysicsScene = SimulationScene.GetPhysicsScene();

        foreach (Transform obj in ObstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var rend = ghostObj.GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;

            SceneManager.MoveGameObjectToScene(ghostObj, SimulationScene);
            if (!ghostObj.isStatic) SpawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    private void Update()
    {
        foreach (var item in SpawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory(BiodiversityUnitManager ballPrefab, Transform spawnPoint, float force, float arcAmount)
    {
        if (ballPrefab == null || Line == null) return;

        var ghostObj = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, SimulationScene);

        Vector3 launchDirection = (spawnPoint.forward + spawnPoint.up * arcAmount).normalized;
        Vector3 velocity = launchDirection * force;
        ghostObj.Init(velocity, true);

        Line.positionCount = MaxPhysicsFrameIterations + 1;
        Line.SetPosition(0, spawnPoint.position);

        for (int i = 1; i <= MaxPhysicsFrameIterations; i++)
        {
            PhysicsScene.Simulate(Time.fixedDeltaTime);
            Line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }

    public void ClearLine()
    {
        if (Line != null)
            Line.positionCount = 0;
    }
}