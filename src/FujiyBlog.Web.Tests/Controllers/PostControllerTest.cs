using System.Collections.Generic;
using System.Linq;
using FujiyBlog.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using FujiyBlog.Core.Repositories;
using System.Web.Mvc;
using Moq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Tests.Controllers
{
    
    
    /// <summary>
    ///This is a test class for PostControllerTest and is intended
    ///to contain all PostControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PostControllerTest
    {
        private PostController postController;
        private IPostRepository postRepository;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
        
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            var postRepoMock = new Mock<IPostRepository>();
            postRepoMock.Setup(x => x.GetPost("slug_post")).Returns((string slug) => new Post());
            postController = new PostController(null, postRepoMock.Object, null);
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestMethod]
        public void Details_Post_Exists()
        {
            var actionResult = postController.Details("slug_post");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            var viewResult = (ViewResult)actionResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(Post));

            var model = (Post)viewResult.Model;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Details_Post_Not_Exists()
        {
            var actionResult = postController.Details("slug_post_nao_existe");
            Assert.IsInstanceOfType(actionResult, typeof(HttpNotFoundResult));
        }
    }
}
