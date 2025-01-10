namespace AppGoodFriendsMVC.Models;

public class FriendsByCityViewModel
{
    public string ChosenCountry { get; set; }
    public Dictionary<string, int> FriendsByCity { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> PetsByCity { get; set; } = new Dictionary<string, int>();
}