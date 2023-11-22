using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LetsSparql.Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetsSparql.Service
{
    public class SparqlExecuter: ISparqlExecuter
    {
        public async Task<SparqlResponse> GetResponse(string endPointUrl, string queryString, bool explain, bool update)
        {
            SparqlResponse resp = new SparqlResponse();
            Stopwatch stpWatch = new Stopwatch();
            try
            {                
                stpWatch.Start();
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(40));
                resp.Response = await Task.Run(() => GetHttpResponse(endPointUrl, queryString, explain, update), cancellationTokenSource.Token); 
                stpWatch.Stop();
                resp.TimeTaken = stpWatch.ElapsedMilliseconds;
                resp.dataTableFormat = ConvertResponseToDataTable(resp.Response);
                resp.TotalRecords = resp.dataTableFormat.Rows.Count;
            }
            catch (Exception ex)
            {
                stpWatch.Stop();
                resp.TimeTaken = stpWatch.ElapsedMilliseconds;
                resp.Error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return resp;
        }

        private DataTable ConvertResponseToDataTable(string response)
        {
            DataTable dataTable = new DataTable();
            try
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(response);
                foreach(JProperty node in jsonObj.Children())
                {
                    foreach (JObject ChildNode in node)
                    {
                        if (node.Name == "head")
                        {
                            ChildNode.TryGetValue("vars", out JToken headers);
                            if (headers != null)
                            {
                                foreach (string val in headers)
                                {
                                    dataTable.Columns.Add(val);                                    

                                }
                            }
                        }
                        else
                        {
                            ChildNode.TryGetValue("bindings", out JToken bindings);
                            if (bindings != null)
                            {
                                foreach (JToken row in bindings)
                                {
                                    DataRow curRow = dataTable.NewRow();
                                    foreach (JToken col in row)
                                    {
                                        foreach (JToken colVal in col)
                                        {
                                            curRow[((JProperty)col).Name] = colVal.SelectToken("value").ToString();
                                        }
                                    }

                                    dataTable.Rows.Add(curRow);
                                }
                            }

                        }
                    }
                }                
            }
            catch 
            {

            }
            return dataTable;
        }
        
        private async Task<string> GetHttpResponse(string endPointUrl, string queryString, bool explain, bool update)
        {            
            var clientHandler = new HttpClientHandler();
            clientHandler.UseProxy = true;
            clientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            clientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                
            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

                var postObj = new Dictionary<string, string>();
                if (update)
                {
                    postObj.Add("update", queryString);
                }
                else
                {
                    postObj.Add("query", queryString);
                    if (explain)
                        postObj.Add("explain", "static");
                }  
                var req = new HttpRequestMessage(HttpMethod.Post, endPointUrl) { Content = new FormUrlEncodedContent(postObj) };                                       
                var response = httpClient.SendAsync(req).Result;               
                var result = await response.Content.ReadAsStringAsync();                    
                return result;
            }                            
        }

    }
}
