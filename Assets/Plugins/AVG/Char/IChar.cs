using UnityEngine;
using System.Collections.Generic;

public interface IChar
{
    GameObject GetGameObject();
    string UID { get; set; }
    string Expression { get; set; }
    void Init(Dictionary<string, string> charData, string charEmo = "�L", string charSimbol = "�L");
    void Focus(float dur = 0.5f);
    void Unfocus(float dur = 0f);
    void SetExpression(string expression = "�L", string transitionType = "fade", float dur = 1f);
    void SetSimbol(string simbol);
    void Move(Vector2[] fromTo, float dur);
    void MoveX(Vector2[] fromTo, float dur);
    void MoveY(Vector2[] fromTo, float dur);
}
