IF EXIST staging-console (
RMDIR /S /Q .\staging-console
)

MKDIR staging-console

SET TOOLS=.\tools
SET OUTPUTNAME=ReactChat.Console.exe
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\..\ReactChat.AppConsole\bin\x86\Release
SET INPUT=%RELEASE%\ReactChat.AppConsole.exe
SET INPUT=%INPUT% %RELEASE%\ReactChat.Resources.dll
SET INPUT=%INPUT% %RELEASE%\ReactChat.ServiceInterface.dll
SET INPUT=%INPUT% %RELEASE%\ReactChat.ServiceModel.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Text.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Client.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Common.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Interfaces.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.OrmLite.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Redis.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Razor.dll
SET INPUT=%INPUT% %RELEASE%\System.Web.Razor.dll

%ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:staging-console\%OUTPUTNAME% /ndebug %INPUT% 

IF NOT EXIST apps (
mkdir apps
)

IF EXIST ReactChat-console.7z (
del ReactChat-console.7z
)

IF EXIST ReactChat-console.exe (
del ReactChat-console.exe
)

cd tools && 7za a ..\ReactChat-console.7z ..\staging-console\* && cd..
copy /b .\tools\7zsd_All.sfx + config-console.txt + ReactChat-console.7z .\apps\ReactChat-console.exe