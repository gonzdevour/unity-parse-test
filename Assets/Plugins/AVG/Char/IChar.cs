using UnityEngine;
using System.Collections.Generic;

public interface IChar
{
    string UID { get; set; }
    void Init(Dictionary<string, string> charData);
    void Focus();
    void Unfocus();
    void Move(Vector2[] fromTo, float dur);
    void MoveX(Vector2[] fromTo, float dur);
    void MoveY(Vector2[] fromTo, float dur);
}
