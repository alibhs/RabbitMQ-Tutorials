using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Bağlantıyı oluşturma

ConnectionFactory factory = new();
factory.Uri = new("amqps://ldkmrxnt:HWEIeEtqZRj8pd1r6sPSSQR_E8Ed8bNX@roedeer.rmq.cloudamqp.com/ldkmrxnt");

//Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P (Point to Point Tasarımı)
//string queueName = "example-p2p-queue";
//channel.QueueDeclare(queue:queueName,durable:false, exclusive:false,autoDelete:false);

//byte[] message = Encoding.UTF8.GetBytes("merhaba");
//channel.BasicPublish(exchange:string.Empty,routingKey:queueName,body:message);

#endregion

#region Publish/Subscribe Tasarımı
//string exchangeName = "example-pub-sub-exchange";

//channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
//byte[] message2 = Encoding.UTF8.GetBytes("merhaba");

//channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, body: message2);

#endregion

#region Work Queue Tasarımı
//string quequeName = "example-work-queue";

//channel.QueueDeclare(
//    quequeName, false, false, false);
//for (int i = 0; i < 100; i++)
//{
//    byte[] message3 = Encoding.UTF8.GetBytes("merhaba " + i);
//    channel.BasicPublish(exchange: string.Empty, routingKey: quequeName, body: message3);

//}
#endregion

#region Request/Response Tasarımı
string requestQueueName = "example-request-response-queue";
channel.QueueDeclare(requestQueueName, false, false, false);

string responseQueueName = channel.QueueDeclare().QueueName;

string correlationId = Guid.NewGuid().ToString();

#region Request Mesajını Oluşturma ve Gönderme

IBasicProperties properties = channel.CreateBasicProperties();
properties.CorrelationId = correlationId; //Gönderilece mesajın corelasyon idsini gönderir
properties.ReplyTo= responseQueueName; //gönderelecek request sonucu gelen responsun hangi kuyruğa gönderileceğini sağlar.

for (int i = 0; i < 100; i++)
{
    byte[] message = Encoding.UTF8.GetBytes("merhaba" + i);
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: requestQueueName,
        body: message,
        basicProperties: properties
        );
}

#endregion
#region Response Kuyruğu dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: responseQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, e) =>
{
    //bu kuyruğa gelen correlasyon id'si responsonse olanları dondurecegiz
    if (e.BasicProperties.CorrelationId == correlationId)
    {
        //....
        Console.WriteLine($"Response: {Encoding.UTF8.GetString(e.Body.Span)}");
    }

};

#endregion
#endregion