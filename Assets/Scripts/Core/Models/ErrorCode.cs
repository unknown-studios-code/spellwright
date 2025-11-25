namespace Spellwright.Core.Models
{
    public enum ErrorCode
    {
        None = 0,
        Unknown,
        ConfigMissing,
        ProviderMissing,
        InvalidConfig,
        ApiKeyMissing,
        PayloadCreationFailure,
        ParseError,
        RetryExhausted,
        ServiceNotInitialized,
    }
}
