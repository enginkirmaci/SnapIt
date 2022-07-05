rmdir /s /q build

dotnet publish ../SnapIt -c Standalone -a x86 -o ./build --self-contained false

"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" innoSetup.iss /DMyAppVersion=4.2.0.0