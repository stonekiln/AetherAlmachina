using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

public class LifetimeSceneProcessor : IProcessSceneWithReport
{
    public int callbackOrder => throw new System.NotImplementedException();

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        throw new System.NotImplementedException();
    }
}