# DIVFactorの使用期間

個体の構築と一回の使用を分ける。RegisterPooledで生成した個体はGameObjectと固定DIコンテナを保持し、ファクトリー引数と使用期間を差し替えて再使用する。通常のRegisterは毎回生成する。廃止したArgument属性や抽象MonoBehaviourへの変更は導入しない。プーリングの詳細はPooling.mdを参照する。

## 使用開始

同じLifetimeObjectに属する対象を段階ごとにまとめて処理する。対象一覧はEditorで保存するため、実行時に表示上の親が変わっても所属を維持する。Buildは物理個体の初回だけ行う。

| 順序 | API | 用途 |
| --- | --- | --- |
| 1 | `Injection(InjectableResolver resolver)` | 依存先を代入し、必要なら注入後の処理を登録する |
| 2 | `resolver.Injected` | 全員の注入後に引数から状態を初期化する |
| 3 | `UpdateView(ViewContext context)` | 現在の状態を表示へ反映する。従来どおり自動実行 |
| 4 | `context.UpdatedView` | 全員の描画処理が戻った後に、表示に依存する処理を実行する |
| 5 | `Subscribe(SubscriptionContext context)` | 使用期間の購読を `.AddTo(context.Lifetime)` で管理する |
| 6 | `context.Subscribed` | 全員の購読後に初期イベントを送る |
| 7 | 表示を有効化 | Unityで表示対象がactiveInHierarchyになり、スクリプトもenabledならOnEnableを受ける |

各契約は任意。注入だけなら `IInjectable`、描画は `IViewUpdatable`、購読は `IUsageSubscriber`、返却時の固有の後始末は `IReleasable` を使う。併用する組み合わせには `ISubscriberComponent`（注入と購読）、`IViewComponent`（注入と描画）、`IReactiveView`（3つ全て）がある。従来のバンドル名を使うこともできる。InitializeやActivateは同名のメソッドを定義するだけでは呼ばれない。

```csharp
public void Injection(InjectableResolver resolver)
{
    resolver.Inject(out skill);
    resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
}

public void UpdateView(ViewContext context)
{
    Draw(skill);
    context.UpdatedView.SubscribeLifecycle(AlignToOtherViews, context.Fail);
}

public void Subscribe(SubscriptionContext context)
{
    damageEvent.Subscribe(OnDamage).AddTo(context.Lifetime);
    context.Subscribed.SubscribeLifecycle(Begin, context.Fail);
}
```

3つの完了通知は、使用ごとに独立した読み取り専用のR3 `Observable<Unit>`。登録順に一度だけ呼び出し、完了後の登録には値を再配信しない。途中で終了要求が届いた場合、未実行の完了処理は取り消す。初期化の例外で有効化を中止するには、例のように `SubscribeLifecycle` を使う。通常のR3の `Subscribe(onNext)` だけを使った場合はR3の既定の例外処理に従う。通知の対象はメインスレッドで同期的に完了する処理。`UpdatedView` はUnityのフレーム描画やCanvasのレイアウト計算完了を表さない。

Playerでは先にEntityがデッキを準備し、その後でHandControllerが初期手札を引く。シーンのStageSpawner.Startは従来どおり。管理外のSetActiveや別スコープとの通知順にはこの保証は及ばない。Editorのプレビューは今回の対象外。

## 使用期間の購読

```csharp
public void Subscribe(SubscriptionContext context)
{
    damageEvent.Subscribe(OnDamage).AddTo(context.Lifetime);
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
lifecycle.Spawn.OnNext(new SpawnRequest(() => createdCard = cardFactory.Spawn(skillData)));
```

Spawn要求はスコープ内の全購読が揃った時点（Subscribed通知の開始）から、使用終了が要求されるまで受け付ける。FactoryのSpawnを直接呼ぶ方法も使用できる。ReleaseRequestは引数を持たない。Injectionで渡すLifecycleEventHubは使用期間ごとに新しくし、終了した期間のHubは完了状態にする。古いHubへの要求は次の使用へ届かない。Container.Resolveで直接得るHubは物理個体の管理用なので、遅延処理にはその使用時点で注入されたHubかUsageLifetimeを保持する。

終了要求は開始フェーズ内で同期的に届いても再入せず、残りの開始処理を止めて終了へ進む。Release/Destroy要求の重複で後始末や破棄を重ねて実行しない。

## 終了と最終破棄

所有する子スコープの使用終了 → 使用期間の購読解除 → Release（対象一覧の逆順）→ 非表示 → 引数のdefault化の順で処理する。プール対象ではInjectionを再実行して引数フィールドもdefaultへ更新してから返却する。返却時の再注入ではInjectedを通知しない。次の使用で引数を差し替え、Injectionから表示の有効化までを実行する。Destroy要求・非プール個体・保管上限超過・初期化や後始末の失敗は最終破棄へ進める。

OnDestroyでは未実行の終了処理を依存先のコンテナが生きている間に済ませ、その後VContainerへ破棄を委譲する。親より先に子のReleaseとコンテナ破棄を行う。正常終了後のOnDestroyではReleaseを再実行しない。

通常の終了にはLifecycleEventHubの要求を使う。継承元のLifetimeScope.Dispose/DisposeCoreやContainer.Disposeを外から直接呼ぶと、Releaseより先にコンテナが解放されるため、この順序を保証できない。

## Bindの廃止

ActivePoint/BindPoint/EndPoint、EventPoint、resolver.Bind、ObjectBinder、BinderInChildを削除した。UIChildMark.UpdateViewがPlayerUIRootへTransformを移す。

移動するUIは、注入したLifetimeOwnership.Track(gameObject)へ登録する。これは親変更を行わず、元のLifetimeが表示と破棄を担当する対象だけを保持する。DIの所属はTransformの移動で変更しない。

PlayerUIBinderはPlayerUIRootへ変更し、スクリプトとフォルダの既存GUIDを維持した。UIChildMarkも既存GUIDを維持し、Prefabのクラス識別名だけを更新している。古いBinderフォルダへ新しいファイルを上書きする方法ではなく、削除・移動を含むパッチ全体を適用する。

## 検証

LifecycleTests、ResourceUsageTests、PoolingTestsで順序・解除・スコープ分離・再取得・引数更新・失敗時の後始末を確認する。既存EventHubRegistrationTestsも実行する。LifecycleUnityTests、PoolingUnityTests、PoolingComponentUnityTestsは空のテスト用シーンからPlay Modeへ入り、Unity上での移動・再利用・破棄とカード/ポインターの状態を確認する。

この環境ではUnity Editorがないため、.NET 8上でVContainer 1.18.0/R3 1.3.1を使い、Unity APIと一部のゲーム入力アセットを代替してコンパイル・テストする。Play Mode・IL2CPP・実Prefabを使ったゲーム全体の動作は、Unityで別途確認する。
