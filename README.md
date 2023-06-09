# 概要
GitHubのコミット数の遷移を棒グラフのランキング形式で可視化します。  
（研究室で「1週間で作れるのか？」と話題になり、挑戦しました。）

WebGLでビルドしたものを下記に公開しています。  
[サイトURL](https://matsufriends.github.io/GitHubVisualizer/)

# 使用方法
1. `OwnerName`に、リポジトリを持つユーザ名/組織名を入力する。
2. `LOAD REPOS`を押し、リポジトリ一覧を取得する。
3. ドロップダウンリストより、リポジトリを選択する。
4. `LOAD COMMITS`を押し、コミット一覧を取得する。  
- `CommitSlider`をドラッグして、フォーカスするコミットを指定します。
- `SpeedSlider`をドラッグして自動読込のスピードを調整し、`PlayButton`で再生します。
- `ClientId` / `ClientSecret`は、後述する`OAuth`を利用しない場合は空欄で構いません。

# API利用制限について
`GitHubAPI`の利用は 1時間60回 という制限がありますが、
[APIの制限 公式ドキュメント](https://docs.github.com/ja/rest/overview/resources-in-the-rest-api?apiVersion=2022-11-28#rate-limiting)

`OAuth`を用いることで 1時間5000回 まで拡張して利用することができ、本アプリケーションも対応しております。
## OAuth利用方法
1. [こちらのサイト](https://qiita.com/besmero628/items/823a7630c77318d910b0) などを参考に、`ClientId`と`ClientSecret`を取得する。
2. 取得した`ClientId` / `ClientSecret`をそれぞれ入力後`LOAD REPOS` / `LOAD COMMITS` ボタンを押すと自動で適用されます。

# プロジェクトについて
- `Unity ver2021.3.6f1`
- [UniRx](https://github.com/neuecc/UniRx/blob/master/LICENSE)
- [UniTask](https://github.com/Cysharp/UniTask/blob/master/LICENSE)
- [WebGLInput](https://github.com/kou-yeung/WebGLInput/blob/master/LICENSE)
- [MornLib](https://github.com/matsufriends/MornLib/blob/main/LICENSE)
