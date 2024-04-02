using TrainCloud.HttpClient;
using TrainCloud.Tests.Microservices.Core;
using TrainCloud.Microservices.Email;
using TrainCloud.Microservices.Email.Models;
using System.Net;

namespace TrainCloud.Tests.Microservices.Email;

[TestClass]
public class UnitTest1 : AbstractTest<Program>
{

    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjExNjUwNTk0LTc2Y2QtNGY3Mi05YThiLWM2MzQ1ZTgzMDZkYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJNYXVkZS5GbGFuZGVycyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJBZG1pbmlzdHJhdG9yIiwiRGF0YUN1c3RvZGlhbiJdLCJleHAiOjIwMjI5MzU4NDYsImlzcyI6Imh0dHBzOi8vdHJhaW5jbG91ZC5uZXQiLCJhdWQiOiJodHRwczovL3RyYWluY2xvdWQubmV0In0.xQWK7hAgNJPe9iLYwoCgd1mGiRY3n68-C39HLACej1k";

    [TestMethod]
    public async Task TestMethod1Async()
    {

        string pdfFilePath = "C:\\Users\\test\\Desktop\\test.pdf";
        byte[] bytes = File.ReadAllBytes(pdfFilePath);

        Dictionary<string, byte[]> testdict = new Dictionary<string, byte[]> { { pdfFilePath, bytes } };

        // Arrange
        PostSendEmailModel model = new()
        {
          To = [
            "jm.schenk@t-online.de", "mail@sebastian-hoyer.online"
          ],
          CC = [
            "nico@caratiola.net"
          ],
          Title = "Unit Test 1 Mail",
          Body = "Hello Unit Test 1 \n Greetings",
          Attachments = testdict,
          IsHTML = false
};

        var anonymousClient = GetClient();

        // Act
        var result = await anonymousClient.PostRequestAsync<PostSendEmailModel, object>("/Email/Send", model, httpStatus =>
        {
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpStatus);
        });

        // Assert
        //Assert.IsNotNull(result);
    }
}

