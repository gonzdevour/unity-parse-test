public partial class Director
{
    public void PortraitIn(string charUID, string charEmo)
    {
        var key = charUID + charEmo;
        Avg.StoryPlayer.PortraitGoTo(key);
    }
}
