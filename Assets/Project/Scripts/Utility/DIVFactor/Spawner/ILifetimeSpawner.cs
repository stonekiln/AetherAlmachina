namespace DIVFactor.Spawner
{
    /// <summary>
    /// このMonoBehaviourはLifetimeObjectのスポナーとしての役割を持つ
    /// </summary>
    public interface ILifetimeSpawner
    {
        /// <summary>
        /// ここでスポナーの設定を行う
        /// </summary>
        /// <param name="builder">スポナーを生成するビルダー</param>
        void SpawnConfigure(SpawnerBuilder builder);
    }
}