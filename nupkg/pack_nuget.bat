REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Core\Acb.Core.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Configuration\Acb.Configuration.csproj &
dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Dapper\Acb.Dapper.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Redis\Acb.Redis.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.MongoDb\Acb.MongoDb.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Framework\Acb.Framework.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.WebApi\Acb.WebApi.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Dapper.Mysql\Acb.Dapper.MySql.csproj &
REM dotnet pack -c Release -o ..\..\nupkg ..\src\Acb.Dapper.PostgreSql\Acb.Dapper.PostgreSql.csproj

REM pack
dotnet pack -c Release  /p:PackageVersion=3.0.${BUILD_NUMBER} .\Acb.Market.Contracts\Acb.Market.Contracts.csproj
REM push
dotnet nuget push .\Acb.Market.Contracts\bin\Release\Acb.Market.Contracts.3.0.${BUILD_NUMBER}.nupkg -k ed97c8430153a668 -s http://192.168.0.110:82
REM delete
dotnet nuget delete Acb.Market.Contracts 3.0.0 -k ed97c8430153a668 -s http://192.168.0.110:82 --non-interactive