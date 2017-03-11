echo off
set disk=%1
mongod --remove
mongod --dbpath=%disk%:\mongodb --logpath=%disk%:\mongodb\log.txt --install
net start MongoDB