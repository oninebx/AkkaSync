## Publish AkkaSync Docker
git tag v0.1.0
git push origin v0.1.0

## Publish AkkaSync Demo Assets
git tag assets-0.1.0
git push origin assets-0.1.0

## Delete tag
git tag -d v0.1.0-demo.1
git push origin --delete v0.1.0-demo.1

## Run locally
dotnet run --project publish/AkkaSync.Publish/AkkaSync.Publish.csproj --configuration Release