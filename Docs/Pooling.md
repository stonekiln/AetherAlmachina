# DIVFactorのプーリング

## 対象と登録

Unity 6000.3のUnityEngine.Pool.ObjectPool<T>を使用する。カードとActPointerをプール対象に変更した。Stage、Player、Enemyなど通常のRegisterによる生成は維持する。Mold属性、事前生成カタログ、明示Buildとの関係はMoldFactories.mdを参照する。

```csharp
[Mold("SkillCard/CardBase")]
Factory<SkillData, CardBase> cardFactory;

// ILifetimeSpawner.SpawnConfigure内
builder.RegisterPooled<CardLifetime>(maxRetained: 64).Inject(out cardFactory);

// 使用時
CardBase card = cardFactory.Spawn(skillData);
```

RegisterPooledはLifetimeObjectのSpawnConfigureで使用する。スポナーのGameObjectにLifetimePoolコンポーネントを自動付与し、親スコープが管理する。シーンへの手動配置は不要。ActiveCountとInactiveCountは保持中の実個体の数を返す。

プールはファクトリーのInjectによる束縛ごとに独立する。プレイヤー間、Friendly/HostileのActPointer間、異なる引数の束縛間で個体や親DIコンテナを混ぜない。

- 待機個体があれば再使用する。
- 空ならPrefabから新しい個体とDIコンテナを作成する。
- maxRetainedは待機個体の保管上限。貸出総数の上限ではない。超過した返却分は破棄する。
- 取得結果は引数の注入とActivateまで完了した個体。開始中に終了した場合は成功した個体として返さない。
- 所有元が使用中でなくなった後の取得は失敗する。破棄された親のファクトリーを使い続けない。

## 再使用と引数

コンポーネント一覧はEditorで保存する。実行時の初回だけPrefab複製、保存済み一覧の復元、Spawnerの設定、コンテナ構築を行う。スコープ管理用GameObjectは返却時に無効化しない。表示用オブジェクトのルートだけを必要時に有効化/無効化する。

各使用では引数を設定し、新しいUsageLifetimeと期間専用のLifecycleEventHubを作り、Injection → Initialize → UpdateView → Subscribe → 表示 → Activateを実行する。

ファクトリー引数はDI内のLifetimeArgumentsが保持する。引数のアクセサーはTransient登録し、同じ固定コンテナから現在値を解決する。固定サービスやMonoBehaviourのSingletonは作り直さない。nullや値型のdefaultも扱う。同じ型を複数の引数として渡すことはできない。

可変引数は毎回Injectionで取得する。固定Singletonのコンストラクターへ可変引数を渡して保持する構成では、そのSingleton内部の参照は再注入されない。引数インスタンス自身の内部状態も自動で巻き戻さない。呼び出し元から今回の使用に対応するデータを渡す。

## 返却

```csharp
lifecycle.Release.OnNext(new ReleaseRequest());
```

子スコープの使用終了 → UsageLifetimeのDispose → Release → 非表示 → 引数のdefault化 → Injectionによるフィールド更新 → プールへの返却を行う。Injectionは代入だけとし、この段階でもイベント登録や状態計算を行わない。例外があれば残りの後始末を試みて、その個体は再使用せず破棄する。

固定サービスの内部状態、任意のゲーム状態、未登録の購読は自動でリセットしない。IReleasableとInitializeで扱う。今回対応したカードは選択・ホバー・サイズ・位置・画像をリセットする。ActPointerはTweenを停止し、位置・色・描画順を戻し、次のスキルで再設定する。使用者のEntityが破棄された場合もポインターを返却する。

返却後のMonoBehaviour参照は使用しない。同じ物理個体が別の使用へ貸し出される可能性がある。

## 古い処理と最終破棄

InjectionされたLifecycleEventHubは期間ごとに異なる。古いHubは完了済みなので、遅れてRelease/Destroy/Spawnを送っても次の使用へ届かない。数値のUsageIdは公開しない。

遅延コールバックには、その時点のUsageLifetimeとHubをローカル変数で保持する。あとからMonoBehaviourのフィールドを読むと、新しい使用の値を参照してしまう。ActPointerのTween完了処理はこの方法で保護する。

Container.Resolve<LifecycleEventHub>()で直接取得するHubは、期間を問わず物理個体を操作する管理用。通常のゲーム処理はInjectableResolver経由で注入する。

DestroyRequestはプールへ返さず破棄する。外部から破棄された待機個体は次の取得時に生成し直す。所有するLifetimeの最終破棄では、貸出中・待機中の両方を終了し、子コンテナを親より先に破棄する。LifetimePool自体が破棄された場合も保持中の全個体を終了する。

## 検証

- PoolingTests: 実際のVContainer/R3/Unity ObjectPoolのC#実装による、引数・固定サービス・取得/返却・保管上限・破棄・失敗・遅延した要求・購読の確認。
- PoolingUnityTests: Unity上のPrefab相当オブジェクト複製、実LifetimeScope、管理MonoBehaviour、親子の破棄。
- PoolingComponentUnityTests: 実カードコンポーネントの状態/購読と、ActPointerのTween/描画順/使用者破棄。

Unity Editorのテストは空のテスト用シーンで実行する。この作業環境にUnity Editorはないため、ネイティブPlay Mode・実ゲームの操作・IL2CPP・Profiler比較は別途確認する。
