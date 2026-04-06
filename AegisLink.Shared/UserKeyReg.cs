namespace AegisLink.Shared;

public record UserKeyReg(string AegisId, string PublicKey);

public record UserKeyGetResult(string AegisId, string PublicKey);
