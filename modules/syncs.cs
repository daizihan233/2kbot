using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;

namespace Net_2kBot.Modules
{
    public static class Syncs
    {
        // 从Hanbot同步黑名单
        public static async void Sync(MessageReceiverBase @base, string group, string executor)
        {
            if (@base is not GroupMessageReceiver receiver) return;
            if (Global.Ops != null && Global.Ops.Contains(executor))
            {
                RestClient client = new("http://101.42.94.97/blacklist");
                RestRequest request = new("look")
                {
                    Timeout = 10000
                };
                RestResponse response = await client.ExecuteAsync(request);
                JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                await File.WriteAllTextAsync("blocklist.txt", String.Empty);
                await using StreamWriter file = new("blocklist.txt", append: true);
                foreach (string? s in jo["data"]!)
                {
                    await file.WriteLineAsync(s);
                }
                file.Close();
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, "从Hanbot同步黑名单成功！");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
        // 将黑名单反向同步到Hanbot
        public static async void Rsync(MessageReceiverBase @base, string group, string executor)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if (Global.Ops != null && Global.Ops.Contains(executor))
                {
                    RestClient client = new("http://101.42.94.97/blacklist");
                    RestRequest request = new("look")
                    {
                        Timeout = 10000
                    };
                    RestResponse response = await client.ExecuteAsync(request);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                    var blocklist2 = new List<string> {""};
                    if (Global.Blocklist == null) return;
                    foreach (string t in Global.Blocklist)
                    {
                        if (jo["data"]!.Contains(t)) continue;
                        RestClient client1 = new("http://101.42.94.97/blacklist");
                        RestRequest request1 = new("up?uid=" + t + "&key=" + Global.ApiKey, Method.Post);
                        request.Timeout = 10000;
                        await client1.ExecuteAsync(request1);
                    }
                    foreach (string? s in jo["data"]!)
                    {
                        if (s != null)
                        {
                            blocklist2.Add(s);
                        }
                    }
                    blocklist2.Remove("");
                    var diff = new HashSet<string>(Global.Blocklist);
                    diff.SymmetricExceptWith(blocklist2);
                    string diff1 = String.Join(", ", diff);
                    string[] diff2 = diff1.Split(",");
                    foreach (string s in diff2)
                    {
                        RestClient client2 = new("http://101.42.94.97/blacklist");
                        RestRequest request2 = new("del?uid=" + s + "&key=" + Global.ApiKey, Method.Delete);
                        request.Timeout = 10000;
                        await client2.ExecuteAsync(request2);
                    }
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.GroupId, "将黑名单反向同步给Hanbot成功！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 合并黑名单+双向同步
        public static async void Merge(MessageReceiverBase @base, string group, string executor)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if (Global.Ops != null && Global.Ops.Contains(executor))
                {
                    RestClient client = new("http://101.42.94.97/blacklist");
                    RestRequest request = new("look")
                    {
                        Timeout = 10000
                    };
                    RestResponse response = await client.ExecuteAsync(request);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                    await using StreamWriter file = new("blocklist.txt", append: true);
                    var blocklist2 = new List<string> {""};
                    if (Global.Blocklist != null)
                    {
                        foreach (string? s in jo["data"]!)
                        {
                            if (s != null)
                            {
                                blocklist2.Add(s);
                            }
                        }
                        blocklist2.Remove("");
                        var diff = new HashSet<string>(Global.Blocklist);
                        diff.SymmetricExceptWith(blocklist2);
                        string diff1 = String.Join(", ", diff);
                        string[] diff2 = diff1.Split(",");
                        foreach (string s in diff2)
                        {
                            if (!jo["data"]!.Contains(s))
                            {
                                RestClient client1 = new("http://101.42.94.97/blacklist");
                                RestRequest request1 = new("up?uid=" + s + "&key=" + Global.ApiKey, Method.Post);
                                request.Timeout = 10000;
                                await client1.ExecuteAsync(request1);
                            }
                            else if (!Global.Blocklist.Contains(s))
                            {
                                await file.WriteLineAsync(s);
                            }
                        }
                        file.Close();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "合并黑名单并双向同步成功！");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }    
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
    }
}
