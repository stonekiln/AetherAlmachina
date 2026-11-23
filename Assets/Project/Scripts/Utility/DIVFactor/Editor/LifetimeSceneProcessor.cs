using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

public class LifetimeSceneProcessor : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        // シーンのスコープ自動構築は未導入のため、現在は加工しない。
    }
}