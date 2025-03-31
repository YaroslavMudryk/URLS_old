using URLS.Application.ViewModels.Group;

namespace URLS.Application.Validations;

public static class GroupValidation
{
    public static bool TryValidateGroupName(this GroupCreateModel group, out string error)
    {
        if (!group.Name.Contains("-"))
        {
            error = "Name must be definition \"-\"";
            return false;
        }
        error = null;
        return true;
    }

    public static bool TryGetIndexForNumber(this GroupCreateModel group, out int index)
    {
        if (group.Name.Contains('-'))
        {
            var indexNumber = group.Name.IndexOf("-");
            index = indexNumber + 1;
            return true;
        }
        index = 0;
        return false; 
    }
}
