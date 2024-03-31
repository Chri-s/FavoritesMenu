del /Q Setup\en-US\*
dotnet.exe build -c Release-Dependent -p Platform=x64 -o Setup src\Setup
dotnet.exe build -c Release-Standalone -p Platform=x64 -o Setup src\Setup
powershell Compress-Archive -Force -CompressionLevel Optimal src\FavoritesMenu\bin\Release\net8.0-windows\Dependent\* Setup\en-US\FavoritesMenu.Dependent.zip
powershell Compress-Archive -Force -CompressionLevel Optimal src\FavoritesMenu\bin\Release\net8.0-windows\Standalone\* Setup\en-US\FavoritesMenu.Standalone.zip