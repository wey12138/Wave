powershell.exe -nologo -noprofile -command "& { If (Test-Path '.\tests\Sample Data\Geodatabases'){ Remove-Item '.\tests\Sample Data\Geodatabases' -Force -Recurse}; Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('.\tests\Sample Data\Geodatabase_931SP1.zip', '.'); }"
powershell.exe -nologo -noprofile -command "& { If (Test-Path '.\tests\Sample Data\Databases'){ Remove-Item '.\tests\Sample Data\Databases' -Force -Recurse}; Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('.\tests\Sample Data\Database_931SP1.zip', '.'); }"