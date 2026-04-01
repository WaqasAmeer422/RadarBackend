using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services.Handlers;
using VisualSoft.Surveillance.Radar.Services.Models;
//using static VisualSoft.Surveillance.Radar.Domain.Constants;

namespace VisualSoft.Surveillance.Radar.Services.HostedServices
{
    public interface IParserRequestRadarConsumerService : IHostedService, IDisposable
    {
        Task ConsumeMessage(ReadOnlyMemory<byte> message, MessageProperties properties);
    }
    public class ParserRequestRadarConsumerService : IParserRequestRadarConsumerService
    {
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly RabbitMQConfiguration _rabbitMQSetting;
        private Exchange _exchange;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBus _bus;
        private IDisposable consumer;


        public ParserRequestRadarConsumerService(IBus bus, IServiceConfiguration serviceConfiguration,
            IOptions<RabbitMQConfiguration> rabbitMQSetting, ILogger logger, IServiceProvider serviceProvider)
        {
            _rabbitMQSetting = rabbitMQSetting.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.Information("Starting Radar Consumer on Queue: {QueueName}", _rabbitMQSetting.HumanDetectionQueueName);
            _logger.Information(Constants.Informations.INFO_STARTING_CONSUMER_SERVICE, new
            {
                QueueName = _rabbitMQSetting?.ParserQueueName
            });
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30))) // Example: 30 seconds timeout
            {
                try
                {
                    // Step 1: Set up the connection 
                    _exchange = await _bus.Advanced.ExchangeDeclareAsync(_rabbitMQSetting.ExchangeName, _rabbitMQSetting.ExchangeType, true, false, cts.Token);
                    var queueName = await _bus.Advanced.QueueDeclareAsync(_rabbitMQSetting.ParserQueueName, cts.Token);
                    await _bus.Advanced.BindAsync(_exchange, queueName, _rabbitMQSetting.ParserRoutingKey, cancellationToken);

                    // Step 2: Start listening
                    _bus.Advanced.Consume(queueName, async (bytes, properties, arg3) => await ConsumeMessage(bytes, properties));
                }
                catch (OperationCanceledException ex) // Catches both TaskCanceledException and other OperationCanceledExceptions
                {
                    //old _logger.Error(ex, "Error starting Radar Service");
                    _logger.Error(ex, Constants.Errors.ERROR_CONSUMING_MESSAGE_RABBIT_MQ, new
                    {
                        QueueName = _rabbitMQSetting?.ParserQueueName,
                        Exception = ex
                    });
                    // Check if cts.IsCancellationRequested is true to confirm it was OUR timeout
                    if (cts.IsCancellationRequested)
                    {
                        _logger.Error(Constants.Errors.ERROR_RABBIT_MQ_TIMEOUT, new
                        {
                            QueueName = _rabbitMQSetting?.ParserQueueName,
                        });
                    }
                    else
                    {
                        _logger.Error(Constants.Errors.ERROR_RABBIT_MQ_EASYNET_TIMEOUT, new
                        {
                            QueueName = _rabbitMQSetting?.ParserQueueName,
                        });
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(Constants.Errors.ERROR_CONSUMING_MESSAGE_RABBIT_MQ, new
                    {
                        QueueName = _rabbitMQSetting?.ParserQueueName,
                        Exception = exception
                    });
                }
            }
 
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            consumer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            consumer?.Dispose();
        }
        public async Task ConsumeMessage(ReadOnlyMemory<byte> message, MessageProperties properties)
        {
            try
            {
                _logger.Information(Constants.Informations.INFO_CONSUMING_MESSAGE_RABBIT_MQ, new
                {
                    QueueName = _rabbitMQSetting?.ParserQueueName
                });
                //var provider = GetHeaderInfo(properties.Headers, Constants.MessageHeader.PROVIDER);
                // var deviceEvent = GetHeaderInfo(properties.Headers, Constants.MessageHeader.EVENT);
                // set the header for radar any time
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Step 3: Deserialize the Radar Data3
                    var data = await ExtensionMethods.DeserializeAsync<RadarEventModel>(message);
                    if (data == null)
                    {
                        return;
                    }
                    //check validate the header by looking the msg
                    string msg = Encoding.UTF8.GetString(message.ToArray());
                    string eventType = msg.Contains(Constants.MessageHeader.RADAR_MESSAGE_HEADER) ? Constants.DeviceEvent.HUMAN_RADAR : string.Empty;
                    if (string.IsNullOrEmpty(eventType))
                    {
                        _logger.Error(Constants.Errors.ERROR_RABBIT_MQ_HEADER_NOT_VALID, new
                        {
                            DeviceEvent = eventType
                        });
                        return;
                    }
                    // or validate by default
                    //string eventType = Constants.DeviceEvent.HUMAN_RADAR;
                    //if (properties.Headers.TryGetValue(Constants.MessageHeader.EVENT, out object? headerValue))
                    //{                   
                    //    if (headerValue is byte[] headerBytes)
                    //    {
                    //        eventType = Encoding.UTF8.GetString(headerBytes);
                    //    }
                    //}
                    var transactionHandeler = scope.ServiceProvider.GetRequiredService<ITransactionLogServiceHandler>();
                    var transactionService = transactionHandeler.GetService(eventType);
                    _logger.Information(Constants.Informations.INFO_STARTING_TRANSACTION_PROCESS, new
                    {
                        QueueName = _rabbitMQSetting?.ParserQueueName
                    });
                    // log the radar data into the DB                                  
                    var result = await transactionService.ProcessData(data);
                    // Check for Error (T1) first,
                    if (result.IsT1)
                    {
                        _logger.Error(Constants.Errors.ERROR_CONSUMING_MESSAGE_RABBIT_MQ, new
                        {
                            QueueName = _rabbitMQSetting?.ParserQueueName,
                            Errors = result.AsT1
                        });
                        return;
                    }
                    else
                    {
                        // Handle Success (T0)
                        var transactionResult = (RadarGeneratedData)result.AsT0;
                        //set this lator hardcoded
                        if (transactionResult != null)
                        {
                            _logger.Information("Radar Data Saved. ID: {LogId}", transactionResult.Transaction);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _logger.Error(Constants.Errors.ERROR_CONSUMING_MESSAGE_RABBIT_MQ, new
                {
                    QueueName = _rabbitMQSetting?.ParserQueueName,
                    Exception = exp,
                });
            }
        }
    }
}