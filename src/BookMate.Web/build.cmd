if not exist %~dp0\node_modules call npm install

call webpack

cd server
dotnet restore
dotnet watch run
cd..