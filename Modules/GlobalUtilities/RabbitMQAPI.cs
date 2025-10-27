using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class RabbitMQAPI
    {
        private static ConnectionFactory _Factory;
        private static ConnectionFactory Factory
        {
            get
            {
                try
                {
                    if (_Factory == null)
                    {
                        _Factory = new ConnectionFactory()
                        {
                            HostName = "localhost",
                            VirtualHost = "/",
                            Port = 5672,
                            UserName = "guest",
                            Password = "guest"
                        };
                    }
                }
                catch { }

                return _Factory;
            }
        }

        private static IConnection new_connection()
        {
            try
            {
                return Factory?.CreateConnection();
            }
            catch { return null; }
        }

        public static bool initialize()
        {
            try
            {
                using (IConnection conn = new_connection())
                using (IModel channel = conn?.CreateModel())
                {
                    if (channel == null)
                    {
                        conn.Close();
                        return false;
                    }

                    //configure the task distributor so that it doesn't give a task to a worker that is still on its previosu task
                    //so, each new task will be transfered to an idle worker
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false);

                    channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false);
                    channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false);

                    channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: string.Empty);

                    channel.QueueBind(queue: "my.queue2", exchange: "ex.fanout", routingKey: string.Empty, 
                        arguments: new Dictionary<string, object>() {
                            { "x-match", "all" },
                            { "status", "done" }
                        });

                    channel.Close();
                    conn.Close();
                }

                return true;
            }
            catch { return false; }
        }

        public static bool erase()
        {
            try
            {
                using (IConnection conn = new_connection())
                using (IModel channel = conn?.CreateModel())
                {
                    if (channel == null)
                    {
                        conn.Close();
                        return false;
                    }

                    channel.QueueDelete(queue: "my.queue1");
                    channel.QueueDelete(queue: "my.queue2");

                    channel.ExchangeDelete(exchange: "ex.fanout");

                    channel.Close();
                    conn.Close();
                }

                return true;
            }
            catch { return false; }
        }

        public static void send_message()
        {
            try
            {
                using (IConnection conn = new_connection())
                using (IModel channel = conn?.CreateModel())
                {
                    if (channel == null)
                    {
                        conn.Close();
                        return;
                    }

                    channel.BasicPublish(exchange: "ex.fanout", routingKey: string.Empty, body: Encoding.UTF8.GetBytes("Gesi Chaghochi"));
                    channel.BasicPublish(exchange: "ex.fanout", routingKey: string.Empty, body: Encoding.UTF8.GetBytes("Jesi Chaghochi"));

                    channel.Close();
                    conn.Close();
                }
            }
            catch { }
        }

        public static void create_consumer()
        {
            try
            {
                IConnection conn = new_connection();
                IModel channel = conn?.CreateModel();

                if (channel == null)
                {
                    conn.Close();
                    return;
                }

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    string message = Encoding.UTF8.GetString(e.Body.ToArray());
                    
                    bool processingResult = true;

                    if (processingResult)
                        channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                    else
                        channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
                };

                string consumerTag = channel.BasicConsume(queue: "my.queue1", autoAck: false, consumer: consumer);

                //channel.BasicCancel(consumerTag); //Unsubscribe
            }
            catch { }
        }
    }
}
