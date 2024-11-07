using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var client = new HttpClient();
string baseUrl = "http://webapi:5000";

const string QUEUE_NAME = "rpc_queue";

var factory = new ConnectionFactory { HostName = "rabbitmq" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: QUEUE_NAME, durable: false, exclusive: false,
    autoDelete: false, arguments: null);

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs ea) =>
{
    AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
    IChannel ch = cons.Channel;
    string response = string.Empty;

    byte[] body = ea.Body.ToArray();
    IReadOnlyBasicProperties props = ea.BasicProperties;
    var replyProps = new BasicProperties
    {
        CorrelationId = props.CorrelationId
    };

    try
    {
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine("Got Request = {0}", message);

        Operations ops = JsonSerializer.Deserialize<Operations>(message);
        switch (ops.Ops)
        {
            case "GET":
                response = SendGetRequest($"{baseUrl}/{ops.Index}", client).Result;
                break;
            case "UPDATE":
                response = SendPutRequest($"{baseUrl}/{ops.Index}", client).Result;
                break;
            case "DELETE":
                response = SendDeleteRequest($"{baseUrl}/{ops.Index}", client).Result;
                break;
        }
        
        Console.WriteLine("Sent Response = {0}", response);
    }
    catch (Exception e)
    {
        Console.WriteLine($" [.] {e.Message}");
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        await ch.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
            mandatory: true, basicProperties: replyProps, body: responseBytes);
        await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};

await channel.BasicConsumeAsync(QUEUE_NAME, false, consumer);

while (true)
{
    Console.ReadLine();
}

static async Task<string> SendGetRequest(string url, HttpClient client)
{
    try
    {
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return responseBody;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"GET Failed: {ex.Message}");
        return ex.ToString();
    }
}

static async Task<string> SendPutRequest(string url, HttpClient client)
{
    try
    {
        string jsonData = "{}";
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PutAsync(url, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return responseBody;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"PUT Failed: {ex.Message}");
        return ex.ToString();
    }
}

static async Task<string> SendDeleteRequest(string url, HttpClient client)
{
    try
    {
        HttpResponseMessage response = await client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return responseBody;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DEL Failed: {ex.Message}");
        return ex.ToString();
    }
}

public class Operations
{
    public string Ops { get; set; }
    public int Index { get; set; }
}
