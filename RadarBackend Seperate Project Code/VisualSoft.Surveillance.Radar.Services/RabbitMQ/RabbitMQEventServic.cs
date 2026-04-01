using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using Serilog;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;

namespace VisualSoft.Surveillance.Radar.Services.RabbitMQ
{
    public interface IRabbitMQEventService
    {
        Task RaiseAlaramEvent(string alaramRuleName, Guid deviceId, string settingName, byte[] message);
    }
    public class RabbitMQEventService : IRabbitMQEventService
    {
        private readonly RabbitMQConfiguration _rabbitMQSetting;
        private readonly ILogger _logger;
        private Exchange _exchange;
        private IBus _bus;

        public RabbitMQEventService(IBus bus, IOptions<RabbitMQConfiguration> rabbitMQSetting, ILogger logger)
        {
            _rabbitMQSetting = rabbitMQSetting.Value;
            _logger = logger;
            _bus = bus;
        }

        public async Task RaiseAlaramEvent(string alaramRuleName, Guid deviceId, string settingName, byte[] message)
        {
            MessageProperties messageProperties = new MessageProperties();
            messageProperties.Headers.Add(new KeyValuePair<string, object>(Constants.MessageHeader.ALARAM_RULE_NAME, alaramRuleName));
            messageProperties.Headers.Add(new KeyValuePair<string, object>(Constants.MessageHeader.DEVICE_ID, deviceId.ToString()));
            messageProperties.Headers.Add(new KeyValuePair<string, object>(Constants.MessageHeader.DEVICE_SETTING_NAME, settingName));

            _logger?.Debug(Constants.Informations.INFO_SENDINDG_PAYLOAD_RABBIT_MQ, new
            {
                QueueName = _rabbitMQSetting.AlaramEventQueueName

            });

            await RaiseEventAsync(messageProperties, message, _rabbitMQSetting.AlaramEventQueueName, _rabbitMQSetting.AlaramEventRoutingKey);
        }

        private async Task RaiseEventAsync(MessageProperties properties, byte[] message, string queueName, string routingKey)
        {
            try
            {
                _exchange = await _bus.Advanced.ExchangeDeclareAsync(_rabbitMQSetting.ExchangeName, _rabbitMQSetting.ExchangeType, true, false);
                var realTimeEventQueue = await _bus.Advanced.QueueDeclareAsync(queueName);
                await _bus.Advanced.BindAsync(_exchange, realTimeEventQueue, routingKey);

                await _bus.Advanced.PublishAsync(_exchange, routingKey, true, properties, message);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, Constants.Errors.ERROR_EXCEPTION_OCCURED);
            }
        }
    }
}