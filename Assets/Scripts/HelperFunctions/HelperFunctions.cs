public static class HelperFunctions
{
    public static int ReverseClampToInt(int value, int min, int max)
    {
        if (value < min) value = max;
        else if (value > max) value = min;
        
        return value;
    }
}
