namespace Products.Model.Enums
{
    public enum AccountStatus
    {
        AwaitingActivation = 0,
        Active = 1,
        Disabled = 2,
        // ReSharper disable once UnusedMember.Global
        ForgottenPassword = 3,
        // ReSharper disable once UnusedMember.Global
        Unverified = 4,
        Generated = 5
    }
}
