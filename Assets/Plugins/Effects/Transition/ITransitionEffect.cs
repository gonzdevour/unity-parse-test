public interface ITransitionEffect
{
    void FadeIn(System.Action onComplete = null);
    void FadeOut(System.Action onComplete = null);
    void Stop(bool instant = true);
}