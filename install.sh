echo ~Welcome Stargazer~

echo fetching build files...
git clone https://github.com/vividuwu/lodestar

cd lodestar

echo building...
dotnet build -c Release -r linux-x64 --self-contained

echo installing...
mkdir /opt/lodestar/ 
cp ./bin/Release/net7.0/linux-x64/* /opt/lodestar
cd ../
rm -r ./lodestar

touch /usr/bin/lodestar
echo "/opt/lodestar/Mercurius" >> /usr/bin/lodestar

chmod +x /usr/bin/lodestar

echo Installation Complete!