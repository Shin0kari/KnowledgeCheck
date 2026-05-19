public class InventoryPanelLinker : AbstractSecondUIPanelLinker, IStartMenuPanel, IBindingSingletonComponent
{
    public void ActivatePanel()
    {
        LinkerObject.SetActive(true);
    }
}
