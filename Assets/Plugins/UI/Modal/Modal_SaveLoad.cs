public class Modal_SaveLoad : ModalReciever
{
    public AVGSaveSlotList avgSaveSlotList;
    private void OnEnable()
    {
        avgSaveSlotList.Init(activatorName);
    }
}