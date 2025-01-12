using UnityEngine;
using System.Collections.Generic;

public interface IChar
{
    string UID { get; set; }
    void Init(Dictionary<string, string> charData, string charEmo = "�L");
    void Focus();
    void Unfocus();
    void SetExpression(string expression = "�L", string transitionType = "fade", float dur = 2f);
    void Move(Vector2[] fromTo, float dur);
    void MoveX(Vector2[] fromTo, float dur);
    void MoveY(Vector2[] fromTo, float dur);
}
