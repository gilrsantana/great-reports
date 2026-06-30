namespace GreatReports.Presentation.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
