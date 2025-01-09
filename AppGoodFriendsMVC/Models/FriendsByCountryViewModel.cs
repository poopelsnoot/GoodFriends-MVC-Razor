namespace AppGoodFriendsMVC.Models;

public class FriendsByCountryViewModel
{
    public Dictionary<string, int> FriendsByCountry { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> CitiesByCountry { get; set; } = new Dictionary<string, int>();
}