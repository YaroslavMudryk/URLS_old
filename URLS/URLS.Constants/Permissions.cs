namespace URLS.Constants;

public class Permissions
{
    public const string CanCreate = "Create";
    public const string CanEdit = "Edit";
    public const string CanRemove = "Remove";
    public const string CanView = "View";
    public const string CanViewAll = "View all";
    public const string CanViewAllGroups = "View all groups";
    public const string CanViewAllSpecialties = "View all specialties";
    public const string Search = "Search";
    public const string All = "All";
    public const string Logout = "Logout";

    public static IEnumerable<string> GetBasic()
    {
        yield return CanCreate;
        yield return CanEdit;
        yield return CanRemove;
        yield return CanView;
    }

    public static IEnumerable<string> GetAll()
    {
        yield return CanCreate;
        yield return CanEdit;
        yield return CanRemove;
        yield return CanView;
        yield return CanViewAll;
        yield return Search;
    }
}
