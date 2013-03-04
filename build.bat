mkdir out
set SRC=src\Main\bin\Release\
%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe src\CodeSaber.sln /p:Configuration=Release
copy /y %SRC%CodeSaber.exe out
copy /y %SRC%CodeSaber.Shrepl.dll out
copy /y %SRC%CodeSaber.Ice.exe out
copy /y %SRC%ICSharpCode.AvalonEdit.dll out
copy /y %SRC%ICSharpCode.NRefactory.dll out
pause