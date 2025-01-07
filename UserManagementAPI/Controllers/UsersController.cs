using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System.Collections.Concurrent;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static ConcurrentDictionary<int, User> users = new ConcurrentDictionary<int, User>
        {
            [1] = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Department = "HR" },
            [2] = new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Department = "IT" }
        };

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Ok<List<User>> GetUsers()
        {
            return TypedResults.Ok(users.Values.ToList());
        }

        // GET: api/users/1
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Results<Ok<User>, NotFound> GetUser(int id)
        {
            if (users.TryGetValue(id, out var user))
            {
                return TypedResults.Ok(user);
            }
            return TypedResults.NotFound();
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

            user.Id = users.Keys.Max() + 1;
            if (users.TryAdd(user.Id, user))
            {
                return TypedResults.CreatedAtRoute(user, null, new
                {
                    action = nameof(GetUser),
                    id = user.Id
                });
            }
            return TypedResults.BadRequest();
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

            if (users.TryGetValue(id, out var user))
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.Department = updatedUser.Department;

                users[id] = user;
                return TypedResults.NoContent();
            }
            return TypedResults.NotFound();
        }

        // DELETE: api/users/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Results<NoContent, NotFound> DeleteUser(int id)
        {
            if (users.TryRemove(id, out _))
            {
                return TypedResults.NoContent();
            }
            return TypedResults.NotFound();
        }
    }
}
