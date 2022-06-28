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
using RestSharp;
using System.Reactive.Linq;

namespace Mirai.Net_2kBot
{
    public static class global
    {
        public static long last_call;
        public static long time_now;
        public static int cd = 40;
        public static string path = Directory.GetCurrentDirectory();

        public static string[] ops;
        public static string[] blocklist;
        public static string[] ignores =
                {
                    "748029973",
                    "2265453790",
                    "2286003479",
                    "3594648576",
                    "3573523379."
                };
        public static string api_key = "";
    };
    public static class admin_functions
    {
        //禁言功能
        public static async void Mute(string executor, string victim, string group, int minutes)
        {
            if (global.ops.Contains(executor))
            {
                if (global.ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.MuteAsync(victim, group, minutes * 60);
                        await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 禁言 " + minutes + " 分钟");
                    }
                    catch
                    {
                        try
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                            }
                            catch
                            {
                                Console.WriteLine("执行失败！正在调用api...");
                            }
                            var client = new RestClient("http://101.42.94.97/guser");
                            var request = new RestRequest("nobb?uid=" + victim + "&gid=" + group + "&tim=" + (minutes * 60) + "&key=" + global.api_key, Method.Post);
                            request.Timeout = 10000;
                            RestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);
                        }
                        catch
                        {
                            Console.WriteLine("你甚至连api都调用不了");
                        }
                    }
                }
                else
                {
                    await MessageManager.SendGroupMessageAsync(group, "此人是机器人管理员，无法禁言");
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //解禁功能
        public static async void Unmute(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor))
            {
                try
                {
                    await GroupManager.UnMuteAsync(victim, group);
                    await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 解除禁言");
                }
                catch
                {
                    try
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                        }
                        catch
                        {
                            Console.WriteLine("执行失败！正在调用api...");
                        }
                        var client = new RestClient("http://101.42.94.97/guser");
                        var request = new RestRequest("nobb?uid=" + victim + "&gid=" + group + "&tim=0&key=" + global.api_key, Method.Post);
                        request.Timeout = 10000;
                        RestResponse response = client.Execute(request);
                        Console.WriteLine(response.Content);
                    }
                    catch
                    {
                        Console.WriteLine("你甚至连api都调用不了");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //踢人功能
        public static async void Kick(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor))
            {
                if (global.ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.KickAsync(victim, group);
                        await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 踢出");
                    }
                    catch
                    {
                        try
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                            }
                            catch
                            {
                                Console.WriteLine("执行失败！正在调用api...");
                            }
                            var client = new RestClient("http://101.42.94.97/guser");
                            var request = new RestRequest("del?key=" + global.api_key + "&uid=" + victim + "&gid=" + group, Method.Post);
                            request.Timeout = 10000;
                            RestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);
                        }
                        catch
                        {
                            Console.WriteLine("你甚至连api都调用不了");
                        }
                    }
                }
                else
                {
                    await MessageManager.SendGroupMessageAsync(group, "此人是机器人管理员，无法踢出");
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //加黑功能
        public static async void Block(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor) == true)
            {
                if (global.ops.Contains(victim) == false)
                {
                    if (global.blocklist.Contains(victim) == false)
                    {
                        using StreamWriter file = new("blocklist.txt", append: true);
                        await file.WriteLineAsync("\r\n" + victim);
                        file.Close();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 加入黑名单");
                        }
                        catch
                        {
                            Console.WriteLine("已将 " + victim + " 加入黑名单");
                        }
                        try
                        {
                            await GroupManager.KickAsync(victim, group);
                            var client = new RestClient("http://101.42.94.97/blacklist");
                            var request = new RestRequest("up?uid=" + victim + "&key=" + global.api_key, Method.Post);
                            request.Timeout = 10000;
                            RestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);
                        }
                        catch
                        {
                            try
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(group, "在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                catch
                                {
                                    Console.WriteLine("在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                var client = new RestClient("http://101.42.94.97/blacklist");
                                var request = new RestRequest("up?uid=" + victim + "&key=" + global.api_key, Method.Post);
                                request.Timeout = 10000;
                                RestResponse response = client.Execute(request);
                                Console.WriteLine(response.Content);
                                var client1 = new RestClient("http://101.42.94.97/guser");
                                var request1 = new RestRequest("del?key=" + global.api_key + "&uid=" + victim + "&gid=" + group, Method.Post);
                                request.Timeout = 10000;
                                RestResponse response1 = client.Execute(request);
                                Console.WriteLine(response.Content);
                            }
                            catch
                            {
                                Console.WriteLine("你甚至连api都调用不了");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, victim + " 已经在黑名单内");
                        }
                        catch
                        {
                            Console.WriteLine(victim + " 已经在黑名单内");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 是机器人管理员，不能加黑");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 是机器人管理员，不能加黑");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //给OP功能
        public static async void Op(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor) == true)
            {
                if (global.ops.Contains(victim) == false)
                {
                    using StreamWriter file = new("ops.txt", append: true);
                    await file.WriteLineAsync("\r\n" + victim);
                    file.Close();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 设置为机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("已将 " + victim + " 设置为机器人管理员");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 已经是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 已经是机器人管理员");
                    }
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch { }
            }
        }
        //解黑功能
        public static async void Unblock(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor) == true)
            {
                if (global.blocklist.Contains(victim) == true)
                {
                    var blocklist_old = global.blocklist;
                    var blocklist_new = global.blocklist.Where(line => !line.Contains(victim));
                    System.IO.File.WriteAllLines("blocklist.txt", blocklist_new);
                    await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 移出黑名单");
                    var client = new RestClient("http://101.42.94.97/blacklist");
                    var request = new RestRequest("del?uid=" + victim + "&key=" + global.api_key, Method.Delete);
                    request.Timeout = 10000;
                    RestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 不在黑名单内");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 不在黑名单内");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //取消OP功能
        public static async void Deop(string executor, string victim, string group)
        {
            if (global.ops.Contains(executor) == true)
            {
                if (global.ops.Contains(victim) == true)
                {
                    var ops_old = global.ops;
                    var ops_new = global.ops.Where(line => !line.Contains(victim));
                    System.IO.File.WriteAllLines("ops.txt", ops_new);
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "已取消 " + victim + " 的机器人管理员权限");
                    }
                    catch
                    {
                        Console.WriteLine("已取消 " + victim + " 的机器人管理员权限");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 不是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 不是机器人管理员");
                    }
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch { }
            }
        }
    }
    public static class functions
    {
        //叫人功能
        public static async void Call(string victim, string group, int times)
        {
            if (times > 10)
            {
                times = 10;
            }
            var messageChain = new MessageChainBuilder()
                               .At(victim)
                               .Plain(" 机器人正在呼叫你")
                               .Build();
            global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (global.time_now - global.last_call >= global.cd)
            {
                for (int i = 0; i < times; i++)
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, messageChain);
                        global.last_call = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    }
                    catch { }
                }
            }
            else
            {
                global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "CD未到，请别急！CD还剩： " + (global.cd - (global.time_now - global.last_call)).ToString() + " 秒");
                }
                catch { }
            }
        }
    }
    public class BotMain
    {
        public static async Task Main()
        {
            var bot = new MiraiBot
            {
                Address = "localhost:8080",
                QQ = "",
                VerifyKey = ""
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
                if (x.MessageChain.GetPlainMessage().StartsWith("/echo") == true)
                {
                    string[] result = x.MessageChain.GetPlainMessage().Split(" ");
                    if (result.Length > 1)
                    {
                        try
                        {
                            string results = "";
                            if (global.ignores.Contains(x.Sender.Id) == false)
                            {
                                for (int i = 1; i < result.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        results = result[i];
                                    }
                                    else
                                    {
                                        results = results + " " + result[i];
                                    }
                                }
                            }
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, results);
                            }
                            catch { }
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
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "你个sb难道没发觉到少了些什么？");
                        }
                        catch { }
                    }
                }
                //主动复读
                string[] repeatwords =
                {
                    "114514",
                    "1919810",
                    "1145141919810",
                    "ccc",
                    "c",
                    "草",
                    "tcl",
                    "?",
                    "。",
                    "？",
                    "e",
                    "额",
                    "呃"
                };
                if (global.ignores.Contains(x.Sender.Id) == false)
                {
                    foreach (string item in repeatwords)
                    {
                        if (item.Equals(x.MessageChain.GetPlainMessage()))
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, x.MessageChain.GetPlainMessage());
                            }
                            catch { }
                        }
                    }
                }
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
                    if (choice == chance-1)
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
                //菜单
                if (x.MessageChain.GetPlainMessage() == "菜单" || x.MessageChain.GetPlainMessage() == "/menu")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "2kbot菜单\r\n" +
                        "1.群管系统\r\n" +
                        "2.复读机\r\n" +
                        "3.精神心理疾病科普\r\n" +
                        "4.量表测试\r\n" +
                        "5.叫人功能\r\n" +
                        "详情请用/help指令");
                    }
                    catch { }
                }
                //帮助
                var indexs = new List<string>
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                };
                var contents = new List<string>
                {
                    "群管功能\r\n禁言：/mute <QQ号或at> [时间] （以分钟算）\r\n解除禁言：/unmute <QQ号或at>\r\n踢出：/kick <QQ号或at>\r\n加黑：/block <QQ号或at>\r\n（上述功能都需要机器人管理员）",
                    "该指令用于复述文本\r\n用法：/echo <文本>",
                    "该指令用于叫人\r\n用法：/call <QQ号或at> [次数]",
                    "发送“精神疾病”或者“心理疾病”并按照后续出现的选项发送相应文字即可获得科普文本",
                    "发送“量表”或者“测试”并按照后续出现的选项发送相应文字即可获得链接"
                };
                if (x.MessageChain.GetPlainMessage().StartsWith("/help") == true)
                {
                    string[] result = x.MessageChain.GetPlainMessage().Split(" ");
                    if (result.Length > 1)
                    {
                        foreach (string q in indexs)
                        {
                            try
                            {
                                if (result[1] == q)
                                {
                                    try
                                    {
                                        await MessageManager.SendGroupMessageAsync(x.GroupId, (contents[indexs.IndexOf(q)]));
                                    }
                                    catch { }
                                }
                                else if (result[1].ToInt32() > indexs.Count)
                                {
                                    try
                                    {
                                        await MessageManager.SendGroupMessageAsync(x.GroupId, "未找到相关帮助");
                                    }
                                    catch { }
                                    break;
                                }
                            }
                            catch
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "请写数字，不要写别的好吗？");
                                }
                                catch { }
                                break;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "目前有对于以下功能的帮助文档：\r\n[1]群管功能\r\n[2]/echo\r\n[3]/call\r\n[4]精神心理疾病科普\r\n[5]量表测试");
                        }
                        catch { }
                    }
                }
                //叫人
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
                                functions.Call(text[1], x.GroupId, text[2].ToInt32());
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
                                    functions.Call(target, x.GroupId, time);
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
                            functions.Call(target, x.GroupId, 3);
                        }
                        else
                        {
                            functions.Call(text[1], x.GroupId, 3);
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
                var disorders = new List<string>
                {
                    "焦虑症",
                    "强迫症",
                    "神经衰弱",
                    "恐惧症",
                    "抑郁症",
                    "双相情感障碍",
                    "精神分裂症",
                    "妄想症",
                    "分裂情感性障碍",
                    "厌食症",
                    "贪食症",
                    "孤独症",
                    "多动症"
                };
                var explanations = new List<string>
                {
                    "焦虑症是指在日常情况下，出现强烈、过度和持续的担忧和恐惧，可在几分钟之内达到顶峰。这种症状会干扰日常活动，难以控制。常见的焦虑症有广泛性焦虑障碍、惊恐障碍、社交恐惧症、特定恐惧症和分离焦虑障碍等。",
                    "强迫症是一种较为常见的精神疾病，以反复出现的强迫观念、强迫冲动或强迫行为等为主要表现。多数患者认为这些观念和行为不必要或不正常，违反了自己的意愿，但无法摆脱，为此深感焦虑和痛苦。",
                    "神经衰弱是在长期紧张和压力下，产生以脑和躯体功能衰弱为主要特征的一种心理疾病，主要表现为精神活动减弱，更易疲劳，注意力难集中，常伴有情绪易激惹、烦恼、紧张，睡眠障碍及肌肉紧张性疼痛等症状。",
                    "恐惧症又称恐怖症、恐怖性神经症，是一种以过分和不合理地惧怕外界某种客观事物或处境为主要表现的神经症。患者发作时常伴有明显的焦虑、自主神经紊乱和回避反应，患者难以自控，症状反复出现，以致影响其正常活动。分为广场恐惧症、社交恐惧症和特定恐惧症三种类型。",
                    "抑郁症是一种患病率高、临床治愈率高的精神障碍，但由于老百姓对该病认知不足，导致坚持接受正规治疗的患者较少，因此也有接受治疗率低、复发率高的特征。它以显著而持久的心境低落为主要特征，部分患者有存在自伤、自杀行为，可伴有妄想、幻觉等精神病性症状，严重时可能发生抑郁性木僵，可表现为面部表情固定、对刺激缺乏反应、话少甚至不言语、少动甚至不动等。抑郁症发作时一般表现为情绪低落、兴趣减退、精力缺乏等。",
                    "双相情感障碍又名双相障碍，是一类既有躁狂发作或者轻躁狂发作，又有抑郁发作（典型特征）的常见精神障碍，首次发病可见于任何年龄。躁狂发作时，患者有情感高涨、言语活动增多、精力充沛等表现；抑郁发作时，患者常表现出情绪低落、愉快感丧失、言语活动减少、疲劳迟钝等症状。患者的临床表现复杂，其复杂性体现在情绪低落或者高涨反复、交替、不规则呈现的同时，伴有注意力分散、轻率、夸大、思维奔逸、高反应性、睡眠减少和言语增多等紊乱症状。还常见焦虑、强迫、物质滥用，也可出现幻觉、妄想或紧张症状等精神病症状。病程多形演变，发作性、循环往复性、混合迁徙性、潮起潮落式病程不一而足，比如3个抑郁期跟着2个躁狂期。间歇期或长或短，间歇期社会功能相对正常，但也可有社会功能损害，多次反复发作后出现发作频率快、病情越发复杂的情况。",
                    "精神分裂症，定义为一种慢性的、严重的精神障碍，包括个人的感知觉、情感与行为的异常。患者很难区分出真实和想象，患者反应迟钝、行为退缩或过激，严重者难以进行正常社交。医学上，疾病分类体系定义它不是一种疾病，而是一种障碍。该病常常发作在青年或者壮年时期，发作时，身体感觉、思维逻辑、情感体验和行为表现等方面产生障碍，但是既不昏迷，也不智障。现阶段研究显示其可能为遗传、大脑结构、妊娠问题以及后天生活的家庭、周围环境因素共同激发，但具体发病机制及病因并未完全明确，因而这种疾病难以治愈，不过，通过适当的治疗手段可以控制病情。该病病期多漫长，约一半的患者因为精神的残疾状态，给自身、家庭、周围带来不同程度影响。",
                     "妄想性障碍是以长期（三个月及以上）持续性、系统性妄想为最主要临床特征的一组精神障碍。患者除了妄想症状外，少有其他精神病症状，其人格和智能通常可保待完整，在不涉及妄想的情况下，情感、言语和行为基本正常。妄想性障碍起病隐匿，病程演进缓慢，甚至可伴随患者终生。",
                     "分裂情感性障碍又称分裂情感性精神病，是一组精神分裂症状（幻觉、妄想等精神病性症状）和情感症状（躁狂、抑郁）同时存在或交替发生，症状又同样典型，常有反复发作的倾向。有人认为它是精神分裂症和情感性障碍的共病体，有人认为是精神分裂症和情感障碍连续谱系上的一个中点，也有人认为是伴有精神病性症状的情感障碍，而非一类独立的疾病。它是由遗传因素、生物化学等因素共同作用引起，对患者的精神、社会功能造成损伤，以药物治疗为主，辅以物理及心理治疗。",
                     "神经性厌食症简称厌食症，是一种慢性进食障碍的临床表现。神经性厌食症的原意为精神性食欲丧失，此症的主要特点为特殊的精神心理变态、以瘦为美的躯体形象障碍，自我造成的拒食、导吐或腹泻，极度的营养不良和消瘦、闭经，甚至死亡。",
                     "贪食症又称为神经性贪食症，是一种精神心理性进食障碍。这个病多见于女性，患者发病多在青春期或者成年早期，主要表现为反复发作、不可以控制的暴饮暴食的情况。暴饮暴食之后又常常采用自我催吐、催泻，以及禁食、过度运动等等不恰当的方式来过度的减肥，这些行为与患者对于自身的体重、体型的过度关注和不客观的评价有关系。",
                     "孤独症也称自闭症，是广泛性发育障碍中最常见、最具有代表性的疾病。该疾病起病于婴幼儿时期，以社会交往障碍、交流障碍、局限的兴趣、刻板与重复行为方式为主要临床表现，多数患儿还会伴有不同程度的精神发育迟滞。孤独症的患病率报道不一，一般认为约为儿童人口的2～5/万人，男女比例约为3～4：1，男孩是女孩的3-4倍。",
                     "注意缺陷多动障碍，俗称多动症，是一种起病于儿童时期，以与年龄水平不相称的注意缺陷、行为多动和情绪冲动为主要表现的神经发育障碍。ADHD多于学龄前起病，慢性病程，有70%的患儿症状持续到青春期，30%~50%持续到成年期。这些患儿常同时患有学习障碍、对立违抗性障碍、情绪障碍、以及适应障碍等，对其学业、工作、社会及家庭生活等方面产生广泛而消极的负面影响，给家庭和社会造成沉重的负担。"
                };
                if (x.MessageChain.GetPlainMessage() == "精神疾病" || x.MessageChain.GetPlainMessage() == "心理疾病")
                {
                    await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "精神心理疾病科普大全：\r\n" +
                        "1.神经症类\r\n" +
                        "焦虑症|强迫症\r\n" +
                        "神经衰弱|恐惧症\r\n" +
                        "2.情感障碍\r\n" +
                        "抑郁症|双相情感障碍\r\n" +
                        "3.精神病性障碍\r\n" +
                        "精神分裂症|妄想症\r\n" +
                        "分裂情感性障碍\r\n" +
                        "4.进食障碍\r\n" +
                        "厌食症|贪食症\r\n" +
                        "5.神经发育障碍\r\n" +
                        "孤独症|多动症");
                }
                foreach (string q in disorders)
                {
                    if (x.MessageChain.GetPlainMessage() == q)
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, (explanations[disorders.IndexOf(q)]));
                        }
                        catch { }
                    }
                }
                //量表测试链接推荐
                var scales = new List<string>
                {
                    "MBTI",
                    "艾森克",
                    "大五",
                    "九型",
                    "SCL90",
                    "SDS",
                    "SAS",
                    "MDQ",
                    "HCL32",
                    "偏执量表",
                    "强迫量表",
                    "精神质量表"
                };
                var links = new List<string>
                {
                    "(1)https://16personalities.com/ch/\r\n" +"(2)https://www.jungus.cn/zh-hans/test/\r\n" +"(3)https://www.zxgj.cn/g/mbti28",
                    "https://www.zxgj.cn/g/askj/",
                    "http://www.apesk.com/bigfive/",
                    "http://www.cnenn.cn/html/jiuxingrengeceshi/test.asp",
                    "http://www.ntneuro.org/scale/scl90.asp",
                    "https://www.zxgj.cn/g/yiyuzheng",
                    "https://www.zxgj.cn/g/jiaolv",
                    "https://www.zxgj.cn/g/mdq",
                    "https://www.zxgj.cn/g/hcl32",
                    "https://www.zxgj.cn/g/mpa",
                    "https://www.zxgj.cn/g/qiangpozheng",
                    "https://www.zxgj.cn/g/mpsy"
                };
                if (x.MessageChain.GetPlainMessage() == "量表" || x.MessageChain.GetPlainMessage() == "测试")
                {
                    await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "量表测试大全\r\n" +
                        "人格/性格测试：\r\n" +
                        "MBTI|艾森克\r\n" +
                        "大五|九型\r\n" +
                        "精神心理量表：\r\n" +
                        "SCL90（综合）|SDS（抑郁）\r\n" +
                        "SAS（焦虑）|MDQ（双相）\r\n" +
                        "HCL32（轻躁狂）|偏执量表\r\n" +
                        "强迫量表|精神质量表\r\n" +
                        "（精神心理量表结果仅供参考，如出现相关症状，还请尽快前往当地医院就诊）");
                }
                foreach (string q in scales)
                {
                    if (x.MessageChain.GetPlainMessage().ToUpper() == q)
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, (links[scales.IndexOf(q)]));
                        }
                        catch { }
                    }
                }
                //处理“你就是歌姬吧”（祖安）
                if (x.MessageChain.Count > 2)
                {
                    if (x.MessageChain[1].ToString() == "AtMessage { Type = At, Target = 2810482259 }" && x.MessageChain[2].ToString() == "PlainMessage { Type = Plain, Text =  你就是歌姬吧 }")
                    {
                        string[] words =
                        {
                         "cnmd",
                         "你更是歌姬吧嗷",
                         "你个狗比玩意",
                         "你是不是被抛上去3次，却只被接住2次？",
                         "你真是小母牛坐灯泡，牛逼一闪又一闪",
                         "小嘴像抹了开塞露一样",
                         "小东西长得真随机",
                         "我只想骂人，但不想骂你",
                         "但凡你有点用，也不至于一点用处都没有",
                         "你还真把自己当个人看了，你也配啊",
                         "那么丑的脸，就可以看出你是金针菇",
                         "阁下长得真是天生励志",
                         "装逼对你来说就像一日三餐的事",
                         "我怎么敢碰你呢，我怕我买洗手液买穷自己",
                         "狗咬了你，你还能咬回狗吗",
                         "你是独一无二的，至少全人类都不希望再有第二个",
                         "你的智商和喜马拉雅山的氧气一样，稀薄",
                         "别人的脸是七分天注定，三分靠打扮，你的脸是一分天注定，九分靠滤镜",
                         "偶尔也要活得强硬一点，软得像滩烂泥一样有什么意思",
                         "任何人工智能都敌不过阁下这款天然呆",
                         "我骂你是为了你好，你应该从中学到些什么，比如说自知之明",
                         "你要好好做自己，反正别的你也做不好",
                         "如果国家把长相分等级的话，你的长相，都可以吃低保了",
                         "你没权利看不惯我的生活方式，但你有权抠瞎自己的双眼",
                         "如果你觉得我哪里不对，请一定要告诉我，反正我也不会改，你别憋出病来",
                         "你（  ）什么时候（  ）啊"
                        };
                        Random r = new Random();
                        int index = r.Next(words.Length);
                        var messageChain = new MessageChainBuilder()
                        .At(x.Sender.Id)
                        .Plain(" ")
                        .Plain(words[index])
                        .Build();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, messageChain);
                        }
                        catch { }
                    }
                }
                //群管功能
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
                            admin_functions.Mute(x.Sender.Id, text[1], x.GroupId, text[2].ToInt32());
                        }
                        else if (text.Length == 2)
                        {
                            if (ja.Count == 4)
                            {
                                string target = ja[2]["target"].ToString();
                                string t = ja[3]["text"].ToString().Replace(" ", "");
                                int time = t.ToInt32();
                                admin_functions.Mute(x.Sender.Id, target, x.GroupId, time);
                            }
                            else if (ja.Count == 3)
                            {
                                string target = ja[2]["target"].ToString();
                                admin_functions.Mute(x.Sender.Id, target, x.GroupId, 10);
                            }
                            else
                            {
                                admin_functions.Mute(x.Sender.Id, text[1], x.GroupId, 10);
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
                            admin_functions.Unmute(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Unmute(x.Sender.Id, text[1], x.GroupId);
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
                            admin_functions.Kick(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Kick(x.Sender.Id, text[1], x.GroupId);
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
                            admin_functions.Block(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Block(x.Sender.Id, text[1], x.GroupId);
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
                            admin_functions.Unblock(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Unblock(x.Sender.Id, text[1], x.GroupId);
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
                            admin_functions.Op(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Op(x.Sender.Id, text[1], x.GroupId);
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
                            admin_functions.Deop(x.Sender.Id, target, x.GroupId);
                        }
                        else
                        {
                            admin_functions.Deop(x.Sender.Id, text[1], x.GroupId);
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
                        "机器人版本：a0.4.14\r\n上次更新日期：2022/6/27\r\n更新内容：移除了代码里奇♂怪的东西");
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