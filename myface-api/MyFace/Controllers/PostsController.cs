using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Models.Database;
using MyFace.Repositories;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;


namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsRepo _posts;

        public PostsController(IPostsRepo posts)
        {
            _posts = posts;
        }

        private readonly IAuthRepo _auth;

        public PostsController(IAuthRepo auth)
        {
            _auth = auth;
        }

        [HttpGet("")]
        public ActionResult<PostListResponse> Search([FromQuery] PostSearchRequest searchRequest)
        {
            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Authorization
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            var user = _auth.Authorize(authHeader);

            if (user == true)
            {
                var post = _posts.Create(newPost);
                var url = Url.Action("GetById", new { id = post.Id });
                var postResponse = new PostResponse(post);
                return Created(url, postResponse);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            var user = _auth.Authorize(authHeader);

            if (user == true)
            {
                var post = _posts.Update(id, update);
                return new PostResponse(post);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            // Authorization
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            var user = _auth.Authorize(authHeader);
            if (user == true)
            {
                _posts.Delete(id);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}