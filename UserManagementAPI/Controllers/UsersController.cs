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
        public IActionResult GetUsers()
        {
            return Ok(users.Values.ToList());
        }

        // GET: api/users/1
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int id)
        {
            if (users.TryGetValue(id, out var user))
            {
                return Ok(user);
            }
            return NotFound();
        }

        // POST: api/users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            user.Id = users.Keys.Max() + 1;
            if (users.TryAdd(user.Id, user))
            {
                return CreatedAtRoute(new
                {
                    action = nameof(GetUser),
                    id = user.Id
                }, user);
            }

            return BadRequest();
        }

        // PUT: api/users/1
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                ModelState.AddModelError("Id", "The ID in the URL does not match the ID in the request body.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }    

            if (users.TryGetValue(id, out var user))
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.Department = updatedUser.Department;

                users[id] = user;
                return NoContent();
            }
            return NotFound();
        }

        // DELETE: api/users/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            if (users.TryRemove(id, out _))
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
