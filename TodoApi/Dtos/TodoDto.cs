


namespace TodoApi.Dtos
{
    public record TodoDto
    {
           public string Title {get; set;} = string.Empty;
           public string Description {get; set;} = string.Empty;
           public bool IsDone {get; set;}
    }
}