using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerExampleObject : MonoBehaviour, Writable
{
    public string a = "b";
    public int i = 1;
    public float f = 1.1f;
    public bool b = true;

    private class TestObject : Writable
    {
        public uint ui = 1;
    }

    void Start()
    {
        SaveObject();
    }

    public void SaveObject()
    {
        // Saving a single object...
        DataManager.Save(this)
        .Then(response =>
        {
            // Do something after object has been saved
            Debug.Log(response.message);
        }).Catch(error => {
            // Do something when either saves fail. Uncaught errors will be posted to the console
            Debug.LogError(error);
        });
    }

    public void SavingObjects()
    {
        List<DataManagerExampleObject> list = new List<DataManagerExampleObject>();

        list.Add(this);
        list.Add(this);

        // Saving a list of objects the the same time...
        // Then can take a delegate function which will be called
        DataManager.Save(list).Then(OnObjectsSaved);
    }

    public void OnObjectsSaved(ServerResponse response)
    {
        // Do something after objects have been saved
        Debug.Log(response.message);
    }

    public void SavingObjectsOfDifferentTypes()
    {
        List<Writable> list = new List<Writable>();

        list.Add(this);
        list.Add(new TestObject());

        // Saving a list of objects of different writable types at the same time...
        DataManager.Save(list);
    }

    public void LogLabelledEvent()
    {
        // Saves a string which can be used to track user interactions
        // For instance, it can be used to log when a player starts a level
        DataManager.Save("Level Started");
    }

    public void LogLabelledEventWithValue()
    {
        // Labelled events can take a value which can be used to specify something related to the event.
        // For instance, you could log a mouse click with its position, or level start together with the level number
        DataManager.Save("Level Started", 1);
    }

    public void PackagingObjects()
    {
        // When using the static Save function the DataManager handles when data it sent to the database
        // By instantiating your own DataManager, you can control when the data is sent to the database
        // This way you can package several save calls to be sent at the same time

        DataManager db = new DataManager();

        // Appends saved data of this object to the DataManager, but does not sent it to the database yet
        db.Append(this);

        // Flush is used on a DataManager object to sent the appended save data
        // The list of appended save data is cleared immediately upon flushing
        db.Flush();

        // IMPORTANT: DATA THAT IS NOT FLUSHED WILL BE LOST
        // This is okay as sometimes we do not care about incomplete data sets
        // But it is crucial when using append, that flush properly follows
    }

    public void PackagingObjectSnapshots()
    {
        // Differently from saving a list of objects, upon appending an object the DataManager takes a snapshot of that object
        // This means that changes to the same object are kept between appending it to the DataManager

        DataManager db = new DataManager();

        a = "b";
        db.Append(this);

        // The data of the first appended snapshot will not change when a is set to "c"
        a = "c";

        // Append and Flush return the DataManager object again to be used immediately
        db.Append(this)
        .Flush();
    }

    public void PackagingUserInteraction()
    {
        // Append can take all the same types as the static Save function, including strings for lebelled events

        DataManager db = new DataManager();

        // Instead of saving user interaction on the go, they can be appended to be saved later
        // A Unix timestamp in milliseconds is set for any data given to the DataManager, whether using Save or Append
        // Therefore the time at which user interaction is sent to the database is not important
        db.Append("Level Started", 1)
        .Append("Player Jumped")
        .Append("Killed a turtle")
        .Append("Player Died");

        // Save and Append can also take an array of Writable objects
        db.Append(new Writable[] { this, new TestObject() })
        .Flush();
    }
}
