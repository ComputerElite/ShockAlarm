npm i
npm run build
rm -rf ../bin/Release/net8.0/frontend
rm -rf ../bin/Debug/net8.0/frontend
mkdir -p ../bin/Release/net8.0/frontend
mkdir -p ../bin/Debug/net8.0/frontend
cp -r build/* ../bin/Release/net8.0/frontend/
cp -r build/* ../bin/Debug/net8.0/frontend/