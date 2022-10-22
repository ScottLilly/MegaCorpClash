namespace MegaCorpClash.Models;

public class ChatterDetails
{
    public string ChatterId { get; set; }
    public string ChatterName { get; set; }
    public Company? Company { get; set; }

    public ChatterDetails(string chatterId, string chatterName, Company? company)
    {
        ChatterId = chatterId;
        ChatterName = chatterName;
        Company = company;
    }

}
