using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientTest
{
    class Program
    {
        private const string _urlBase = "http://localhost/MinhaAplicacao/api/";
        private const string _token = "123";
        private const string _rota = "MinhaController/Get";

        static void Main(string[] args)
        {
            RestApi.RealizarRequisicao(_urlBase, _token, _rota); 
        }

      
    }

   
}
