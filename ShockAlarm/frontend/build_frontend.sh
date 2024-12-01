npm i
npm run build
rm -rf ../PhantomPursuit/PhantomPursuit/bin/Release/net6.0/frontend
rm -rf ../PhantomPursuit/PhantomPursuit/bin/Debug/net6.0/frontend
mkdir -p ../PhantomPursuit/PhantomPursuit/bin/Release/net6.0/frontend
mkdir -p ../PhantomPursuit/PhantomPursuit/bin/Debug/net6.0/frontend
cp -r build/* ../PhantomPursuit/PhantomPursuit/bin/Release/net6.0/frontend/
cp -r build/* ../PhantomPursuit/PhantomPursuit/bin/Debug/net6.0/frontend/