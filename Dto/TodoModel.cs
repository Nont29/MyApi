namespace MyApi.Dto
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
    }

    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
    }

    public class ViewTodoResponse
    {
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
    }
}
