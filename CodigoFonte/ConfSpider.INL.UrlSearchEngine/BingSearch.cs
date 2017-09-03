using ConfSpider.TOLibrary;
using Masters.Core.Comunication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.INL.UrlSearchEngine
{
    public class BingSearch
    {
        public event EventHandler<UrlSearchEngineEventArgs> ReceivedMessage;
        public void OnReceivedMessage(string message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, new UrlSearchEngineEventArgs(message));
        }

        public MessageResponse<object> SearchURLs(List<ConferenciaTO> conferencias, int topUrls)
        {
            var _response = new MessageResponse<object>();

            try
            {
                foreach (var _conf in conferencias)
                {
                    var _edicao = _conf.Edicoes.LastOrDefault();

                    OnReceivedMessage($"Buscando urls para a conferência {_conf.IdConferencia}");

                    if (_edicao != null)
                    {
                        // This is the query - or you could get it from args.
                        string _query = _edicao.Query;

                        // Create a Bing container.
                        string _rootUri = "https://api.datamarket.azure.com/Bing/Search";
                        var _bingContainer = new Bing.BingSearchContainer(new Uri(_rootUri));

                        // Replace this value with your account key.
                        ////var accountKey = @"o3vloXlt0s2NwzdEJXVZDekcyCIZYnXan65GQB/jJYE"; //antigo
                        var accountKey = @"0JiqfxRG9R63zJQYZxYGaz9gg5p5ftuvFaZ6vmdWhg4";

                        // Configure bingContainer to use your credentials.
                        _bingContainer.Credentials = new System.Net.NetworkCredential(accountKey, accountKey);

                        var _webQuery = _bingContainer.Web(_query, null, null, null, null, null, null, null);
                        _webQuery = _webQuery.AddQueryOption("$top", topUrls);

                        var _results = _webQuery.Execute().ToList();
                        for (int _c = 0; _c < _results.Count; _c++)
                        {
                            _edicao.Urls.Add(new UrlTO
                            {
                                Edicao = new EdicaoTO
                                {
                                    Conferencia = new ConferenciaTO { IdConferencia = _conf.IdConferencia },
                                    Ano = _edicao.Ano
                                },
                                Ordem = _c + 1,
                                Url = _results[_c].Url
                            });
                        }
                    }
                }

                return _response;
            }
            catch (Exception ex)
            {
                _response.Exceptions.Add(new MessageResponseException(ex, ex.Message, ResponseKind.Error));
                return _response;
            }
        }
    }
}
