using System.Linq;

public partial class Director
{
    private void Cut(object[] args = null)
    {
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
