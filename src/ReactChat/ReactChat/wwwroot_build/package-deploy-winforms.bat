IF EXIST staging-winforms\ (
RMDIR /S /Q .\staging-winforms
)

MKDIR staging-winforms

SET TOOLS=.\tools
SET RELEASE=..\..\ReactChat.AppWinForms\bin\x86\Release
COPY %RELEASE%\ReactChat.AppWinForms.exe .\staging-winforms
COPY %RELEASE%\ReactChat.AppWinForms.exe.config .\staging-winforms
COPY %RELEASE%\CefSharp.BrowserSubprocess.exe .\staging-winforms
ROBOCOPY "%RELEASE%" ".\staging-winforms" *.dll *.pak *.dat /E

IF NOT EXIST apps (
mkdir apps
)

IF EXIST ReactChat-winforms.7z (
del ReactChat-winforms.7z
)

IF EXIST ReactChat-winforms.exe (
del ReactChat-winforms.exe
)

cd tools && 7za a ..\ReactChat-winforms.7z ..\staging-winforms\* && cd..
copy /b .\tools\7zsd_All.sfx + config-winforms.txt + ReactChat-winforms.7z .\apps\ReactChat-winforms.exe