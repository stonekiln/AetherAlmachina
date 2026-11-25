using System;
namespace DIVFactor.Spawner
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MoldAttribute : Attribute
    {
        public string Path { get; }
        public MoldAttribute(string path) => Path = path;
    }

    /// <summary>EditorがFactoryフィールドを識別するための契約。Factory本体は保存しない。</summary>
    public interface IMoldFactory { }
}
