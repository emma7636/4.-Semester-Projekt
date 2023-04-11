namespace AssemblyLineManager.CommonLib{
    public interface ICommunicationControllerWithInventory : ICommunicationController{
        public KeyValuePair<int, string>[] GetInventory();
    }
}