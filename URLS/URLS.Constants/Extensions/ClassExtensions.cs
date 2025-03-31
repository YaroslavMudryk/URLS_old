namespace URLS.Constants.Extensions;

public static class ClassExtensions
{
    public static string NotFoundMessage(this Type type, object id)
    {
        return $"{type.Name} with ID ({id.ToString()}) not found";
    }
}
