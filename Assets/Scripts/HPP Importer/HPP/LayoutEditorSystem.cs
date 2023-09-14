using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using static HeroesPowerPlant.LayoutEditor.LayoutEditorFunctions;

namespace HeroesPowerPlant.LayoutEditor
{
    public class LayoutEditorSystem
    {
        private static bool isShadow;
        public bool IsShadow => isShadow;

        private string currentlyOpenFileName;
        public string CurrentlyOpenFileName => currentlyOpenFileName;

        public static Dictionary<(byte, byte), ObjectEntry> shadowObjectEntries { get; private set; }
        public static ObjectEntry shadowObjectEntry(byte List, byte Type) => shadowObjectEntries[(List, Type)];

        public bool autoUnkBytes;

        public static void SetupLayoutEditorSystem(){
            string filePath = Path.Combine(Application.dataPath, "Scripts\\HPP Importer\\ShadowObjectList.ini");
            shadowObjectEntries = ReadObjectListData(filePath);
        }

        private BindingList<SetObject> setObjects { get; set; } = new BindingList<SetObject>();

        public int GetSetObjectAmount()
        {
            return setObjects.Count;
        }

        public SetObject GetSetObjectAt(int index)
        {
            return setObjects[index];
        }

        public static IEnumerable<ObjectEntry> GetAllObjectEntries()
        {
            List<ObjectEntry> list = new List<ObjectEntry>();
            list.AddRange(shadowObjectEntries.Values);

            return list;
        }

        public static ObjectEntry[] GetActiveObjectEntries()
        {
            return shadowObjectEntries.Values.ToArray();
        }

        public (byte, byte)[] GetAllCurrentObjectEntries()
        {
            HashSet<(byte, byte)> objectEntries = new HashSet<(byte, byte)>();

            foreach (SetObject s in setObjects)
                if (!objectEntries.Contains((s.List, s.Type)))
                    objectEntries.Add((s.List, s.Type));

            return objectEntries.ToArray();
        }

        public void NewShadowLayout()
        {
            isShadow = true;
            currentlyOpenFileName = null;
            setObjects.Clear();
        }
    }
}