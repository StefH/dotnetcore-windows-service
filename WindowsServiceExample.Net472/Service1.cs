using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceExample.Lib.Interfaces;
using Microsoft.Extensions.Logging;

namespace WindowsServiceExample.Net472
{
    public partial class Service1 : ServiceBase
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly ILogger<Service1> _logger;
        readonly IMessageHandlerService _service;

        public Service1(ILogger<Service1> logger, IMessageHandlerService service)
        {
            _logger = logger;
            _service = service;

            InitializeComponent();
        }

        internal Task StartServiceAsync(string[] args)
        {
            _cts.Token.Register(() => _logger.LogInformation("Cancel"));

            return _service.StartListeningAsync(_cts.Token);
        }

        protected override void OnStart(string[] args)
        {
            _logger.LogInformation("OnStart");
            StartServiceAsync(args);
        }

        protected override void OnStop()
        {
            _logger.LogInformation("OnStop");
            _cts.Cancel();
        }
    }
}
