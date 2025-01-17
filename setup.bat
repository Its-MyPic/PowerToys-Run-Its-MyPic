@echo off
set new_folder=%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins\Its-MyPic
md "%new_folder%"
echo.
copy * "%new_folder%"
echo.
echo 安裝完成
echo 如有異常請回報給作者
pause