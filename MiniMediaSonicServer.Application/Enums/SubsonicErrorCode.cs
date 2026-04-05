namespace MiniMediaSonicServer.Application.Enums;

public enum SubsonicErrorCode
{
    GenericError = 0,
    RequiredParameter = 10,
    IncompatibleVersionClientMustUpgrade = 20,
    IncompatibleVersionServerMustUpgrade = 30,
    WrongUsernameOrPassword = 40,
    TokenAuthenticationNotSupportedForLdapUsers = 41,
    ProvidedAuthenticationMechanismNotSupported = 42,
    ConflictingAuthentication = 43,
    InvalidApiKey = 44,
    UserNotAuthorized = 50,
    TrialPeriodOver = 60,
    DataNotFound = 70
}