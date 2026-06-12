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
