public interface ISavable<T> where T : ISavable<T>
{
    /// <summary>
    /// sauvegarde l'objet 
    /// </summary>
    /// <param name="SaveFileID"></param>
    public abstract void Save(byte SaveFileID);

    public abstract T Load(byte SaveFileID) ;

}

