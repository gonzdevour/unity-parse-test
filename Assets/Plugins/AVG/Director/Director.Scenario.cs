using System.Linq;

public partial class Director
{
    private void Cut(object[] args = null)
    {
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
