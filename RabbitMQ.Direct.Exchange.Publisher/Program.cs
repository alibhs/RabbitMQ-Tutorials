using RabbitMQ.Client;
using System.Text;

//Bağlantıyı oluşturma
ConnectionFactory factory = new();
factory.Uri = new("amqps://befjdvjy:X6brcqMd4AMZmJKYHu6RshOAyBD08E0P@moose.rmq.cloudamqp.com/befjdvjy");

//Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

while (true)
{
    Console.Write("Mesaj : ");
    string message = Console.ReadLine();
    byte[] byteMessage = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "direct-exchange-example", 
        routingKey: "direct-queue-example",
        body: byteMessage);//hangi kuyruga göndereceğimizi routing key ile belirleriz
}

Console.Read();