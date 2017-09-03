using ConfSpider.BLL.DateExtraction;
using ConfSpider.TOLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfSpider.UIConsole.Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            //TextExtraction();
            PositionalExtraction();
            //DoMerge();
        }

        private static void TextExtraction()
        {
            var _dateExtractor = new TextDateExtractor(EnumTipoExtracao.AnaliseTexto_URLsGoldstandard);

            _dateExtractor.ReceivedMessage += _dateExtractor_ReceivedMessage;

            var _resp = _dateExtractor.TextExtraction();

            if (_resp.HasErrors())
            {
                Console.WriteLine(_resp.Exceptions.First().Description);
            }
            Console.WriteLine("Extração de datas concluída\nTecle ENTER para sair.");
            
            Console.ReadLine();
        }

        private static void PositionalExtraction()
        {
            try
            {
                var _extractor = new PositionalDateExtractor(EnumTipoExtracao.Posicional_URLsGoldstandard, EnumTipoExtracao.Manual, true);

                _extractor.ReceivedMessage += _dateExtractor_ReceivedMessage;

                //var _resp = _extractor.ExtractInformationPosition();
                //if (_resp.HasErrors())
                //{
                //    Console.WriteLine(_resp.Exceptions.First().Description);
                //}
                //Console.WriteLine("Extração de posições concluída.\n\n\n");

                var _resp2 = _extractor.ExtractDates();
                if (_resp2.HasErrors())
                {
                    Console.WriteLine(_resp2.Exceptions.First().Description);
                }
                Console.WriteLine("Extração de datas concluída.\n\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }

            Console.WriteLine("Tecle ENTER para sair.");
            Console.ReadLine();
        }

        private static void DoMerge()
        {
            try
            {
                var _merger = new MergeExtractions(2016, EnumTipoExtracao.Posicional_URLsGoldstandard, EnumTipoExtracao.AnaliseTexto_URLsGoldstandard);

                _merger.ReceivedMessage += _dateExtractor_ReceivedMessage;
                
                var _resp = _merger.ExecuteMerge();
                if (_resp.HasErrors())
                {
                    Console.WriteLine(_resp.Exceptions.First().Description);
                }
                Console.WriteLine("Extração de datas concluída.\n\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }

            Console.WriteLine("Tecle ENTER para sair.");
            Console.ReadLine();
        }
        
        private static void _dateExtractor_ReceivedMessage(object sender, DateExtractorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
