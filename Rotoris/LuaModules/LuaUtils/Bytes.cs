namespace Rotoris.LuaModules.LuaUtils
{
    /*
--- @class Bytes
     */
    public class Bytes
    {
        public byte[] Data { get; }
        public Bytes(int size)
        {
            Data = new byte[size];
        }

        public Bytes(byte[] data)
        {
            Data = data;
        }
    }
}
