# MakeUPMPackage

Narazakaが個人用に作ったUPMパッケージのGitリポジトリとかを雑に作るやつ

## Install

### OpenUPM

See [OpenUPM page](https://openupm.com/packages/net.narazaka.unity.make-upm-package/)

### VCC用インストーラーunitypackageによる方法（VRChatプロジェクトおすすめ）

https://github.com/Narazaka/MakeUPMPackage/releases/latest から `net.narazaka.unity.make-upm-package-installer.zip` をダウンロードして解凍し、対象のプロジェクトにインポートする。

### VCCによる方法

1. https://vpm.narazaka.net/ から「Add to VCC」ボタンを押してリポジトリをVCCにインストールします。
2. VCCでSettings→Packages→Installed Repositoriesの一覧中で「Narazaka UPM Listing」にチェックが付いていることを確認します。
3. アバタープロジェクトの「Manage Project」から「MakeUPMPackage」をインストールします。

## Usage

[gh (Github CLI)](https://cli.github.com)とgitを設定しておく必要があります。

- `Tools/[UPM] Make UPM Package`
- `Tools/[UPM] Clone UPM Package`

## License

[Zlib License](LICENSE.txt)
