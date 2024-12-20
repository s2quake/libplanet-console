using System.Text;
using LibplanetConsole.Common;

var queryObject = new
{
    query = @"
    {
        nodeStatus {
            tip {
                index
                hash
                miner
            }
        }
    }",
};

var queryJson = System.Text.Json.JsonSerializer.Serialize(queryObject);

var url = "http://k8s-9codinli-validato-aeaeca2683-8e145acafdea8c00.elb.us-east-2.amazonaws.com/graphql";
var content = new StringContent(queryJson, Encoding.UTF8, "application/json");
using (var client = new HttpClient())
{
    var response = await client.PostAsync(url, content);
    var responseString = await response.Content.ReadAsStringAsync();

    var json = JsonUtility.ToColorizedString(responseString);
    Console.WriteLine(json);
}
