npm i
npm run build
rm -rf ../bin/Release/net6.0/frontend
rm -rf ../bin/Debug/net6.0/frontend
mkdir -p ../bin/Release/net6.0/frontend
mkdir -p ../bin/Debug/net6.0/frontend
cp -r build/* ../bin/Release/net6.0/frontend/
cp -r build/* ../bin/Debug/net6.0/frontend/