using AetherAlmachina.Editor.DIVFactor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

public sealed class LifetimeSceneProcessor : IProcessSceneWithReport
{
    public int callbackOrder => -10000;
    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report != null) MoldBaker.ProcessScene(scene);
    }
}
