$build = $env:APPVEYOR_BUILD_NUMBER
$version = "preview.$build"

dotnet pack src/MS.RestApi/MS.RestApi.csproj -o ../build/nuget -c Release --version-suffix $version
dotnet pack src/MS.RestApi.Client/MS.RestApi.Client.csproj -o ../build/nuget -c Release --version-suffix $version
dotnet pack src/MS.RestApi.Server/MS.RestApi.Server.csproj -o ../build/nuget -c Release --version-suffix $version
dotnet pack src/MS.RestApi.SourceGenerator/MS.RestApi.SourceGenerator.csproj -o ../build/nuget -c Release --version-suffix $version
