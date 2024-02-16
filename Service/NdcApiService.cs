using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class NdcApiService : INdcApiService
    {
        private  readonly HttpClient client;
        private readonly HttpClient clinetRx;
        IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbcontext;

        public NdcApiService(IHttpClientFactory clientFactory, IConfiguration configuration, ApplicationDbContext applicationDbcontext)
        {
                client = clientFactory.CreateClient("NdcServices");
                clinetRx = clientFactory.CreateClient("RxInfoServices");
                _configuration = configuration;
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<string> GetRelatedNDCs(string NDCNumber)
        {

            string fullGenericName = "";
            var url = string.Format(_configuration["Rx-nav-Uri"] + NDCNumber);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                
                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(stringResponse);
                JsonElement root = document.RootElement;
                JsonElement body = root.GetProperty("ndcStatus");
                
                var checkrxcui = body.TryGetProperty("rxcui", out JsonElement value);
                if(checkrxcui && value.ToString() != "")
                {
                    JsonElement rxcui = body.GetProperty("rxcui");
                    var allRxInfoUrl = string.Format(_configuration["Rx-nav-Url"] + "/REST/RxTerms/rxcui/" + rxcui + "/allinfo.json");
                    var allRxInfoResponse = await clinetRx.GetAsync(allRxInfoUrl);
                    var stringResponseForallRxInfo = await allRxInfoResponse.Content.ReadAsStringAsync();
                    JsonDocument document1 = JsonDocument.Parse(stringResponseForallRxInfo);

                    JsonElement root1 = document1.RootElement;
                    var checkValue = root1.TryGetProperty("rxtermsProperties", out JsonElement value1);

                    if (checkValue)
                    {
                        JsonElement body1 = root1.GetProperty("rxtermsProperties");
                        JsonElement fullGenericName1 = body1.GetProperty("fullGenericName");
                        fullGenericName = fullGenericName1.ToString();
                    }
                }
                
               
                

            }

            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            return fullGenericName;
        }



        public async Task<Pdc_Medication> ForPdcMedicationAddition(string NDCNumber)
        {
            var url = string.Format(_configuration["Rx-nav-Uri"] + NDCNumber);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {

                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(stringResponse);
                JsonElement root = document.RootElement;
                JsonElement body = root.GetProperty("ndcStatus");
                JsonElement rxcui = body.GetProperty("rxcui");

                var allRxInfoUrl = string.Format("/REST/RxTerms/rxcui/" + rxcui + "/allinfo.json");
                var allRxInfoResponse = await clinetRx.GetAsync(allRxInfoUrl);
                var stringResponseForallRxInfo = await allRxInfoResponse.Content.ReadAsStringAsync();
                JsonDocument document1 = JsonDocument.Parse(stringResponseForallRxInfo);
                JsonElement root1 = document1.RootElement;
                JsonElement body1 = root1.GetProperty("rxtermsProperties");
  

                Pdc_Medication pdc_Medication = new Pdc_Medication()
                {
                    value_set_subgroup = body1.GetProperty("fullName").ToString(),
                    code_type ="NDC",
                    code =NDCNumber,
                    strength =  body1.GetProperty("strength").ToString(),
                    route =body1.GetProperty("route").ToString(),
                    category =""
                     
              };
                 _applicationDbcontext.Add(pdc_Medication);
                await _applicationDbcontext.SaveChangesAsync();

                return pdc_Medication;

            }

            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            
        }
    }
}
