using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static List<User> users = new List<User>
        {
            new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Department = "HR" },
            new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Department = "IT" }
        };

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Ok<List<User>> GetUsers()
        {
            return TypedResults.Ok(users);
        }

        // GET: api/users/1
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Results<Ok<User>, NotFound> GetUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(user);
        }

        // POST: api/users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Results<CreatedAtRoute<User>, BadRequest> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return TypedResults.BadRequest();
            }

            user.Id = users.Max(u => u.Id) + 1;
            users.Add(user);
            return TypedResults.CreatedAtRoute(user, null, new
            {
                action = nameof(GetUser),
                id = user.Id
            });
        }

        // PUT: api/users/1
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Results<NoContent, BadRequest, NotFound> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid || id != updatedUser.Id)
            {
                return TypedResults.BadRequest();
            }

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return TypedResults.NotFound();
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;

            return TypedResults.NoContent();
        }

        // DELETE: api/users/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Results<NoContent, NotFound> DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return TypedResults.NotFound();
            }

            users.Remove(user);
            return TypedResults.NoContent();
        }
    }
}
