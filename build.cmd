dotnet restore

@echo off
cd src

@echo off
cd BookMate.Core
IF NOT EXIST "appconfiguration.json" copy NUL "appconfiguration.json"
dotnet build

@echo off
cd..
cd BookMate.Integration
dotnet build

@echo off
cd..
cd BookMate.NLPServices
dotnet build

@echo off
cd..
cd BookMate.Processing
dotnet build

@echo off
cd..
cd BookMate.Web/server
dotnet build
@echo off
cd..

cd..
@echo off
cd BookMate.ConsoleClient
dotnet build

@echo off
cd..
@echo off
cd..