using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;

namespace SupportPlatform.BLL.Helpers
{
    public static class SignalRHubConnectionBuilder
    {
        public static HubConnection Build(string ip)
        {
            var hub = new HubConnectionBuilder()
                  .WithUrl(new Uri("https://" + ip + "/hub/interact"), options =>
                  {
                      var handler = new HttpClientHandler
                      {
                          ClientCertificateOptions = ClientCertificateOption.Manual,
                          ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                      };
                      options.HttpMessageHandlerFactory = _ => handler;
                      options.WebSocketConfiguration = sockets =>
                      {
                          sockets.RemoteCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;
                      };
                  })
            .Build();

            return hub;
        }
    }
}
