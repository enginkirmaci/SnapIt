rmdir /s /q build

dotnet publish ../SnapScreen -c Standalone -a x86 -o ./build --self-contained false