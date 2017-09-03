using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.INL.UrlSearchEngine
{
    public class GoogleSearch
    {
        //API Key
        private static string API_KEY = "AIzaSyAKAvWebFiqsvpFv-bpemSvZ3hi1NuLftA";

        //The custom search engine identifier
        private static string CX = "005667485335738942617:jh7zdhayeq0";

        private CustomsearchService m_service;

        public GoogleSearch()
        {
            m_service = new CustomsearchService(new BaseClientService.Initializer
                                                {
                                                    ApiKey = API_KEY,
                                                });
        }

        public List<string> CustomSearch(string query, int top)
        {
            CseResource.ListRequest _listRequest = m_service.Cse.List(query);
            _listRequest.Cx = CX;
            _listRequest.Num = top;
            _listRequest.Start = 1;
            
            Search _search = _listRequest.Execute();

            var _response = new List<string>();

            if (_search.Items != null)
            {
                _response = _search.Items.Select(s => s.Link).ToList();
            }
            return _response;
        }
    }
}
