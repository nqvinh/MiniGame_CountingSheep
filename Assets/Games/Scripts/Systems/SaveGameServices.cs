using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SaveGameServices : Singleton<SaveGameServices>
{
   
    public bool encode = false;
    public string encodePassword = "";
    public ISaveGameSerializer serializer;
    public ISaveGameEncoder encoder;

    public Encoding encoding;

    public SaveGamePath savePath = SaveGamePath.PersistentDataPath;

    private readonly string gameId = "CountingMergeSheep";
    PlayerData localPlayerData = new PlayerData();

    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(encodePassword))
        {
            encodePassword = SaveGame.EncodePassword;
        }
        if (serializer == null)
        {
            serializer = SaveGame.Serializer;
        }
        if (encoder == null)
        {
            encoder = SaveGame.Encoder;
        }
        if (encoding == null)
        {
            encoding = SaveGame.DefaultEncoding;
        }

        serializer = new SaveGameJsonSerializer();

     
    }


    /// <summary>
    /// Save this instance.
    /// </summary>
    public virtual void Save(PlayerData playerData)
    {
        
        SaveGame.Save<PlayerData>(gameId, playerData,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);
    }

    /// <summary>
    /// Load this instance.
    /// </summary>
    public virtual PlayerData Load()
    {
        localPlayerData= SaveGame.Load<PlayerData>(gameId, localPlayerData, 
            encode, 
            encodePassword, 
            serializer, 
            encoder, 
            encoding, 
            savePath);
        return localPlayerData;
    }

}
