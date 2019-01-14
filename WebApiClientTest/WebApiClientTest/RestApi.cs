using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientTest
{
    public static class RestApi
    {
        private static T ObterTokenAcesso<T>(string urlBase, string token)
        {
            //Url da api que irá fornecer o token de acesso válido
            string url = urlBase + "/api/acesso/token";


            try
            {
                //Obter token sistemicamente
                string body = string.Format("grant_type=application/x-www-form-urlencoded&Token={0}", token);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                using (Stream stream = request.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(body);

                    stream.Write(bytes, 0, bytes.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                string result;

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                var tokenAcesso = JsonConvert.DeserializeObject<T>(result);

                return tokenAcesso;
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao obter o token de acesso. " + Environment.NewLine + "Detalhes do erro: " + ex.Message);
            }


        }

        public static string RealizarRequisicao(string urlBase, string token, string rota, Dictionary<string, object> parametrosApi = null, string CorpoMensagem = null)
        {
            string url = urlBase + rota;
            string body = string.Empty;

            if (parametrosApi != null)
                MontarCorpoRequisicao(parametrosApi);
            else
                body = CorpoMensagem;

            string tokenAcesso = ObterTokenAcesso<TokenAcesso>(urlBase, token).access_token;

            if (!string.IsNullOrEmpty(tokenAcesso))
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + tokenAcesso);


                using (Stream stream = request.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(body);

                    stream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                return result;

            }
            else
            {
                throw new Exception("Não foi possível gerar o token de acesso.");
            }



        }

        private static string MontarCorpoRequisicao(IDictionary<string, object> parametrosApi)
        {
            string bodyRetorno = string.Empty;
            var bodyMontagem = new StringBuilder();


            foreach (var item in parametrosApi)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    bodyMontagem.Append(string.Format("{0}={1}&", item.Key, item.Value));
                }
                else //Quando o valor não for uma string, deve se tratar de um objeto que será serializado em JSON
                {
                    bodyMontagem.Append(string.Format("{0}={1}&", item.Key, JsonConvert.SerializeObject(item.Value)));
                }
            }

            //Retira o último '&' para a montagem do correta o urlencoded
            bodyRetorno = bodyMontagem.ToString().Remove(bodyMontagem.ToString().Length - 1);

            return bodyRetorno;
        }
    }

    public class TokenAcesso
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string token { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
    }
}

