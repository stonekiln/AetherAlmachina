# Mold属性・Factory・事前生成カタログ

## 登録と使用

```csharp
using DIVFactor.Spawner;

public class CardSpawner : MonoBehaviour, ILifetimeSpawner
{
    [Mold("SkillCard/CardBase")]
    Factory<SkillData, CardBase> cardFactory;

    public void SpawnConfigure(SpawnerBuilder builder)
        => builder.RegisterPooled<CardLifetime>().Inject(out cardFactory);

    public CardBase Create(SkillData skill) => cardFactory.Spawn(skill);
}
```

`ILifetimeSpawner`と`SpawnConfigure`は維持する。通常生成には`Register<TLifetime>()`を使う。Factoryは引数0〜3個に対応し、戻り値の型は最後に指定する。戻り値が不要な場合は`ActionFactory`、`ActionFactory<T1>`〜`ActionFactory<T1,T2,T3>`を使う。引数は従来と同じく型で識別するため、同じ型を2回指定できない。意味を表す別のrecord等で包む。

すべてのFactoryフィールドに`[Mold("論理パス")]`が必要。継承元のprivateフィールドも対象になる。Player用アセンブリのMonoBehaviourを検証し、属性なし、空・不正なパス、参照先なし、ILifetimeSpawner以外の所有者、static/readonly、未対応のFactory型をエラーにする。Editor用テストの不正入力fixtureはPlayer型の全件検証に含めない。

一つのフィールドは一つのMoldに対応する。同型のFactoryが複数ある場合は、登録順に依存せず、次のように選択する。

```csharp
[Mold("Entity/Ally")] Factory<StatusAsset, Entity> ally;
[Mold("Entity/Enemy")] Factory<StatusAsset, Entity> enemy;

public void SpawnConfigure(SpawnerBuilder builder)
{
    builder.Register<EntityLifetime>().Inject(out ally, nameof(ally));
    builder.Register<EntityLifetime>().Inject(out enemy, nameof(enemy));
}
```

同じGameObjectに複数のSpawnerがあっても、所有コンポーネント参照で区別する。Factoryの内部では、対応するMold、Lifetime型、親、生成先、Poolを束縛した処理を保持する。フィールドのリフレクションはEditorでの対応表作成に使用し、実行時のSpawnでは行わない。`out`自体からフィールド名を取得しているわけではない。`[Mold]`の引数はパスだけでよいが、Lifetime型とPoolの選択はSpawnConfigureで確定する。

## フォルダと自動更新

管理フォルダは`Assets/Project/DIVFactorPrefabs`。例のパスは`Assets/Project/DIVFactorPrefabs/SkillCard/CardBase.prefab`に対応する。先頭`./`、拡張子、空要素、`.`、`..`は禁止。`Path.Combine`等のバックスラッシュは`/`に正規化する。キーは大小文字を区別するが、移植時の衝突を防ぐため大小文字だけ違うPrefabは登録できない。

管理フォルダ内のPrefab追加・変更、スクリプト再コンパイル後に、ルートへ`MoldMetadata`を追加・更新する。非アクティブな子も含めて取得し、Spawner、Injection、UpdateView、Subscribe、Releaseの参照配列とFactory対応表を保存する。完了フックは使用時に登録するので保存しない。旧形式のPrefabとシーンはEditorの既存の再生成処理で形式2へ更新する。配列はprivateのシリアライズフィールド。Inspectorには読み取り専用の一覧を表示する。`DIVFactor/Mold catalog`で全Prefab、実行用カタログ、各Moldの参照元を確認できる。

Play直前とビルド直前にも再生成し、不備があれば停止する。手動更新は`DIVFactor/Rebuild open scenes`。Prefab Variant/Nested PrefabはLoadPrefabContentsで取得した実効構成を保存する。子のMoldMetadataを二重にライフサイクル対象とせず、生成するPrefab全体の実際のコンポーネント参照を保存する。Unity上でのVariant/Nestedの回帰確認は導入時に行うこと。

EditorのEnter Play Mode SettingsではReload Sceneを有効にする。Reload Domainは任意。SceneのAwakeを省略する設定には対応せず、Play前に診断する。

## シーンと配布対象

シーンの全Spawnerを探索起点とし、FactoryのMold依存を再帰的に収集する。複数の起点を統合し、既訪問のMoldは再探索しない。循環も許容するが、実際のゲーム処理による無限Spawnを防ぐ機能ではない。

`Assets/Project/DIVFactorGenerated/Catalog_<Scene GUID>.asset`に、シーンから到達できるMoldだけを保存する。カタログはScriptableObjectアセットである。SceneとPrefabの参照元を文字列で記録し、全Prefabへの参照を持つEditor専用の一覧アセットは作らない。未使用PrefabがResources外にあり、ほかのScene・Prefab・preloaded assets等からも参照されていなければ、通常のPlayerビルドへの収録経路がなくなる。Variantの基底PrefabなどUnityのアセット依存は別途含まれ得る。

ビルド時に処理中のSceneへ`MoldSceneBootstrap`と事前生成データを設定し、対応するカタログを参照させる。標準ビルドは有効なBuild Settingsシーンを準備する。カスタムビルドで異なるScene一覧を渡す場合は、先に`MoldBaker.PrepareBuild(options.scenes)`を呼ぶ。ビルド処理内でアセットを再保存せず、事前生成済みカタログとの整合を検証してからSceneへ設定する。

`DIVFactor.MoldObjects.Load("SkillCard/CardBase")`は、ロード済みシーンのカタログからPrefabのGameObject参照を辞書検索して返す。ディスクの個別ロードや複製を行うAPIではない。Prefabの直接参照を持つカタログをSceneが読み込む際にUnityが依存アセットを扱うため、Factoryを呼ぶまで個別ロードを遅延する方式ではない。辞書検索はResources.Load自体を高速化する仕組みではない。パス文字列によるLoadだけをコードに追加しても、自動収録の起点にはならない。

加算ロードではカタログを統合し、同一パス・同一Prefabの共有を許可する。異なるPrefabの同一パスはエラー。Sceneの終了時は、そのSceneの登録だけを外す。

## 初期化と所有関係

シーンデータのAwakeでシーン用RootLifetimeScopeを作る。シーン上のSpawnerはこのRootの子を生成し、Scene自体の通常MonoBehaviourへ自動Injectionは行わない。これは従来Rootの扱いを維持する。

子の生成は、アクティブな制御用GameObjectへLifetimeObjectをAddComponentし、Moldを複製、親・カタログ・引数・Pool・表示先をPrepareで設定し、親のContainerを確認してBuild、その後に使用開始する。LifetimeObject.AwakeはautoRunをfalseにしてからVContainerのAwakeを呼ぶ。LifetimeSeedは不要になり削除する。Prefab原本へ実行時の親を記入しない。

表示用MoldはSpawnerのTransformへ配置する一方、制御用Lifetimeは所有Lifetime配下で管理する。Poolも各Spawner/Factoryに分け、親が終了すると子とPoolを閉じる。表示の非アクティブ化はライフサイクル順序を守るために残すが、DIのBuildをAwakeに先行させない制御は明示Buildで行う。通常のUnity Awake/OnEnableがDIより先に実行され得る点は変わらないため、DIを使う開始処理には専用ライフサイクルを使う。

Injection → Injected → UpdateView → UpdatedView → Subscribe → Subscribed → 表示の順に進める。終了時の購読解除・逆順Release・引数クリア・Pool再利用は維持する。実行時に階層を再探索せず、保存済みの各フェーズ配列を使用する。コンポーネントのDI登録も保存済み一覧から選択する。VContainer内部のDI解決処理は引き続き存在する。

実行後のAddComponent・子追加への追従、非同期ロード、Addressables、外部MOD、Roslyn Analyzerは今回の対象外。明示Prefabを渡す旧Register/Action/Funcオーバーロードは移行・既存テスト用に残す。通常のゲームコードはMold付きFactoryを使う。

## 初回移行

Unityを閉じて配布ZIPのInstall.ps1を適用後、Unityのコンパイルを待ち、`DIVFactor/Migrate legacy prefabs and rebuild`を一度実行する。EntityObject・SkillCard・StageをResources外へGUIDを維持して移動する。旧LifetimeScopes.prefabはDIVFactorLegacyへ移し、VContainerSettingsの旧RootLifetimeScope指定を解除する。生成データのあるシーンを保存する。以降はPlay/ビルド前に自動検証する。

このメニューはアセットと設定を変更する。Gitの作業ツリーを確認してから使い、戻す場合はPrefab移動、追加metadata、シーン、カタログ、設定の変更も一緒に戻す。ZIPのRestore.ps1はソース更新用で、Unityでのアセット変更の巻き戻しは行わない。
