nuget restore
nuget pack .\GMO.Versioning\ -build -Symbols -SymbolPackageFormat snupkg
$pkg = gci *.nupkg 
nuget push $pkg -Source https://www.nuget.org/api/v2/package -NonInteractive
