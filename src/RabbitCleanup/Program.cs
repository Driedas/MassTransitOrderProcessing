using System.Net;
using System.Net.Http.Json;

var handler = new HttpClientHandler();
handler.Credentials = new NetworkCredential("guest", "guest");
var client = new HttpClient(handler);

client.BaseAddress = new Uri("http://localhost:15672");
var exchanges = await client.GetFromJsonAsync<Exchange[]>("/api/exchanges");
foreach (var exchange in exchanges!.Where(x => x.Name != "" && !x.Name.StartsWith("amq")))
{
    var result = await client.DeleteAsync($"/api/exchanges/%2f/{exchange.Name}");
}

var queues = await client.GetFromJsonAsync<Queue[]>("/api/queues");
foreach (var queue in queues!)
{
    var result = await client.DeleteAsync($"/api/queues/%2f/{queue.Name}");
}

public class Exchange
{
    public string Name { get; set; } = null!;
}

public class Queue
{
    public string Name { get; set; } = null!;
}
