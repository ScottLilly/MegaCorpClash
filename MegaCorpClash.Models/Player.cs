namespace MegaCorpClash.Models;

public class Player
{
    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string DisplayName { get; set; }
    public string CompanyName { get; set; }
    public string Motto { get; set; } = "We don't need a motto";
    public int Points { get; set; }
}