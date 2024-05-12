using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Bağlantıyı oluşturma

ConnectionFactory factory = new();
factory.Uri = new("amqps://ldkmrxnt:HWEIeEtqZRj8pd1r6sPSSQR_E8Ed8bNX@roedeer.rmq.cloudamqp.com/ldkmrxnt");

//Aktifleştirme ve Kanal Oluşturma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

//Queue Oluşturma
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);//Consumer'daki kuyruk, publisher ile birebir aynı olmalıdır.| Durable parapertisi için publishera bak.

//Queue mesaj okuma
EventingBasicConsumer consumer = new(channel);
//channel.BasicConsume(queue: "example-queue", false, consumer);
var consumerTag = channel.BasicConsume(queue: "example-queue", autoAck: false, consumer); //AutoAck:false ile mesaj onaylamayı aktifleştiririz
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: true); //prefetchsize: byte cinsinden gönderilcek mesaj boyutunu ayarlatır (0 sınırısız) | count: consumer aynı anda kaç mesaj işleyecek |
consumer.Received += (sender, e) =>
{
    //Kuyruğa gelen mesajın işlendiği yerdir
    //e.body kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    //e.body.Span veya e.body.ToArray(): Kuyruktaki mesajın byte verisini getirir.

    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    #region MessageAcknowledgment
    /*channel.BasicAck(deliveryTag:e.DeliveryTag, multiple: false);*/// e.DeliveryTag ile ilgili mesaj secilir ve multiple:false ile sadece bu mesaj için onay verir / true ise bu ve öncekiler için onay verir
    /*channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false,requeue:true);*/// requeue true ise kuyruğa tekrar ekler false ise siler
    /*channel.BasicCancel(consumerTag);*/ //tüm mesajların reddetmeye sağlar
    /*channel.BasicReject(e.DeliveryTag,true);*///tek bir mesajın işlenmesini reddetme
    #endregion


};

Console.Read();


