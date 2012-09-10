msbuild Modular.Mvc\Modular.Mvc.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath="bin\Release"
packages\NuGet.CommandLine.2.0.40001\tools\NuGet.exe pack Modular.Mvc\Modular.Mvc.nuspec
packages\NuGet.CommandLine.2.0.40001\tools\NuGet.exe push *.nupkg

