using UnityEngine;
using System.Collections.Generic;

public interface IChar
{
    string UID { get; set; }
    void Init(Dictionary<string, string> charData, string charEmo = "�L");
    void Focus(float dur = 0.5f);
    void Unfocus(float dur = 0f);
    void SetExpression(string expression = "�L", string transitionType = "fade", float dur = 1f);
    void Move(Vector2[] fromTo, float dur);
    void MoveX(Vector2[] fromTo, float dur);
    void MoveY(Vector2[] fromTo, float dur);
}
