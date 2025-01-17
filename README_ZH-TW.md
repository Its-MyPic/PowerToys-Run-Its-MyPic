# PowerToys Run Plugin: IT's MyPic!!!!!

[English](./README.md)

一個用於複製 MyGO meme 的 [PowerToys Run](https://learn.microsoft.com/zh-tw/windows/powertoys/run) 擴充功能

## 功能
 * 搜尋並複製截圖
 * 記憶常用截圖
 * 也可用於複製文字


![alt text](Docs/intro.png)


## 安裝方式
### 執行安裝腳本
1. 從 [releases page](releases) 下載最新的安裝包
2. 解壓縮安裝包並打開資料夾

    ![alt text](Docs/setup_bat.png)

3. 執行 `setup.bat`
    
    ![alt text](Docs/terminal.png)

> 若提示 `終止 PowerToys 失敗`，請使用管理員身分執行 `setup.bat`

### 手動安裝
1. 從 [releases page](releases) 下載最新的安裝包
2. 將 `.zip` 的內容解壓縮至 `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`

    ![alt text](Docs/install.png)

3. 重新啟動 PowerToys Run

## 使用方式
1. 開啟 PowerToys Run (預設快捷鍵為 `Alt+Space` )
2. 輸入 `go` 並搜尋想複製的圖

## 如何建置
1. 確保已啟用 Windows 11 24H2 之 [sudo](https://learn.microsoft.com/zh-tw/windows/sudo/) 並設定為內嵌
2. 按下 `debug` 即可開始偵錯

    ![alt text](Docs/debug.png)
