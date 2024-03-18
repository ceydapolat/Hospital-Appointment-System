namespace Services;

public class GenderHelper
{
    public static string ConvertToTurkish(string gender)
    {
        switch (gender.ToLower())
        {
            case "male":
                return "Erkek";
            case "female":
                return "Kadın";
            default:
                return gender; // If neither "male" nor "female", return the original value
        }
    }
}
