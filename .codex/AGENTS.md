# Codex 作業指示

## 対象範囲

- このプロジェクトで「Scripts」と言われた場合は `Assets/Project/Scripts` を指す。
- 外部コードや生成物は原則として編集しない。特に `Assets/Plugins`, `Assets/TextMesh Pro`, `Library`, `Packages` はユーザーから明示された場合だけ触る。
- Unity の `.meta` ファイルは、ファイルの追加・移動・削除に伴って必要な場合だけ扱う。

## コメントと命名

- 機能、変数、クラス、リファクタリング案を提案するときは、必要なら同時に適切なコメントも提案する。
- コメントは「何をしているか」より「なぜその設計なのか」「ゲーム仕様上どういう意味か」を優先する。
- 自明な処理に説明コメントを増やしすぎない。
- 変数名は `log`, `data`, `type`, `flag`, `number` のような汎用名を避け、イベントやゲーム仕様が分かる名前にする。
  - 例: `log` -> `drawEvent`, `selectEvent`, `damageEvent`, `lockOnRequest`
  - 例: `flag` -> `isSelected`, `isActive`
  - 例: `number` -> `count`, `cardCount`, `slotCount`

## 名前空間と配置

- ゲーム内で使うコードのルート名前空間は `AetherAlmachina` を維持する。
- `Runtime` / `Editor` はフォルダや asmdef の役割名として使ってよいが、名前空間はプロジェクト名から始める。
- Editor 拡張専用コードはゲーム実行時コードから分離する。

## DIVFactor の前提

- `Assets/Project/Scripts/Tools/DIVFactor` には、このプロジェクト固有の DI + イベント駆動フレームワークがある。
- DIVFactor は VContainer の DI スコープと R3 のイベントを併用する。
- 機能単位のイベントは `EventBus<TEvent>` を Singleton 登録し、必要なスクリプトへ DI で注入する。
- 全てのイベントメッセージは `DIVFactor.Event.EventObject` を継承する record として定義する。
- Request/Response 型のイベント連結には `EventChannel<TReq, TRes>` と `Switch` 拡張を使う。

## DI 設定の置き場所

- DI コンテナの設定は `Assets/Project/Scripts/DependencyConfig` に置く。
- DI スコープごとにフォルダを分ける。
  - Stage スコープ: `DependencyConfig/Stage`
  - Entity スコープ: `DependencyConfig/Entity`
  - Card スコープ: `DependencyConfig/Card`
  - ActPointer スコープ: `DependencyConfig/ActPointer`
- イベント定義は各スコープの `Event` フォルダに置く。
- スコープ固有の `LifetimeObject` 派生クラスは、該当スコープのフォルダに置く。

## DIVFactor で機能を追加するときの手順

1. まず、その機能がどの DI スコープに属するかを決める。
2. 新しいイベントが必要なら、該当する `DependencyConfig/<Scope>/Event` に `EventObject` 派生 record を追加する。
3. イベントを使う場合は、該当する Installer で `builder.RegisterEvent<TEvent>()` を追加する。
4. 複数の EventBus をまとめて注入したい場合は、既存の `ActionEventBundle`, `ResourceUpdateEventBundle`, `DeckDrawEventBundle` のような bundle record を使う。
5. MonoBehaviour が依存解決を受ける場合は `IInjectable` を実装し、`Injection(InjectableResolver resolver)` 内で `resolver.Inject(out value)` または `resolver.GetComponent<T>()` を使う。
6. Hierarchy 上の Component を登録する場合は、`LifetimeObject.Register(ComponentRegister register)` で `register.ComponentInChild<T>()` または `register.BinderInChild<T>()` を使う。
7. Prefab を生成してスコープを作る場合は `ILifetimeSpawner.SpawnConfigure(SpawnerBuilder builder)` で `builder.Register<TLifetime>(prefab)` を使う。

## Lifetime とイベント購読

- `LifetimeObject` はスコープの作成、DI 登録、Injectable への注入、子オブジェクトの有効化を管理する。
- `ActivePoint` は DIVFactor のバインド後に発行される。通常の Unity `Start` より前提が多い初期化は `resolver.ActivePoint.Subscribe(...)` で行う。
- R3 の購読は、MonoBehaviour の寿命に紐づく場合 `AddTo(this)` を付ける。
- `EndPoint` / `EntryEndPoint` はスコープ終了のために使う。直接 `Destroy` で代替しない。

## 既存コードを変更・レビューするときの注意

1.  既存の DIVFactor の流れを崩さない。新しい Singleton や static 状態を増やす前に、既存の EventBus + DI で表現できないか確認する。
2.  関数など機能はイベントでラップし、インジェクトによって外部に呼び出すこと。
3.  外部に露出する役割が単一であるように実装すること。
4.  原則クラスのメンバはprivateととし、外部からアクセス可能とする場合はゲッターなど読み取り専用にすること。
5.  原則一つのクラスに対して二つ以上のpublicメソッドを実装してはならない。
6.  但し第5則より第3則を優先し、複数あっても役割が単一である場合はその限りではない。(特にDIVFactor由来のインターフェイスなど初期化や生成に必ず必要なメソッドは第5則に抵触するメソッドとしてカウントしなくて良い)
7.  DependencyConfig を変更したら、そのイベントや Component がどの Runtime クラスで注入されるかまで確認する。
8.  Tools/DIVFactor 本体は基盤コードなので、ゲーム機能追加のために安易に変更しない。変更する場合は影響範囲を広めに確認する。
9.  コメント、変数名、フォルダ構成のリファクタリングは、Unity の serialized reference や inspector 表示に影響する可能性を考慮する。