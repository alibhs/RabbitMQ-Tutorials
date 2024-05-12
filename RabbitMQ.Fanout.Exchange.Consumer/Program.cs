using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;

//Bağlantıyı oluşturma

ConnectionFactory factory = new();
factory.Uri = new("amqps://ldkmrxnt:HWEIeEtqZRj8pd1r6sPSSQR_E8Ed8bNX@roedeer.rmq.cloudamqp.com/ldkmrxnt");

//Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout); //exchange oluşturduk

Console.Write("Kuyruk adını giriniz : ");
string _queueName = Console.ReadLine(); //kuyruk adı aldık

channel.QueueDeclare(queue: _queueName, exclusive: false); //kuyruk oluşturduk aldığımız adla

channel.QueueBind( //exchange ile kuyruk arası iletişim sağladık.
    queue: _queueName,
    exchange: "fanout-exchange-example",
    routingKey: string.Empty
    );

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: _queueName,
    true, consumer);

consumer.Received +=(sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();