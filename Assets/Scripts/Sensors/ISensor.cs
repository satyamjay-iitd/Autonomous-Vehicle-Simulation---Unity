namespace Sensors
{
    public interface ISensor<out T>
    {
        T ReadData();
    }
}

