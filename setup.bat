@echo off
cd /d "%~dp0"

taskkill /F /IM PowerToys*
ping -n 2 127.0.0.1 >nul
tasklist | findstr /i "PowerToys.exe" >nul
if %errorlevel% equ 0 (
    echo �פ� PowerToys ���ѡA�ХH�t�κ޲z���������s���楻�妸�ɡC
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
echo ���b���� PowerToys...

start /B "window" "%ptPath%\PowerToys.exe"

echo.
echo �w�˧���
echo �p�����`�Ц^�����@��
pause