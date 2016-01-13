## WeChat.NET#

WeChat.NET is based on web wechat.http://wx.qq.com

see more: http://www.cnblogs.com/xiaozhi_5638

## overview

- this repository is cloned from https://github.com/sherlockchou86/WeChat.NET

## can do and cant do

**can do**

- login with qrcode;
- log all incoming msg into wechat.log file

**cant do**

- nothing else

## step by step

1. get session id. like "gZ3bQtahGg==";
2. get qrcode using session id;
3. show the qrcode;
4. check weather the user scan the qrcode or not;
5. initialize latest contacts;
6. loading all friends;
7. check the status continuously.

## thanks and references

1. json.net
2. webkit.net
3. wx.qq.com
4. chrome

**all source code follow the [MIT][1] license.**