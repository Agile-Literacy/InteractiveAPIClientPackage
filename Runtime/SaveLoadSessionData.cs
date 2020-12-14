using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;



public static class SaveLoadSessionData 
{

    //void Start()
    //{

    //    //Save();
    //    //Load();
    //    if (Input.GetKeyDown(KeyCode.Y)) { Save(); }

    //}

    public static void Save()
    {
        Debug.Log("Delete me: Placeholder test save");
        // Check for events that have not been uploaded and add them to a new list to save locally
        List<MetricEvent> eventsToSave = new List<MetricEvent>();
        MetricEvent newEvent = new MetricEvent();
        newEvent.SetEventInfo("0", "a", "0001", "0a");
        QuizStart metricData = new QuizStart("quiz0");

        newEvent.Timestamp(DateTime.Now);
        newEvent.Data(metricData);

        eventsToSave.Add(newEvent);

        Save(eventsToSave);
        // Save the data to disk.
        //string filepath = Application.persistentDataPath + "/save.dat";

        //using (FileStream file = File.Create(filepath))
        //{
        //    new BinaryFormatter().Serialize(file, eventsToSave);
        //}
    }

    public static void Save(MetricEvent eventToSave)
    {
        Debug.Log("Save: " + eventToSave);
        // Check for events that have not been uploaded and add them to a new list to save locally
        List<MetricEvent> ListToSave = new List<MetricEvent>();

        // Save the data to disk.
        string filepath = Application.persistentDataPath + "/save.dat";

        //if there is already a save file we want to keep the data in it
        if (File.Exists(filepath))
        {
            ListToSave.AddRange(Load());
        }

        ListToSave.Add(eventToSave);

        using (FileStream file = File.Create(filepath))
        {
            new BinaryFormatter().Serialize(file, ListToSave);
        }

    }

    public static void Save(List<MetricEvent> eventsToSave)
    {
        Debug.Log("Save: " + eventsToSave.Count.ToString());
        // Check for events that have not been uploaded and add them to a new list to save locally
        List<MetricEvent> ListToSave = new List<MetricEvent>();

        // Save the data to disk.
        string filepath = Application.persistentDataPath + "/save.dat";

        //if there is already a save file we want to keep the data in it
        if (File.Exists(filepath))
        {
            ListToSave.AddRange(Load());
        }

        ListToSave.AddRange(eventsToSave);

        using (FileStream file = File.Create(filepath))
        {
            new BinaryFormatter().Serialize(file, ListToSave);
        }
    }

    public static void Save(MetricEvent[] eventsToSave)
    {
        Debug.Log("Save: " + eventsToSave.Length.ToString());
        // Check for events that have not been uploaded and add them to a new list to save locally
        List<MetricEvent> ListToSave = new List<MetricEvent>();

        // Save the data to disk.
        string filepath = Application.persistentDataPath + "/save.dat";

        //if there is already a save file we want to keep the data in it
        if (File.Exists(filepath))
        {
            ListToSave.AddRange(Load());
        }

        ListToSave.AddRange(eventsToSave);

        using (FileStream file = File.Create(filepath))
        {
            new BinaryFormatter().Serialize(file, ListToSave);
        }
    }


    public static List<MetricEvent> Load()
    {
        List<MetricEvent> listToReturn = new List<MetricEvent>();
        Debug.Log("Checking for metrics save file to load");
        string filepath = Application.persistentDataPath + "/save.dat";

        //TODO: check if file exists

        if (File.Exists(filepath))
        {
            Debug.Log("Metrics save file exists");
            //load the data from the file that needs to be uploaded
            
            using (FileStream file = File.Open(filepath, FileMode.Open))
            {
                object loadedData = new BinaryFormatter().Deserialize(file);
                listToReturn = (List<MetricEvent>)loadedData;
                Debug.Log(listToReturn.Count);

            }
        }

        //Delete the file once loaded, if it still isnt sent to the database the new events will be appended and re saved
        File.Delete(filepath);
        return listToReturn;
    }


    public static bool HasLocalSaveData()
    {

        string filepath = Application.persistentDataPath + "/save.dat";
        if (File.Exists(filepath))
        {
            return true;
        }

        return false;
    }

}
