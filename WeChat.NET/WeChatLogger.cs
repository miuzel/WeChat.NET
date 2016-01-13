using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.NET.Controls;
using WeChat.NET.HTTP;
using WeChat.NET.Objects;
using WeChat.NET.util;
using log4net;

namespace WeChat.NET
{
    class WeChatLogger
    {
        private WXUser _me;
        private WChatBox _chat2friend;
        private WPersonalInfo _friendInfo;
        private List<Object> _contact_all = new List<object>();
        private List<object> _contact_latest = new List<object>();
        protected static ILog log = log4net.LogManager.GetLogger("WeChatLog.Log");

        public void MainLogic()
        {
            WXService wxs = new WXService();
            JObject init_result = wxs.WxInit();  //初始化

            List<object> contact_all = new List<object>();
            if (init_result != null)
            {
                _me = new WXUser();
                _me.UserName = init_result["User"]["UserName"].ToString();
                _me.City = "";
                _me.HeadImgUrl = init_result["User"]["HeadImgUrl"].ToString();
                _me.NickName = init_result["User"]["NickName"].ToString();
                _me.Province = "";
                _me.PYQuanPin = init_result["User"]["PYQuanPin"].ToString();
                _me.RemarkName = init_result["User"]["RemarkName"].ToString();
                _me.RemarkPYQuanPin = init_result["User"]["RemarkPYQuanPin"].ToString();
                _me.Sex = init_result["User"]["Sex"].ToString();
                _me.Signature = init_result["User"]["Signature"].ToString();

                foreach (JObject contact in init_result["ContactList"])  //部分好友名单
                {
                    WXUser user = new WXUser();
                    user.UserName = contact["UserName"].ToString();
                    user.City = contact["City"].ToString();
                    user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                    user.NickName = contact["NickName"].ToString();
                    user.Province = contact["Province"].ToString();
                    user.PYQuanPin = contact["PYQuanPin"].ToString();
                    user.RemarkName = contact["RemarkName"].ToString();
                    user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                    user.Sex = contact["Sex"].ToString();
                    user.Signature = contact["Signature"].ToString();

                    _contact_latest.Add(user);
                }
            }

            JObject contact_result = wxs.GetContact(); //通讯录
            if (contact_result != null)
            {
                foreach (JObject contact in contact_result["MemberList"])  //完整好友名单
                {
                    WXUser user = new WXUser();
                    user.UserName = contact["UserName"].ToString();
                    user.City = contact["City"].ToString();
                    user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                    user.NickName = contact["NickName"].ToString();
                    user.Province = contact["Province"].ToString();
                    user.PYQuanPin = contact["PYQuanPin"].ToString();
                    user.RemarkName = contact["RemarkName"].ToString();
                    user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                    user.Sex = contact["Sex"].ToString();
                    user.Signature = contact["Signature"].ToString();

                    contact_all.Add(user);
                }
            }
            IOrderedEnumerable<object> list_all = contact_all.OrderBy(e => (e as WXUser).ShowPinYin);

            WXUser wx; string start_char;
            foreach (object o in list_all)
            {
                wx = o as WXUser;
                start_char = wx.ShowPinYin == "" ? "" : wx.ShowPinYin.Substring(0, 1);
                if (!_contact_all.Contains(start_char.ToUpper()))
                {
                    _contact_all.Add(start_char.ToUpper());
                }
                _contact_all.Add(o);
            }


            JObject sync_flag;
            JObject sync_result;
            while (true)
            {
                sync_flag = wxs.WxSyncCheck();  //同步检查
                if (sync_flag == null)
                {
                    continue;
                }
                else if (sync_flag["retcode"].ToString() != "0")
                {
                    return;
                }
                //这里应该判断 sync_flag中selector的值
                else //有消息
                {
                    sync_result = wxs.WxSync();  //进行同步
                    if (sync_result != null)
                    {
                        if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
                        {
                            Console.WriteLine(System.DateTime.Now.ToString() + sync_result["AddMsgCount"].ToString() + " Msg(s) Received");
                            foreach (JObject m in sync_result["AddMsgList"])
                            {
                                LiveClient.PrintData(m.ToString());
                                LiveClient.PrintData(",\n");
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
