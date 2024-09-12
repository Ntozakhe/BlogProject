namespace BlogProjectPrac7.Services.Interfaces
{
    public interface ISlugService
    {
        //This takes in a title and turn it into a url
        string UrlFriendly(string title);
        //determine whether a slug is unique
        bool isUnique(string slug);
    }
}
