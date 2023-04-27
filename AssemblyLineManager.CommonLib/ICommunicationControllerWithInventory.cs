namespace AssemblyLineManager.CommonLib;

public interface ICommunicationControllerWithInventory : ICommunicationController{
    /**
     * This method is used to get the current inventory of the machine utilised by the module.
     * The inventory information shall be formatted as a list of information with each entry having a numeric id and a name.
     * It shall return an array of key-value pairs, where the key is the name of the entry, and the value is the value of the entry.
     */
    public KeyValuePair<int, string>[] GetInventory();
}