public partial class Director
{
    private void SetMoney(object[] args = null)
    {
        string input = args[0].ToString();
        PPM.Inst.Set("����", input);
    }
}
