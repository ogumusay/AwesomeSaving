using AwesomeSaving;
using UnityEngine;

namespace AwesomeSavingExample
{
    public class SavingExample : MonoBehaviour
    {
        private string APP_PATH;
        private SavingHelper _savingHelper;

        private void Awake()
        {
            APP_PATH = Application.persistentDataPath;
            _savingHelper = SavingHelper.GetInstance();
        }

        void Start()
        {
            SaveData saveData1 = new SaveData(String: "Some important data", Integer: 10);
            SaveData saveData2 = new SaveData(String: "Some more important data", Integer: 20);

            _savingHelper.Save(APP_PATH + "/" + "saveData_1.dat", saveData1, onComplete: OnSaveData1);
            _savingHelper.Save(APP_PATH + "/" + "saveData_2.dat", saveData2, onComplete: OnSaveData2);
        }

        private void OnSaveData1(object obj)
        {
            SaveData savedData = _savingHelper.Load<SaveData>(APP_PATH + "/" + "saveData_1.dat");
            Debug.Log("SaveData_1");
            Debug.Log("String: " + savedData.String);
            Debug.Log("Integer: " + savedData.Integer);
        }

        private void OnSaveData2(object obj)
        {
            SaveData savedData = _savingHelper.Load<SaveData>(APP_PATH + "/" + "saveData_2.dat");
            Debug.Log("SaveData_2");
            Debug.Log("String: " + savedData.String);
            Debug.Log("Integer: " + savedData.Integer);
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public string String;
        public int Integer;

        public SaveData(string String, int Integer)
        {
            this.String = String;
            this.Integer = Integer;
        }
    }
}