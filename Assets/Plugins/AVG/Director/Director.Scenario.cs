using UnityEngine;

public partial class Director
{
    private void GoTo(object[] args = null)
    {
        string nextStoryTitle = args[0]?.ToString();
        AVG.Inst.PrependStoryByTitle(nextStoryTitle);//�j����w�U�@�ӬG��
        AVG.Inst.nextCutIndex = 99999; //�ϤU�@�d���޶W�X�d��A�j���
    }

    private void Cut(object[] args = null)
    {
        Debug.Log("Set nextCutIndex = 99999");
        AVG.Inst.nextCutIndex = 99999; //�ϤU�@�d���޶W�X�d��A�j���
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
