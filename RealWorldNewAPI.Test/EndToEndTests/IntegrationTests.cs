using FluentAssertions;
using Newtonsoft.Json;
using RealWorldNew.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldNewAPI.Test.EndToEndTests
{
    public class IntegrationTests : IntegrationTest
    {
        [Test]
        public async Task GetArticles_AfterRegisterWithoutAnyArticles_ReturnEmptyResponse()
        {
            //Arrange
            await Register();

            // Act
            var response = await TestClient.GetAsync("api/articles");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (response.Content.ReadAsAsync<MultiArticleResponse>().Result).Should().BeOfType(typeof(MultiArticleResponse));
        }

        [Test]
        public async Task AddArticles_AfterLoginAddSomeArticle_ReturnArticleUploadResponse()
        {
            //Arrange
            await Login();

            //Act
            var response = await TestClient.PostAsync("api/articles", new StringContent(
                JsonConvert.SerializeObject(new ArticleUploadPack()
                {
                    article = new ArticleUpload()
                    {
                        title = "some title",
                        description = "some description",
                        body = "text",
                        tagList = new List<string>()
                        {
                            "test1", "test2"
                        }
                    }
                }),
                Encoding.UTF8,
                "application/json")
                );

            var content = await response.Content.ReadAsAsync<ArticleUploadResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.article.Should().BeOfType(typeof(articleAUP));
        }

        [Test]
        public async Task GetArticles_AfterLoginGetGlobal_ReturnMultiArticleResponse()
        {
            //Arrange
            await Login();

            //Act
            var response = await TestClient.GetAsync("api/articles?limit=10&offset=0");
            var content = await response.Content.ReadAsAsync<MultiArticleResponse>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.articles.Should().BeOfType(typeof(List<articleAUP>));
            content.articlesCount.Should().BeGreaterThan(0);
        }

        //[Test]
        //public async Task 


    }
}
