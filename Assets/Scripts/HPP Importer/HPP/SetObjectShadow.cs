using System;

    public abstract class SetObjectShadow : SetObject
    {
        public SetObjectShadow()
        {
            UnkBytes = new byte[8];
            MiscSettings = new byte[0];
        }
        
        public string DefaultMiscSettingCount { get; private set; }

        public override void SetObjectEntry(ObjectEntry objectEntry)
        {
            base.SetObjectEntry(objectEntry);

            if (objectEntry.MiscSettingCount == -1)
                DefaultMiscSettingCount = "Unknown";
            else
                DefaultMiscSettingCount = (objectEntry.MiscSettingCount / 4).ToString();
        }

        public int ReadInt(int j) => BitConverter.ToInt32(MiscSettings, j);

        public float ReadFloat(int j) => BitConverter.ToSingle(MiscSettings, j);

        public void Write(int j, int value)
        {
            for (int i = 0; i < 4; i++)
                MiscSettings[j + i] = BitConverter.GetBytes(value)[i];
        }

        public void Write(int j, float value)
        {
            for (int i = 0; i < 4; i++)
                MiscSettings[j + i] = BitConverter.GetBytes(value)[i];
        }
    }
