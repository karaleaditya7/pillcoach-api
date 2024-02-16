#!/bin/bash
echo "Login to Azure"
az login -u darshanj@heaptrace.com -p Heaptrace@2018
echo "Building API"
# dotnet clean --configuration Release
dotnet build
echo "publishing API changes"
dotnet publish OntrackDb.sln --configuration Release --output ./ontrackrx --no-build
echo "zipping the changes"
zip -r ontrackrx.zip ontrackrx
echo "Deploying to app service"
az webapp deploy --resource-group OntrackRxResourcegroup --name ontrackrxstagingapi --type zip --clean true --src-path ./ontrackrx.zip


