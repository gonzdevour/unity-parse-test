using UnityEngine;

public partial class Director
{
    private void GoTo(object[] args = null)
    {
        string nextStoryTitle = args[0]?.ToString();
        AVG.Inst.PrependStoryByTitle(nextStoryTitle);//強制指定下一個故事
        AVG.Inst.nextCutIndex = 99999; //使下一卡索引超出範圍，強制結束
    }

    private void Cut(object[] args = null)
    {
        Debug.Log("Set nextCutIndex = 99999");
        AVG.Inst.nextCutIndex = 99999; //使下一卡索引超出範圍，強制結束
    }

    private void FadeIn(object[] args = null)
    {
        TEffect.FadeIn();
    }

    private void FadeOut(object[] args = null)
    {
        TEffect.FadeOut();
    }
}
