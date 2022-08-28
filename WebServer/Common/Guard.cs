public class Guard
{
    public static void AgainstNull(object value, string? name = null)
    {
        //TODO Migh have problems there fix name ??
        if (value == null)
        {
            name??= "Value";

            throw new ArgumentException($"{name} cannot be null.");
        }
    }
}