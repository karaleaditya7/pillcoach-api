using DinkToPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OntrackDb.Model;
using RestSharp;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
#nullable enable
namespace OntrackDb.Helper
{
	public class UtilityFax : IUtilityFax
	{
		private IConfiguration _configuration;
		public UtilityFax(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<string?> SendFax(PdfModel model, string filePath)
		{

			var client = new RestClient(_configuration["FaxUrl"]);
			var UsernameFax = _configuration["UsernameFax"];
			var Password = _configuration["Password"];
			var request = new RestRequest();
			request.Method = Method.Post;
			request.AddHeader("ContentType", "multipart/form-data");
			request.AddParameter("Username", UsernameFax);
			request.AddParameter("Password", Password);
			request.AddParameter("Cookies", "false");
			request.AddParameter("ProductId", _configuration["ProductId"]);
			request.AddParameter("JobName", "for api");
			request.AddParameter("Header", "api");
			request.AddParameter("BillingCode", "Customer Code 1234");
			request.AddParameter("Numbers1", model.providerFax);
			request.AddParameter("Numbers2", "");
			request.AddParameter("CSID", "3035550101");
			request.AddParameter("ANI", "0000000000");
			request.AddParameter("StartDate", "1/1/2019");
			request.AddParameter("FaxQuality", "Fine");
			request.AddParameter("FeedbackEmail", "(your email here)");
			request.AddParameter("CallbackUrl", model.CallbackUrl);

			request.AddFile("Files0", @filePath);

			RestResponse response = await client.ExecutePostAsync(request);
			File.Delete(@filePath);
			return response.Content;
		}

		public async Task<string?> GetFaxDescriptionByIds(PdfModel model)
		{
            //if(string.IsNullOrEmpty(_configuration["FaxDescriptionUrl"]))
            //{
            //	var client = new RestClient(_configuration["FaxDescriptionUrl"]);
            //}

            var client = new RestClient(_configuration["FaxDescriptionUrl"]);
            var request = new RestRequest();
			request.Method = Method.Post;

			var UsernameFax = _configuration["UsernameFax"];
			var Password = _configuration["Password"];

			request.AlwaysMultipartFormData = true;
			request.AddParameter("Username", UsernameFax);
			request.AddParameter("Password", Password);
			request.AddParameter("Cookies", "false");
			request.AddParameter("ProductId", _configuration["ProductId"]);
			JObject jsonObject = new JObject();
			jsonObject.Add("Id", model.JobId);
			jsonObject.Add("Direction", "Inbound");
			string str1 = Convert.ToString(jsonObject);
			request.AddParameter("FaxIds1", str1);
			RestResponse response = await client.ExecuteAsync(request);
			return response.Content;
		}

	}
}

