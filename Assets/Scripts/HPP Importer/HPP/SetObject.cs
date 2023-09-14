using System.ComponentModel;
using UnityEngine;

public abstract class SetObject
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public byte List;
        public byte Type;
        public byte Link;
        public byte Rend;
        public byte[] UnkBytes;

        public byte[] MiscSettings;
      //  /**/[JsonIgnore]
        private ObjectEntry objectEntry;
        [Browsable(false)]
        public string GetName => objectEntry.GetName();
        protected int ModelMiscSetting => objectEntry.ModelMiscSetting;
        protected string[][] ModelNames => objectEntry.ModelNames;
        public bool HasMiscSettings;

        public override string ToString()
        {
            return objectEntry.GetName() + (Link == 0 ? "" : $" ({Link})");
        }

        public virtual void SetObjectEntry(ObjectEntry objectEntry)
        {
            this.objectEntry = objectEntry;
            this.HasMiscSettings = objectEntry.HasMiscSettings;
         }

/*        protected void SetDFFModels()
        {
            int modelNumber = (ModelMiscSetting != -1 && ModelMiscSetting < MiscSettings.Length) ?
               MiscSettings[ModelMiscSetting] : 0;

            if (ModelNames != null && ModelNames.Length != 0 && modelNumber < ModelNames.Length)
            {
                models = new RenderWareModelFile[ModelNames[modelNumber].Length];

                for (int i = 0; i < models.Length; i++)
                    if (Program.MainForm.renderer.dffRenderer.DFFModels.ContainsKey(ModelNames[modelNumber][i]))
                        models[i] = Program.MainForm.renderer.dffRenderer.DFFModels[ModelNames[modelNumber][i]];
                return;
            }
            models = null;
        }*/
    }

public class ObjectEntry
{
    public byte List;
    public byte Type;
    public string Name;
    public string DebugName;
    public int ModelMiscSetting;
    public string[][] ModelNames;
    public bool HasMiscSettings;
    public int MiscSettingCount;

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Name))
            return string.Format("{0, 2:X2} {1, 2:X2} {2}", List, Type, Name);
        else if (!string.IsNullOrWhiteSpace(DebugName))
            return string.Format("{0, 2:X2} {1, 2:X2} {2}", List, Type, DebugName);
        else
            return string.Format("{0, 2:X2} {1, 2:X2} {2}", List, Type, "Unknown / Unused");
    }

    public string GetName()
    {
        if (Name != "")
            return Name;
        if (DebugName != "")
            return DebugName;
        return "Unknown/Unused";
    }
}