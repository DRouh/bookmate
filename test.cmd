@echo off
cd tests

@echo off
cd BookMate.Core.Tests 
call dotnet test 
@echo off
cd..

@echo off
cd BookMate.Integration.Tests 
call dotnet test 
@echo off
cd..

@echo off
cd BookMate.Processing.Tests 
call dotnet test 
@echo off
cd..

cd..