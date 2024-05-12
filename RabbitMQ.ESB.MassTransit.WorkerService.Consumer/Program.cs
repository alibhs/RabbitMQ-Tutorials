
using MassTransit;
using RabbitMQ.ESB.MassTransit.WorkerService.Consumer.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<ExampleMessageConsumer>(); //Consumer eklenir

            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("amqps://befjdvjy:bs5zD-4j8OfHQrZFUOnEAKomCudYmkL1@moose.rmq.cloudamqp.com/befjdvjy");

                _configurator.ReceiveEndpoint("example-message-queue", e => e.ConfigureConsumer<ExampleMessageConsumer>(context)); //Hangi kuyruða hangi consumer eriþecek belirtilir QueueBind gibi
            });
        });
    })
    .Build();

await host.RunAsync();
