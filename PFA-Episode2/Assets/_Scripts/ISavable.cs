public interface ISavable<T> where T : ISavable<T>
{
    /// <summary>
    /// sauvegarde l'objet 
    /// </summary>
    /// <param name="SaveFileID"></param>
    public abstract void Save(string saveName);
}

