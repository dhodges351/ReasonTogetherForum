using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Services;

namespace ReasonTogetherForum.Tests
{
    [TestFixture]
    public class Post_Service_Should
    {
        [OneTimeSetUp] public void Set_Up_Test_Fixture() 
        { 
            //For example starting browser at beginning of tests.
        }

        [OneTimeTearDown] public void TearDown() 
        {
            //For example closing browser at end of tests.
        }

        [SetUp]
        public void Setup_Before_Every_Test() 
        {
            //For example automation browser testing.
            //LoginAsTestUser();
        }

        [TearDown]
        public void TearDown_After_Every_Test()
        {
            //CleanUpTestArtifacts();
        }

        [TestCase("coffee", 3)]
        [TestCase("teA", 1)]
        [TestCase("water", 0)]
        public void Return_Filtered_Results_Corresponding_To_Query(string query, int expected)
        {
            var postCount = 0;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            //Arrange
            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Forums.Add(new Forum 
                { 
                    Id = 19, 
                    Description= "Coffee and Tea Forum",
                    ImageUrl = "",
                    Title = "Coffee And Tea",
                    Created = DateTime.Now
                });

                ctx.SaveChanges();

                var forum = ctx.Forums.Find(19);
                var user = new ApplicationUser
                {
                    FirstName = "Debra",
                    LastName = "Hodges",
                    Rating = 35,
                    ProfileImageUrl = "",
                    MemberSince = DateTime.Now.AddDays(-1),
                    IsActive = true
                };             

                ctx.Posts.Add(new Post
                {
                    Forum = forum,
                    Id = -2144,
                    Title = "First Post",
                    Content = "Coffee",
                    UserId = "9f64ef5e-966a-448a-8819-66758dc3a744",
                    Created = DateTime.Now,
                    User = user
                });

                ctx.Posts.Add(new Post
                {
                    Forum = forum,
                    Id = 23523,
                    Title = "coffee",
                    Content = "Some Content",
                    UserId = "9f64ef5e-966a-448a-8819-66758dc3a744",
                    Created = DateTime.Now,
                    User = user
                });

                ctx.Posts.Add(new Post
                {
                    Forum = forum,
                    Id = 223,
                    Title = "Tea",
                    Content = "Coffee",
                    UserId = "9f64ef5e-966a-448a-8819-66758dc3a744",
                    Created = DateTime.Now,
                    User = user
                });

                ctx.SaveChanges();
            }

            //Act
            using (var ctx = new ApplicationDbContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetFilteredPosts(query);
                postCount = result.Count();
            }

            //Assert
            Assert.AreEqual(expected, postCount);
            //Assert.AreEqual(4, postCount);
            //Assert.AreEqual(0, postCount);
            //Assert.AreEqual(1, postCount);
        }
    }
}