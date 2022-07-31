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
        public static async void Sync(MessageReceiverBase @base)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                RestClient client = new("http://101.42.94.97/blacklist");
                RestRequest request = new("look", Method.Get);
                request.Timeout = 10000;
                RestResponse response = await client.ExecuteAsync(request);
                JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                File.WriteAllText("blocklist.txt", String.Empty);
                using StreamWriter file = new("blocklist.txt", append: true);
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
        }
    }
}
