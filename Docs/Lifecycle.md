# DIVFactorの使用期間

個体の構築と一回の使用を分ける。RegisterPooledで生成した個体はGameObjectと固定DIコンテナを保持し、ファクトリー引数と使用期間を差し替えて再使用する。通常のRegisterは毎回生成する。廃止したArgument属性や抽象MonoBehaviourへの変更は導入しない。プーリングの詳細はPooling.mdを参照する。

## 使用開始

個体ごとにコンポーネント一覧を一度取得し、全対象のInjection → Initialize → UpdateView → Subscribe → 表示有効化 → Activateの順で実行する。途中で親を変えても、この一覧から対象を失わない。

| 定義 | 担当 |
| --- | --- |
| IInjectable.Injection(InjectableResolver) | 依存するインスタンスの代入だけ |
| DIVFactor.Lifecycle.IInitializable.Initialize() | 引数から作る状態の初期化 |
| IViewUpdatable.UpdateView() | 画像・テキスト・配置への反映 |
| IUsageSubscriber.Subscribe(UsageLifetime) | 今回の購読・後始末の登録 |
| IActivatable.Activate() | 全員の購読完了後の開始通知・動作開始 |
| IReleasable.Release() | 自動管理外の停止・解除と固有の後始末 |

各インターフェイスは任意実装。IInitializableはVContainer.Unityの同名インターフェイスと別なので、両方をusingする場合は型名を修飾する。Injectionは既存のDIVFactor.Injectable.IInjectableを使う。

### インターフェイスのバンドル

よく併用する契約をLifecycleBundles.csでまとめる。バンドルの継承元にある個別の契約は維持し、LifecycleRunnerは従来どおり各契約で判定する。同じIInjectableを複数の経路で継承してもInjectionを重複実行しない。

| バンドル | 含む処理 | 使用する型 |
| --- | --- | --- |
| IInitializableComponent | Injection、Initialize | CardBase |
| ISubscriberComponent | Injection、Subscribe | SkillTriggerManager、FriendlyLayout、HostileLayout |
| IReactiveComponent | Injection、Initialize、Subscribe | CostManager、CardSelector |
| IActiveComponent | Injection、Initialize、Subscribe、Activate | Entity、HandController、EntitySpawner |
| IViewComponent | Injection、Initialize、UpdateView | CardDesign |
| IReactiveView | Injection、Initialize、UpdateView、Subscribe | CostDisplay |
| IActiveView | Injection、Initialize、UpdateView、Activate、Release | PointerController |

Player/EnemyはEntityから、HandVisualizerはHandControllerから契約を引き継ぐ。ILifetimeSpawnerとICardData/IEntityInteractionは生成やゲーム側の役割なのでバンドルに含めない。

Injectionだけを行うBrainBase/EntityLayout、InjectionとUpdateViewだけのUIChildMark、ILifetimeSpawnerとInitializeだけのPointerSpawnerは個別のインターフェイスを維持する。既存の組み合わせに合わない型へ、空のInitializeやSubscribeなどを追加してバンドルへ合わせない。必要なら個別契約を併用する。

同じ段階内は初回に取得したHierarchy上の一覧順。Initializeから別対象のInitialize完了を前提にした通知は発行しない。現在のPlayerでは根のEntityのActivateでデッキを設定し、その後、配下のHandControllerのActivateで初期手札を引く。

UnityのStartは再実行されないため、手札、Entity生成、ポインターのTweenなどの使用開始処理をActivateへ移した。シーンのStageSpawner.Startは従来の起動入口として維持する。シーンの一般MonoBehaviourをRootから自動注入する機能は追加していない。

## 使用期間の購読

```csharp
public void Subscribe(UsageLifetime lifetime)
{
    damageEvent.Subscribe(OnDamage).AddTo(lifetime);
}
```

UsageLifetime.Disposeは登録済みのIDisposableをすべて終了し、Tokenをキャンセルする。一件の終了処理が失敗しても残りを処理し、例外をまとめて報告する。終了済みの使用期間への追加は即Disposeする。使い終えた期間を再び有効にしない。

これは登録したハンドルの後始末であり、共有EventPortを破棄する指示ではない。未登録の購読、TweenなどはReleaseで解除する。Unity APIとライフサイクルの要求はメインスレッドで扱う。

## デフォルトのEventHub

RootLifetimeScopeと各LifetimeObjectは、Installより前に自身専用のLifecycleEventHubを登録する。子スコープの要求が親や兄弟へ混ざらない。管理用の購読は使用期間の購読と分け、個体の最終破棄まで保持する。

ReleaseRequest、DestroyRequest、SpawnRequestは、処理を依頼するメッセージとしてRequestEventを継承する。RequestEventはEventObjectを継承しているため、EventPortとEventHubの登録方式は変わらない。

```csharp
public void Injection(InjectableResolver resolver)
{
    resolver.Inject(out lifecycle);
}

// 使用を終える。プール対象は返却し、それ以外は最終破棄する。
lifecycle.Release.OnNext(new ReleaseRequest());

// 明示的な最終破棄要求。
lifecycle.Destroy.OnNext(new DestroyRequest());

// 引数と生成結果の型は、既存のファクトリーに任せる。
CardBase createdCard = null;
lifecycle.Spawn.OnNext(new SpawnRequest(() => createdCard = cardFactory(skillData)));
```

SpawnはActive中だけ受け付ける。既存のAction/Funcファクトリーを通常どおり直接呼ぶ方法も維持する。ReleaseRequestは引数を持たない。Injectionで渡すLifecycleEventHubは使用期間ごとに新しくし、終了した期間のHubは完了状態にする。古いHubへの要求は次の使用へ届かない。Container.Resolveで直接得るHubは物理個体の管理用なので、遅延処理にはその使用時点で注入されたHubかUsageLifetimeを保持する。

終了要求は開始フェーズ内で同期的に届いても再入せず、残りの開始処理を止めて終了へ進む。Release/Destroy要求の重複で後始末や破棄を重ねて実行しない。

## 終了と最終破棄

所有する子スコープの使用終了 → 使用期間の購読解除 → Release（対象一覧の逆順）→ 非表示 → 引数のdefault化の順で処理する。プール対象ではInjectionを再実行して引数フィールドもdefaultへ更新してから返却する。次の使用では引数を差し替え、InjectionからActivateまで再実行する。Destroy要求・非プール個体・保管上限超過・初期化や後始末の失敗は最終破棄へ進める。

OnDestroyでは未実行の終了処理を依存先のコンテナが生きている間に済ませ、その後VContainerへ破棄を委譲する。親より先に子のReleaseとコンテナ破棄を行う。正常終了後のOnDestroyではReleaseを再実行しない。

通常の終了にはLifecycleEventHubの要求を使う。継承元のLifetimeScope.Dispose/DisposeCoreやContainer.Disposeを外から直接呼ぶと、Releaseより先にコンテナが解放されるため、この順序を保証できない。

## Bindの廃止

ActivePoint/BindPoint/EndPoint、EventPoint、resolver.Bind、ObjectBinder、BinderInChildを削除した。UIChildMark.UpdateViewがPlayerUIRootへTransformを移す。

移動するUIは、注入したLifetimeOwnership.Track(gameObject)へ登録する。これは親変更を行わず、元のLifetimeが表示と破棄を担当する対象だけを保持する。DIの所属はTransformの移動で変更しない。

PlayerUIBinderはPlayerUIRootへ変更し、スクリプトとフォルダの既存GUIDを維持した。UIChildMarkも既存GUIDを維持し、Prefabのクラス識別名だけを更新している。古いBinderフォルダへ新しいファイルを上書きする方法ではなく、削除・移動を含むパッチ全体を適用する。

## 検証

LifecycleTests、ResourceUsageTests、PoolingTestsで順序・解除・スコープ分離・再取得・引数更新・失敗時の後始末を確認する。既存EventHubRegistrationTestsも実行する。LifecycleUnityTests、PoolingUnityTests、PoolingComponentUnityTestsは空のテスト用シーンからPlay Modeへ入り、Unity上での移動・再利用・破棄とカード/ポインターの状態を確認する。

この環境ではUnity Editorがないため、.NET 8上でVContainer 1.18.0/R3 1.3.1を使い、Unity APIと一部のゲーム入力アセットを代替してコンパイル・テストする。Play Mode・IL2CPP・実Prefabを使ったゲーム全体の動作は、Unityで別途確認する。
