namespace Kompetenzgipfel.Models;

public class Topic
{
    public int Id { get; set; }
    public string Title;

    public Topic(string title)
    {
        Title = title;
    }
    
    internal Topic()
    {}
}