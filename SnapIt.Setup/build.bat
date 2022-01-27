rmdir /s /q build

dotnet publish ../SnapIt -c Standalone -a x86 -o ./build --self-contained false