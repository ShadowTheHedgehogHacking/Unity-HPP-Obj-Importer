namespace HeroesPowerPlant.LayoutEditor
{
    public class Object0010_Ring : SetObjectShadow
    {

        public RingType RingType
        {
            get => (RingType)ReadInt(0);
            set => Write(0, (int)value);
        }

        public int NumberOfRings
        {
            get => ReadInt(4);
            set => Write(4, value);
}

        public float LengthRadius
        {
            get => ReadFloat(8);
            set => Write(8, value);
        }

        public float Angle  
        {
            get => ReadFloat(12);
            set => Write(12, value);
        }

        public bool Ghost
        {
            get => (ReadInt(16) != 0);
            set => Write(16, value ? 1 : 0);
        }
    }
}
