namespace MegaCorpClash.Models;

public class Player
{
    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string DisplayName { get; set; }
    public string CompanyName { get; set; }
    public int Points { get; set; }
}