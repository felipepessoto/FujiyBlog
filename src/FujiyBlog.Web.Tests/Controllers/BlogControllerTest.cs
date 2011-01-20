using System.Linq;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using FujiyBlog.Core.Repositories;
using System.Web.Mvc;
using Moq;
using System.Collections.Generic;

namespace FujiyBlog.Web.Tests.Controllers
{
    
    
    /// <summary>
    ///This is a test class for BlogControllerTest and is intended
    ///to contain all BlogControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BlogControllerTest
    {
        private BlogController blogController;
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
            postRepoMock.Setup(x => x.GetRecentPosts(It.IsAny<int>(), It.IsAny<int>())).Returns(
                (int skip, int take) =>
                Enumerable.Range(skip, take).Select(x => new Post { Title = x + " - Title", Content = new string('A', x) }));

            blogController = new BlogController(postRepoMock.Object);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestMethod()]
        public void IndexTest()
        {
            var model = ((ViewResult)blogController.Index(0)).Model;
            Assert.IsInstanceOfType(model, typeof(IEnumerable<Post>));
        }
    }
}
