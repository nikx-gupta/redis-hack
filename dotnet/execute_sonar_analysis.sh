#!/bin/bash

SONAR_PROJECT_KEY=hack-mongo
SONAR_URL=$1
SONAR_TOKEN=$2
SONAR_ZIP=sonar-scanner-msbuild-5.1.0.28487-net5.0.zip

rm -rf sonar
wget -O sonar.zip https://github.com/SonarSource/sonar-scanner-msbuild/releases/download/5.1.0.28487/"$SONAR_ZIP"
unzip -d sonar sonar.zip
chmod +x -R sonar

dotnet sonar/SonarScanner.MSBuild.dll begin /k:"$SONAR_PROJECT_KEY" /d:sonar.host.url="$SONAR_URL" /d:sonar.login="$SONAR_TOKEN" 
dotnet restore
dotnet build src/Service/Hack.Service.MongoDb.csproj
dotnet sonar/SonarScanner.MSBuild.dll end /d:sonar.login="$SONAR_TOKEN"

