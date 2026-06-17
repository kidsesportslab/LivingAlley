# Relive (LivingAlley) - プロジェクト文脈

## 概要
人生シミュレーションゲーム。AI住民が暮らす村「ナリソメ村」。
Unity 6 / C#。ソロ開発。現在Phase 2(ローカルLLM統合)。

## ハードウェア制約(重要)
- GPU: GTX 1660 Ti 6GB VRAM → LLMはOllama + Llama3.2 (Q4_K_M)のみ。これより大きいモデルを提案しない。
- LLM呼び出しは localhost:11434 のOllama API経由。外部API・有料サービスは禁止。

## 既存の主要システム
- NPCMover.cs: 欲求(空腹/疲労/孤独/労働)、経済(食費25f/労働報酬20f/初期30f)、餓死
- GameLogger: history.txt にイベント記録(C:/Users/Administrator/AppData/LocalLow/DefaultCompany/LivingAlley/history.txt)
- NPCSpawner: 死亡→リスポーン(Coroutine)、日本語名ランダム付与
- Animator: int "state" で6状態(idle/walk/working/social/sleep/eating)
- 吹き出し: World Space Canvas + LateUpdateでカメラ向き

## Phase 2 の目標
1. OllamaClient.cs(UnityWebRequestでOllamaに非同期リクエスト)
2. NPC会話のLLM生成(状態・所持金・直近イベントをプロンプトに注入)
3. AI新聞: history.txt を要約して村の新聞記事を生成

## 作業ルール
- コードのみ担当。Unityエディタ作業は人間がやる。エディタ操作が必要な変更をしたら手順を箇条書きで出力すること。
- 日本語UIテキストに新しい文字を使ったら「Font Assetに未登録の可能性がある文字一覧」を出力すること(TMPのYuGothic_JPは手動登録制)。
- LLM応答待ちでメインスレッドをブロックしない(Coroutine or async)。
- フレーム毎のLLM呼び出し禁止。会話はイベント駆動+クールダウン。
- コミットは機能単位、メッセージは日本語でOK。push前に確認を取る。
- 説明は最小限。動くコードを優先。

## ゲーム内時間設計
- 1ゲーム日=現実30分(検証フェーズ用)。リーダーNPC実装時にこの値を使う。欲求パラメータの減衰とは独立したタイマーとして実装すること。

## 次回タスク
- フォント欠落対応: YuGothic_JP TMP Font Assetに未登録文字が出るたびConsole警告が出る。録画前に警告を全部確認して文字を手動登録する必要あり。
- NPC同期問題: 全NPCが同じ初期値・同じ減衰速度で動いてるため欲求が同時に閾値を超えて行動が集団的に見える。個体差(初期値のランダムオフセット等)の導入を検討。

## 将来の設計方針メモ
- バイラル設計: 「開発者にも予測できない出来事」がコアコンテンツ。NPCの予想外の行動を開発者が一番最初に目撃してSNS投稿する設計。
- 予測不能性の設計: 「制御する部分」と「自由にする部分」を意図的に分ける。構造化JSON出力だけでは予測不能性が生まれない。Phase 3以降の議題。
- NPCの異常報告システム: プレイヤーがゲーム内で報告→Claude APIで自動対応→人力チェック。Phase 4以降の議題。
