using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Service
{
    public class SmsSender
    {
        private readonly TwoFactorOptions _options;
        private readonly TwoFactorService _service;
        private readonly IConfiguration _configuration;

        public SmsSender(IOptions<TwoFactorOptions> options, TwoFactorService service, IConfiguration configuration)
        {
            _options = options.Value;
            _service = service;
            _configuration = configuration;
        }

        public string Send(string phone)
        {
            string code = _service.GetCodeVerification().ToString();
            /*
             * SMS Provider kodlaması yapılacak (Ara kodlar)
            */
            return code;
        }
    }
}
