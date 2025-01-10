using Models;

namespace AppGoodFriendsMVC.Models;

public class ListOfFriendsViewModel
{
    public List<IFriend> FriendsList { get; set; } = new List<IFriend>();
    public string ChosenCity { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; } 
}