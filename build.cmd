dotnet restore

dir 

cd src

cd BookMate.Core
IF NOT EXIST "appconfiguration.json" copy NUL "appconfiguration.json"
dotnet build

cd..
cd BookMate.Integration
dotnet build

cd..
cd BookMate.NLPServices
dotnet build

cd..
cd BookMate.Processing
dotnet build

cd..
cd BookMate.Web/server
dotnet build
cd..

cd..
cd BookMate.ConsoleClient
dotnet build