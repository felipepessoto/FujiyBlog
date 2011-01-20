using FujiyBlog.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using FujiyBlog.Core.Repositories;
using System.Web.Mvc;

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
            //postRepository = 
            postController = new PostController(null);
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestMethod()]
        public void DetailsTest()
        {
            IPostRepository postRepository = null; // TODO: Initialize to an appropriate value
            PostController target = new PostController(postRepository); // TODO: Initialize to an appropriate value
            string postSlug = string.Empty; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Details(postSlug);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
