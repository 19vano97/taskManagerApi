using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController(ITaskItemService taskItemService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<TaskItemOutputDto>>> GetTasks()
        {
            return Ok(await taskItemService.GetTasksAsync());
        }

        [HttpPost("create")]
        public async Task<ActionResult<TaskItemOutputDto>> CreateTask(TaskItemInputDto newTask)
        {
            var createdTask = await taskItemService.CreateTaskAsync(newTask);

            if (createdTask is null)
                return BadRequest("Empty title");

            return Ok(createdTask);
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<TaskItemOutputDto>> GetTaskById(Guid Id)
        {
            var task = await taskItemService.GetTaskByIdAsync(Id);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("edit/{Id}")]
        public async Task<ActionResult<TaskItemOutputDto>> EditTaskById(Guid Id, [FromBody] TaskItemInputDto editTask)
        {
            var taskToEdit = await taskItemService.EditTaskByIdAsync(Id, editTask);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/assignee/{assigneeId}")]
        public async Task<ActionResult<TaskItemOutputDto>> ChangeTaskAssigneeById(Guid taskId, Guid assigneeId)
        {
            var taskToEdit = await taskItemService.ChangeAssigneeAsync(taskId, assigneeId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/project/{projectId}")]
        public async Task<ActionResult<TaskItemOutputDto>> ChangeTaskProject(Guid Id, Guid projectId)
        {
            var taskToEdit = await taskItemService.ChangeAssigneeAsync(Id, projectId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/task/{statusId}")]
        public async Task<ActionResult<TaskItemOutputDto>> ChangeTaskStatus(Guid Id, int statusId)
        {
            var taskToEdit = await taskItemService.ChangeTaskStatusAsync(Id, statusId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<TaskItemOutputDto>> DeleteTaskById(Guid Id)
        {
            var taskToDelete = await taskItemService.DeleteTaskAsync(Id);

            if (taskToDelete is null)
                return NotFound();

            return Ok(taskToDelete);
        }
    }
}
