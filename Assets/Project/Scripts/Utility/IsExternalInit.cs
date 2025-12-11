using System.ComponentModel;

//このクラスを実装しないと位置指定レコードの機能を使えない
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IsExternalInit { }
}