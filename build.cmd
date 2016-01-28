@ECHO OFF

SETLOCAL

CALL ..\..\..\build\set35vars.bat

%msbuildexe% Cyotek.Drawing.Imaging.Farbfeld.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build
CALL signcmd src\Cyotek.Drawing.Imaging.Farbfeld\bin\Release\Cyotek.Drawing.Imaging.Farbfeld.dll

PUSHD %CD%

MD nuget > NUL
CD nuget

NUGET pack ..\src\Cyotek.Drawing.Imaging.Farbfeld\Cyotek.Drawing.Imaging.Farbfeld.csproj -Prop Configuration=Release

POPD

ENDLOCAL
