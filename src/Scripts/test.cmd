cd %~dp0

dotnet test --logger "trx;LogFileName=AgentCore.trx" --results-directory TestResults ../Agent/Tests/Core
set agentCoreEX=%ERRORLEVEL%

dotnet test --logger "trx;LogFileName=SecurityCommon.trx" --results-directory TestResults ../Modules/Security/Tests/Common
set securityCommonEX=%ERRORLEVEL%

dotnet test --logger "trx;LogFileName=SecurityWindows.trx" --results-directory TestResults ../Modules/Security/Tests/Windows
set securityWindowsEX=%ERRORLEVEL%

if "%agentCoreEX%" neq "0" GOTO testsFailed
if "%securityCommonEX%" neq "0" GOTO testsFailed
if "%securityWindowsEX%" neq "0" GOTO testsFailed

echo "Tests passed"
set combinedEX="0"

GOTO end

:testsFailed
echo "Tests failed"
set combinedEX="1"

:end
exit /B %EX%