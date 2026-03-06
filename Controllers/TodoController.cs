using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyApi.Dto;
using MyApi.Model;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]   // api/todo
    [ApiController]
    public class TodoController : ControllerBase
    {
        private static readonly List<Todo> TodoList = new()
        {
            new Todo { Id = 1, Title = "External Meeting", IsDone = false },
            new Todo { Id = 2, Title = "Internal Meeting", IsDone = false },
            new Todo { Id = 3, Title = "Learning API", IsDone = false }
        };

        //api/todo
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(TodoList);
        }

        // api/todo/1
        [HttpGet("{Id:int}")]
        public ActionResult GetById(int Id)
        {
            var TodoItem = TodoList.Where(x => x.Id == Id).FirstOrDefault();
            if (TodoItem == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูล"});
            }

            return Ok(TodoItem);
        }


        [HttpPost]
        public ActionResult Create([FromForm] CreateTodoRequest Request)
        {
            var NextId = (TodoList.Count == 0) ? 1 : TodoList.Max(X => X.Id) + 1;

            var NewTodo = new Todo
            {
                Id = NextId,
                Title = Request.Title,
                IsDone = Request.IsDone
            };

            TodoList.Add(NewTodo);
            return CreatedAtAction(nameof(GetById), new { id = NewTodo.Id}, NewTodo);
        }

        [HttpPut("{Id:int}")]
        public ActionResult Update(int Id, [FromBody] UpdateTodoRequest Request)
        {
            if (string.IsNullOrWhiteSpace(Request.Title))
            {
                return BadRequest(new { message = "กรุณากรอกข้อมูลให้ถูกต้อง"});
            }

            var TodoItem = TodoList.FirstOrDefault(x => x.Id == Id);
            if (TodoItem == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลที่ต้องการแก้ไข"});
            }

            TodoItem.Title = Request.Title;
            TodoItem.IsDone = Request.IsDone;

            return Ok(TodoItem);
        }

        [HttpDelete("{Id:int}")]
        public ActionResult Delete(int Id)
        {
            var TodoItem = TodoList.FirstOrDefault(x => x.Id == Id);
            if (TodoItem == null)
            {
                return NotFound(new { meesage = "ไม่พบข้อมูลที่ต้องการลบ"});
            }
            
            TodoList.Remove(TodoItem);
            return NoContent();
        }
    }
}
