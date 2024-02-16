using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class SafetyDisposalService:ISafetyDisposalService
    {
        private IConfiguration _configuration;
        private readonly ISafetyDisposalData _safetyDisposalData;
        private readonly HttpClient client;
        private readonly HttpClient clientZip;
        

        public SafetyDisposalService(IHttpClientFactory clientFactory,IConfiguration configuration, ISafetyDisposalData safetyDisposalData)
        {
            _configuration = configuration;
            _safetyDisposalData = safetyDisposalData;
            client = clientFactory.CreateClient("SafetyDisposalService");
            clientZip = clientFactory.CreateClient("ZipTasticUrl");
            

        }

        public void InsertIntoDBForSafetyDisposalFromTextFile(IFormFile file)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (Stream stream = file.OpenReadStream())
            {

                DataTable dt = new DataTable();

                dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id", typeof(int)),
                                new DataColumn("Name", typeof(string)),
                                new DataColumn("ADDLCOINFO",typeof(string)),
                                 new DataColumn("ADDRESS1",typeof(string)),
                                 new DataColumn("ADDRESS2",typeof(string)),
                                 new DataColumn("CITY",typeof(string)),
                                 new DataColumn("STATE",typeof(string)),
                                 new DataColumn("ZIP",typeof(string)),
                                 new DataColumn("LATITUDE",typeof(string)),
                                 new DataColumn("LONGITUDE",typeof(string)),
                });

                StreamReader sr = new StreamReader(stream);
                string input;

                while ((input = sr.ReadLine()) != null)
                {
                    if (!input.StartsWith("NAME"))
                    {
                        string[] values = input.Split(new char[] { '|' });
                        DataRow dr = dt.NewRow();
                        dr["Name"] = values[0];
                        dr["ADDLCOINFO"] = values[1];
                        dr["ADDRESS1"] = values[2];
                        dr["ADDRESS2"] = values[3];
                        dr["CITY"] = values[4];
                        dr["STATE"] = values[5];
                        dr["ZIP"] = values[6];
                        dr["LATITUDE"] = values[7];
                        dr["LONGITUDE"] = values[8];
                        dt.Rows.Add(dr);
                    }

                }
                sr.Close();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(_configuration["ConnectionStrings:Connstr"],
                SqlBulkCopyOptions.TableLock);
                bulkCopy.DestinationTableName = "dbo.safetyDisposal";
                bulkCopy.WriteToServer(dt);

            }

        }


        public async Task<Response<SafetyDisposal>> GetSafetyDisposalByZipCode(string zipCode,string username)
        {
            List<SafetyDisposal> safetyDisposals =new List<SafetyDisposal>();
            List<SafetyDisposal> safetyDisposals1 =new List<SafetyDisposal>();
           
            Response<SafetyDisposal> response = new Response<SafetyDisposal>();
         
            var safetyDisposal = await _safetyDisposalData.GetSafetyDisposalsByZipCode(zipCode);
           
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
           
            if (safetyDisposal == null)
            {
                var url = string.Format(_configuration["SafetyDisposalNearByAddressUrl"]+zipCode+ _configuration["SafetyDisposalNearByAddressUri"]+username);
                var response1 = await client.GetAsync(url);
               
                if (response1.IsSuccessStatusCode)
                {

                    var stringResponse = await response1.Content.ReadAsStringAsync();
                    JsonDocument document = JsonDocument.Parse(stringResponse);
                    JsonElement root = document.RootElement;
                    var checkValue = root.TryGetProperty("postalCodes", out JsonElement value);
                    if(checkValue)
                    {
                        JsonElement body = root.GetProperty("postalCodes");
                        JsonElement lat = body[0].GetProperty("lat");
                        JsonElement longi = body[0].GetProperty("lng");
                   
                        safetyDisposals = await _safetyDisposalData.GetSafetyDisposalsByZipCodeForLatitudeAndLongitude(lat.ToString(), longi.ToString());
                        var myLocation = geometryFactory.CreatePoint(new Coordinate(Convert.ToDouble(lat.ToString()), Convert.ToDouble(longi.ToString())));
                        foreach (SafetyDisposal safety in safetyDisposals)
                        {
                            safety.Location = geometryFactory.CreatePoint(new Coordinate(Convert.ToDouble(safety.LATITUDE), Convert.ToDouble(safety.LONGITUDE)));
                            var distance = myLocation.Distance(safety.Location);
                            safety.Distance = distance * 100;
                            safetyDisposals1.Add(safety);
                        }

                        safetyDisposals1 = safetyDisposals1
                            .OrderBy(x => x.Distance)
                           .ToList();
                    }

                }
            }


            if (safetyDisposal != null)
            {
                safetyDisposal.Location = geometryFactory.CreatePoint(new Coordinate(Convert.ToDouble(safetyDisposal.LATITUDE), Convert.ToDouble(safetyDisposal.LONGITUDE)));

             
                safetyDisposals = await _safetyDisposalData.GetSafetyDisposalsByZipCodeForLatitudeAndLongitude(safetyDisposal.LATITUDE, safetyDisposal.LONGITUDE);
                foreach (SafetyDisposal safety in safetyDisposals)
                {
                    safety.Location = geometryFactory.CreatePoint(new Coordinate(Convert.ToDouble(safety.LATITUDE), Convert.ToDouble(safety.LONGITUDE)));

                    var distance = safetyDisposal.Location.Distance(safety.Location);
                    safety.Distance = distance * 100;
                    safetyDisposals1.Add(safety);
                }


                safetyDisposals1 = safetyDisposals1
                        .OrderBy(x => x.Distance)
                        .ToList();
  
            }

            if (safetyDisposals == null)
            {
                response.Success = false;
                response.Message = "safetyDisposals Not Found With That zipCode";
                return response;
            }

            response.Success = true;
            response.Message = "safetyDisposals retrived successfully";
            response.DataList = safetyDisposals1;
            return response;
        }


        public async Task<Response<SafetyDisposalModel>> GetNearByAddressByZipCode(string zipCode,string username)
        {
           List<SafetyDisposalModel> safetyDisposalModels = new List<SafetyDisposalModel>();
           List<string> zipcodes = new List<string>();
            Response<SafetyDisposalModel> result = new Response<SafetyDisposalModel>();
            JsonElement state;
            JsonElement city;

            var url = string.Format(_configuration["SafetyDisposalNearByAddressUrl"]+zipCode + _configuration["SafetyDisposalNearByAddressUri"]+username);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {

                var stringResponse = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(stringResponse);
                JsonElement root = document.RootElement;
                JsonElement body = root.GetProperty("postalCodes");
                for (int i=0;i< body.GetArrayLength();i++)
                {
                    SafetyDisposalModel safetyDisposalModel = new SafetyDisposalModel();
                    var checkValue = root.TryGetProperty("postalCodes", out JsonElement value);
                    

                    if (checkValue)
                    {
                       
                        JsonElement name = body[i].GetProperty("placeName");
                        JsonElement latitude = body[i].GetProperty("lat");
                        JsonElement longitude = body[i].GetProperty("lng");
                        JsonElement zipcode = body[i].GetProperty("postalCode");
                         state = body[i].GetProperty("adminName1");
                         city = body[i].GetProperty("adminName2");

                        var safetydisposal = await _safetyDisposalData.GetSafetyDisposalsByZipCode(zipcode.ToString());
                        
                        
                        
                        if(safetydisposal != null)
                        {
                            safetyDisposalModel.ADDRESS1 = safetydisposal.ADDRESS1;
                            safetyDisposalModel.ADDRESS2 = safetydisposal.ADDRESS2;
                            safetyDisposalModel.ADDLCOINFO = safetydisposal.ADDLCOINFO;
                        }

                            safetyDisposalModel.Name = name.ToString();
                            safetyDisposalModel.LATITUDE = latitude.ToString();
                            safetyDisposalModel.LONGITUDE = longitude.ToString();
                            safetyDisposalModel.ZIP = zipCode.ToString();
                            safetyDisposalModel.STATE = state.ToString();
                            safetyDisposalModel.CITY = city.ToString();

                    }
                    safetyDisposalModels.Add(safetyDisposalModel);
                }
               
               


            }
            result.Success = true;
            result.Message = "safety disposal retrived successfully!";
            result.DataList = safetyDisposalModels;
            return result;


        }
    }
}
