echo "~Welcome Stargazer~"

echo fetching build files...
git clone https://github.com/vividuwu/lodestar

cd lodestar

echo building...
dotnet build -c Release

echo installing
mkdir /usr/bin