namespace GreatReports.Presentation.Requests;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
