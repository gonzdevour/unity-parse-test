using System.Collections.Generic;

public interface IStoryPlayer
{
    void Display(
        Dictionary<string, string> charData,
        string charUID,
        string charPos,
        string charEmo,
        string charSimbol,
        string charTone,
        string charEffect,
        string DisplayName,
        string Content
        );
    void SkipTyping();
    bool IsTyping();
    void ClearPortrait();
    void PortraitGoTo(string key);
}
