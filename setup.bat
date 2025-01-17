@echo off
cd /d "%~dp0"

taskkill /F /IM PowerToys*
ping -n 2 127.0.0.1 >nul
tasklist | findstr /i "PowerToys.exe" >nul
if %errorlevel% equ 0 (
    echo 終止 PowerToys 失敗，請以系統管理員身分重新執行本批次檔。
    pause
    exit
)

set ptPath=%LocalAppData%\PowerToys
set newFolder=%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins\Its-MyPic
md "%newFolder%"
md "%newFolder%\images"
echo.
copy /y * "%newFolder%"
echo.
echo 正在重啟 PowerToys...

start /B "window" "%ptPath%\PowerToys.exe"

echo.
echo 安裝完成
echo 如有異常請回報給作者
pause