namespace EventHub.Config;

public class RabbitMqConfig
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "demo-queue";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}