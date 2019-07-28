pushd "%~dp0\.."
dotnet restore --source https://api.nuget.org/v3/index.json
set EX=%ERRORLEVEL%
popd

if "%EX%" neq "0" (
    echo "Restore failed"
)

exit /B %EX%
