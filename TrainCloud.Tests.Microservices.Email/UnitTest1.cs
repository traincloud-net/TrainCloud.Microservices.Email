using TrainCloud.HttpClient;
using TrainCloud.Tests.Microservices.Core;
using TrainCloud.Microservices.Email;
using TrainCloud.Microservices.Email.Models;
using System.Net;

namespace TrainCloud.Tests.Microservices.Email;

[TestClass]
public class UnitTest1 : AbstractTest<Program>
{
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

