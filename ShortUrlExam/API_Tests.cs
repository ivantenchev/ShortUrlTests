using NUnit.Framework;
using RestSharp;
using ShortUrl.ApiTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace ShortUrlExam
{
    public class API_Tests
    {
        private const string myurl = "https://shorturl.nakov.repl.co/api/urls";         

        private RestClient client;
        private RestRequest request;

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient();
        }

        [Test]
        public void Test_ListUrls_CheckFirst()
        {
            this.request = new RestRequest(myurl);

            var response = this.client.Execute(request);
            var allUrls = JsonSerializer.Deserialize<List<ShortCodeUrl>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(allUrls, Is.Not.Empty);

            Assert.That(allUrls[0].url, Is.EqualTo("https://nakov.com"));
            Assert.That(allUrls[0].shortCode, Is.EqualTo("nak"));
        }

        [Test]
        public void Test_SearchUrlsByShort_CheckFirst_ValidData()
        {
            this.request = new RestRequest(myurl + "/seldev");

            var response = this.client.Execute(request);
            var allUrls = JsonSerializer.Deserialize<ShortCodeUrl>(response.Content);

            var expectedUrl = allUrls.url;
            var expectedShortUrl = allUrls.shortUrl;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(expectedUrl, Is.EqualTo("https://selenium.dev"));
            Assert.That(expectedShortUrl, Is.EqualTo("http://shorturl.nakov.repl.co/go/seldev"));
        }

        [Test]
        public void Test_SearchUrlsByShort_CheckFirst_INValidData()
        {
            this.request = new RestRequest(myurl + "/chikiriki");

            var response = this.client.Execute(request);
            var allUrls = JsonSerializer.Deserialize<ShortCodeUrl>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Short code not found: chikiriki\"}"));
        }

        [Test]
        public void Test_CreateNewShort_InvalidData()
        {
            this.request = new RestRequest(myurl);
            var body = new
            {
                url = "cnn.com",
                shortCode = "cnn",
            };
            request.AddJsonBody(body);

            var response = this.client.Execute(request, Method.Post);
            var allUrls = JsonSerializer.Deserialize<ShortCodeUrl>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Invalid URL!\"}"));
        }

        [Test]
        public void Test_CreateNewShort_ValidData()
        {
            this.request = new RestRequest(myurl);

            var body = new
            {
                url = "https://ala.com" + DateTime.Now.Ticks,
                shortCode = "ala" + DateTime.Now.Ticks,
            };

            request.AddJsonBody(body);

            var response = this.client.Execute(request, Method.Post);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUrls = this.client.Execute(request, Method.Get);

            var allUrls = JsonSerializer.Deserialize<List<ShortCodeUrl>>(updatedUrls.Content);

            var lastUrl = allUrls.Last();


            Assert.That(lastUrl.url, Is.EqualTo(body.url));
            Assert.That(lastUrl.shortCode, Is.EqualTo(body.shortCode));
        }

        [Test]
        public void Test_DeleteUrl_ValidData()
        {
            this.request = new RestRequest(myurl + "/{short}");
            request.AddUrlSegment("short", "ala637917509762805779");

            var response = this.client.Execute(request, Method.Delete);

            Assert.That(response.Content, Is.EqualTo("{\"msg\":\"URL deleted: ala637917509762805779\"}"));
        }

        [Test]
        public void Test_DeleteUrl_InValidData()
        {
            this.request = new RestRequest(myurl + "/{short}");
            request.AddUrlSegment("short", "alabala");

            var response = this.client.Execute(request, Method.Delete);

            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Short code not found: alabala\"}"));
        }

        [Test]
        public void Test_VisitUrl_IncreaseVisits_ValidData()
        {
            this.request = new RestRequest(myurl + "/{short}");
            request.AddUrlSegment("short", "node");

            var response1 = this.client.Execute(request);

            var websiteProp = JsonSerializer.Deserialize<ShortCodeUrl>(response1.Content);
            var numOfVisits = websiteProp.visits;

            this.request = new RestRequest(myurl + "/visit/{short}");
            request.AddUrlSegment("short", "node");

            var response = this.client.Execute(request, Method.Post);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updated = JsonSerializer.Deserialize<ShortCodeUrl>(response.Content);

            var expectedVisits = updated.visits;

            Assert.That(expectedVisits, Is.GreaterThan(numOfVisits));

        }

        [Test]
        public void Test_VisitUrl_IncreaseVisits_InValidData()
        {

            this.request = new RestRequest(myurl + "/visit/{short}");
            request.AddUrlSegment("short", "random1234");

            var response = this.client.Execute(request, Method.Post);
            var updated = JsonSerializer.Deserialize<ShortCodeUrl>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Cannot navigate to given short URL\",\"errDetails\":\"Invalid short URL code: random1234\"}"));

        }
    }
}