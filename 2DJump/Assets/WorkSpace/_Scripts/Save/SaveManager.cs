using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    public PlayerDetails_SO playerDetails;

    private void OnEnable()
    {
        EventHandler.AddEventListener<Vector3>("SaveEvent", Save);
        EventHandler.AddEventListener("LoadEvent", Load);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<Vector3>("SaveEvent", Save);
        EventHandler.RemoveEventListener("LoadEvent", Load);
    }

    public void Save(Vector3 savePosition)
    {
        SaveData saveData = CreatSaveData(savePosition);
        string JsonString = JsonUtility.ToJson(saveData);
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/JSONData.text");
        streamWriter.Write(JsonString);
        streamWriter.Close();
        Debug.Log("Save");
    }

    public SaveData CreatSaveData(Vector3 savePosition)
    {
        SaveData saveData = new SaveData();
        ResetPlayerDetails(saveData);
        saveData.sceneName = SceneManager.GetActiveScene().name;
        saveData.targetPos = savePosition;
        return saveData;
    }

    public void Load()
    {
        if (File.Exists(Application.dataPath + "/JSONData.text"))
        {
            StreamReader streamReader = new StreamReader(Application.dataPath + "/JSONData.text");
            string JsonString = streamReader.ReadToEnd();
            streamReader.Close();

            SaveData saveData = JsonUtility.FromJson<SaveData>(JsonString);
            ResetPlayerDetails(saveData);
            EventHandler.TriggerEvent<string, Vector3>("TransitionScene", saveData.sceneName, saveData.targetPos);
        }
        else
        {
            Debug.Log("FileNotExists");
        }
    }

    private void ResetPlayerDetails(SaveData saveData)
    {
        saveData.playerDetails = ScriptableObject.CreateInstance<PlayerDetails_SO>();
        playerDetails.heath = saveData.playerDetails.heath;
        playerDetails.energy = saveData.playerDetails.energy;
        playerDetails.enhancementPoints = saveData.playerDetails.enhancementPoints;
        playerDetails.hasDrillingBit = saveData.playerDetails.hasDrillingBit;
        playerDetails.hasConvolute = saveData.playerDetails.hasConvolute;
        EventHandler.TriggerEvent<float, float>("UpdatePlayerHealth", playerDetails.heath, playerDetails.maxHeath);
        EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
        EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
        EventHandler.TriggerEvent<bool>("UpdateDrillingBitEvent", playerDetails.hasDrillingBit);
    }
}
