pushd "%~dp0/../Agent/Windows"
dotnet msbuild -t:PublishAllRids
set agentWindowsEX=%ERRORLEVEL%
popd

pushd "%~dp0/../Modules/Security/Windows"
dotnet msbuild -t:PublishAllRids
set securityWindowsEX=%ERRORLEVEL%
popd

pushd "%~dp0/../Modules/Security/WindowsIoTCore"
dotnet msbuild -t:PublishAllRids
set securityIoTEX=%ERRORLEVEL%
popd

pushd "%~dp0/../Modules/Security/Linux"
dotnet msbuild -t:PublishAllRids
set securityLinuxEX=%ERRORLEVEL%
popd

if "%agentWindowsEX%" neq "0" GOTO buildFailed
if "%securityWindowsEX%" neq "0" GOTO buildFailed
if "%securityIoTEX%" neq "0" GOTO buildFailed
if "%securityLinuxEX%" neq "0" GOTO buildFailed

echo "Build passed"
set combinedEX="0"

GOTO end

:buildFailed
echo "Build failed"
set combinedEX="1"

:end
exit /B %EX%