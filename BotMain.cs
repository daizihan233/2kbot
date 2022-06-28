using Manganese.Text;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Events.Concretes.Request;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Shared;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Mirai.Net_2kBot.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;

namespace Mirai.Net_2kBot
{
    public class BotMain
    {
        public static async Task Main()
        {
            var bot = new MiraiBot
            {
                Address = "localhost:8080",
                QQ = global.qq,
                VerifyKey = global.verify_key
            };
            // 注意: `LaunchAsync`是一个异步方法，请确保`Main`方法的返回值为`Task`
            await bot.LaunchAsync();

            //初始化
            if (System.IO.File.Exists("ops.txt"))
            {

            }
            else
            {
                System.IO.File.Create("ops.txt").Close();
            }
            if (System.IO.File.Exists("blocklist.txt"))
            {

            }
            else
            {
                System.IO.File.Create("blocklist.txt").Close();
            }
            // 在这里添加你的代码，比如订阅消息/事件之类的
            //持续更新op/黑名单
            bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(x =>
            {
                global.ops = System.IO.File.ReadAllLines("ops.txt");
                global.blocklist = System.IO.File.ReadAllLines("blocklist.txt");
                Thread.Sleep(500);
            });
            //戳一戳效果
            bot.EventReceived
            .OfType<NudgeEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Target == "2810482259" && receiver.Subject.Kind == "Group")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Subject.Id, "狗日的，你tm还有脸戳我？");
                    }
                    catch { }
                }
                else if (receiver.Target == "2810482259" && receiver.Subject.Kind == "Friend")
                {
                    await MessageManager.SendFriendMessageAsync(receiver.Subject.Id, "cnmlgbd，还跑到私信里来了？"); ;
                }
            });
            //bot加群
            bot.EventReceived
            .OfType<NewInvitationRequestedEvent>()
            .Subscribe(async e =>
            {
                if (e.FromId == "2548452533")
                {
                    //同意邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Approve, "");
                    Console.WriteLine("机器人已同意加入 " + e.GroupId);
                }
                else
                {
                    //拒绝邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Reject, "我都还没准备好，我为什么要进来？");
                    Console.WriteLine("机器人已拒绝加入 " + e.GroupId);
                }
            });
            //侦测改名
            bot.EventReceived
            .OfType<MemberCardChangedEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Current != "")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, "QQ号：" + receiver.Member.Id + "\r\n" + "原昵称：" + receiver.Origin + "\r\n" + "新昵称：" + receiver.Current);
                    }
                    catch { }
                }

            });
            //侦测撤回
            bot.EventReceived
           .OfType<GroupMessageRecalledEvent>()
           .Subscribe(async receiver =>
           {
               var messageChain = new MessageChainBuilder()
                .At(receiver.Operator.Id)
                .Plain(" 你又撤回了什么见不得人的东西？")
                .Build();
               if (receiver.AuthorId != receiver.Operator.Id)
               {
                   if (receiver.Operator.Permission.ToString() != "Administrator" && receiver.Operator.Permission.ToString() != "Owner")
                   {
                       try
                       {
                           await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                       }
                       catch { }
                   }
               }
               else
               {
                   try
                   {
                       await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                   }
                   catch { }
               }
           });
            //侦测踢人
            bot.EventReceived
            .OfType<MemberKickedEvent>()
            .Subscribe(async receiver =>
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, receiver.Member.Name + " (" + receiver.Member.Id + ") " + "被踢出去辣，最好滚得远远的！");
                }
                catch { }
            });
            //侦测退群
            bot.EventReceived
            .OfType<MemberLeftEvent>()
            .Subscribe(async receiver =>
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, receiver.Member.Name + " (" + receiver.Member.Id + ") " + "退群辣，祝一路走好！");
                }
                catch { }
            });
            //侦测入群
            bot.EventReceived
            .OfType<MemberJoinedEvent>()
            .Subscribe(async receiver =>
            {
                var messageChain = new MessageChainBuilder()
               .At(receiver.Member.Id)
               .Plain(" 来辣，让我们一起撅新人！（bushi")
               .Build();
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, messageChain);
                }
                catch { }
                if (global.blocklist.Contains(receiver.Member.Group.Id))
                {
                    try
                    {
                        await GroupManager.KickAsync(receiver.Member.Id, receiver.Member.Group.Id);
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, receiver.Member.Id + " 在黑名单内，已经被踢出！");
                        }
                        catch { }
                    }
                    catch
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, receiver.Member.Id + " 在黑名单内，但是无法踢出！（可能是没有足够权限）");
                        }
                        catch { }
                    }
                }
            });
            //bot对接收消息的处理
            bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(async x =>
            {
                //复读机
                Repeat.Execute(x);
                //surprise
                if (x.MessageChain.GetPlainMessage() == "/surprise")
                {
                    var chain = new MessageChainBuilder()
                         .VoiceFromPath(global.path + "/ysxb.slk")
                         .Build();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, chain);
                    }
                    catch { }
                }
                //随机图片
                if (x.MessageChain.GetPlainMessage() == "/rphoto")
                {
                    Random r = new Random();
                    string url = "";
                    int chance = 3;
                    int choice = r.Next(chance);
                    if (choice == chance - 1)
                    {
                        url = "https://www.dmoe.cc/random.php";
                    }
                    else
                    {
                        url = "https://source.unsplash.com/random";
                    }
                    var chain = new MessageChainBuilder()
                         .ImageFromUrl(url)
                         .Build();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "图片在来的路上...");
                    }
                    catch { }
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, chain);
                    }
                    catch
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "图片好像不见了！再等等吧？");
                        }
                        catch { }
                    }
                }
                //菜单与帮助
                Help.Execute(x);
                //叫人
                //引入模块
                var Call = new Call();
                if (x.MessageChain.GetPlainMessage().StartsWith("/call") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 3)
                    {
                        try
                        {
                            if (text[2].ToInt32() >= 1)
                            {
                                Call.Execute(text[1], x.GroupId, text[2].ToInt32());
                            }
                            else if (text[2].ToInt32() < 1)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "nmd，这个数字是几个意思？");
                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "油饼食不食？");
                            }
                            catch { }
                        }
                    }
                    else if (text.Length == 2)
                    {
                        if (ja.Count == 4)
                        {
                            string target = ja[2]["target"].ToString();
                            string t = ja[3]["text"].ToString().Replace(" ", "");
                            int time = t.ToInt32();
                            try
                            {
                                if (time >= 1)
                                {
                                    Call.Execute(target, x.GroupId, time);
                                }
                                else if (time < 1)
                                {
                                    try
                                    {
                                        await MessageManager.SendGroupMessageAsync(x.GroupId, "nmd，这个数字是几个意思？");
                                    }
                                    catch { }
                                }
                            }
                            catch
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "油饼食不食？");
                                }
                                catch { }
                            }
                        }
                        else if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            Call.Execute(target, x.GroupId, 3);
                        }
                        else
                        {
                            Call.Execute(text[1], x.GroupId, 3);
                        }
                    }
                    else if (text.Length < 2)
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //鸣谢
                if (x.MessageChain.GetPlainMessage() == "鸣谢")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "感谢Leobot和Hanbot给我的启发，感谢Leo给我提供C#的技术支持，也感谢Setup群各位群员对我的支持！");
                    }
                    catch { }
                }
                //精神心理疾病科普
                MentalHealth.Execute(x);
                //处理“你就是歌姬吧”（祖安）
                Zuan.Execute(x);
                //群管功能
                //引入模块
                var admin = new Admin();
                //禁言
                if (x.MessageChain.GetPlainMessage().StartsWith("/mute") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length != 1)
                    {
                        if (text.Length == 3)
                        {
                            admin.Mute(x.Sender.Id, text[1], x.GroupId, text[2].ToInt32());
                        }
                        else if (text.Length == 2)
                        {
                            if (ja.Count == 4)
                            {
                                string target = ja[2]["target"].ToString();
                                string t = ja[3]["text"].ToString().Replace(" ", "");
                                if (t == "")
                                {
                                    admin.Mute(x.Sender.Id, target, x.GroupId, 10);
                                }
                                else
                                {
                                    int time = t.ToInt32();
                                    admin.Mute(x.Sender.Id, target, x.GroupId, time);
                                }
                            }
                            else if (ja.Count == 3)
                            {
                                string target = ja[2]["target"].ToString();
                                admin.Mute(x.Sender.Id, target, x.GroupId, 10);
                            }
                            else
                            {
                                admin.Mute(x.Sender.Id, text[1], x.GroupId, 10);
                            }
                        }
                    }
                    else if (text.Length < 2)
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //解禁
                if (x.MessageChain.GetPlainMessage().StartsWith("/unmute") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Unmute(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Unmute(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //踢人
                if (x.MessageChain.GetPlainMessage().StartsWith("/kick") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Kick(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Kick(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //加黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/block") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Block(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Block(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //解黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/unblock") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Unblock(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Unblock(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //给予机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/op") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Op(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Op(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //夺走机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/deop") == true)
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1);//正常获取jobject
                    string[] text = ja[1]["text"].ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        if (ja.Count == 3)
                        {
                            string target = ja[2]["target"].ToString();
                            admin.Deop(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin.Deop(x.Sender.Id, text[1], x.GroupId);
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                //版本
                if (x.MessageChain.GetPlainMessage() == "版本")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "机器人版本：b1.0.0\r\n上次更新日期：2022/6/28\r\n更新内容：恭喜机器人进入beta阶段！此次没有重大更新，但机器人代码实现了模块化，方便后期维护");
                    }
                    catch { }
                }
            });
            // code

            // 然后在这之后卡住主线程（也可以使用别的方式，文档假设阅读者是个C#初学者）
            while (true)
            {
                if (Console.ReadLine().ToLower() == "exit")
                {
                    return;
                }
            }
        }
    }
}