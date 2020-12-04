dotnet restore
dotnet pack .\GMO.Versioning\ -t:Build,Pack --include-symbols -p:Configuration=Release -o .
$pkg = gci *.nupkg 
nuget push $pkg -Source https://www.nuget.org/api/v2/package -NonInteractive
