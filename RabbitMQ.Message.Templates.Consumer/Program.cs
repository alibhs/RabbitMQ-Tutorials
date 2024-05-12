using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

//Bağlantıyı oluşturma

ConnectionFactory factory = new();
factory.Uri = new("amqps://ldkmrxnt:HWEIeEtqZRj8pd1r6sPSSQR_E8Ed8bNX@roedeer.rmq.cloudamqp.com/ldkmrxnt");

//Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P (Point to Point Tasarımı)
//string queueName = "example-p2p-queue";
//channel.QueueDeclare(
//    queue: queueName, durable: false, exclusive: false, autoDelete: false);

//EventingBasicConsumer consumer = new(channel);
//channel.BasicConsume(
//    queue: queueName,
//    autoAck: false,
//    consumer);

//consumer.Received  +=(sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};


#endregion

#region Publish/Subscribe Tasarımı
//string exchangeName = "example-pub-sub-exchange";

//channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

//string quequeName = channel.QueueDeclare().QueueName;

//channel.QueueBind(quequeName, exchangeName, routingKey: string.Empty);

//EventingBasicConsumer consumer2 = new(channel);

//channel.BasicConsume(quequeName, false, consumer2);

//channel.BasicQos(
//    prefetchCount: 1,
//    prefetchSize: 0,
//    global: false);

//consumer2.Received +=(sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};



#endregion

#region Work Queue Tasarımı
//string quequeName2 = "example-work-queue";

//channel.QueueDeclare(
//    quequeName, false, false, false);

//EventingBasicConsumer consumer1 = new(channel);

//channel.BasicConsume(quequeName2, true, consumer1);

//channel.BasicQos(
//    prefetchCount: 1,
//    prefetchSize: 0,
//    false);

//consumer1.Received  +=(sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion

#region Request/Response Tasarımı
string requestQueueName = "example-request-response-queue";
channel.QueueDeclare(requestQueueName, false, false, false);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: requestQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
    //....
    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem tamamlandı. : {message}");
    IBasicProperties properties = channel.CreateBasicProperties();
    properties.CorrelationId = e.BasicProperties.CorrelationId;
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: e.BasicProperties.ReplyTo,//request response modelindeki response kuyruk adını temsil eder
        basicProperties: properties,
        body: responseMessage
 );
};
#endregion