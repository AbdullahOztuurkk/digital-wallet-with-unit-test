namespace Wallet.Domain.Constants;
public static class SystemError
{
    public static readonly Error E_0001 = new Error(nameof(E_0001),"Email is invalid");
    public static readonly Error E_0002 = new Error(nameof(E_0002),"Msisdn is invalid.");
    public static readonly Error E_0003 = new Error(nameof(E_0003),"Wallet not found.");
    public static readonly Error E_0004 = new Error(nameof(E_0004),"Wallet already blocked");
    public static readonly Error E_0005 = new Error(nameof(E_0005),"From Wallet Number field is required.");
    public static readonly Error E_0006 = new Error(nameof(E_0006),"Amount must be greater than zero.");
    public static readonly Error E_0007 = new Error(nameof(E_0007),"To Wallet Number field is required.");
    public static readonly Error E_0008 = new Error(nameof(E_0008),"To Account Number field is required.");
    public static readonly Error E_0009 = new Error(nameof(E_0009),"Transfer operation has been canceled due of sender.");
    public static readonly Error E_0010 = new Error(nameof(E_0010),"Transfer operation has been canceled due of insufficient balance.");
    public static readonly Error E_0011 = new Error(nameof(E_0011),"Transfer operation has been canceled due of time exceeded.");
    public static readonly Error E_0012 = new Error(nameof(E_0012),"Transfer operation has been canceled due of transaction status.");
    public static readonly Error E_0013 = new Error(nameof(E_0013),"Transfer operation has been canceled due of unmatched amount.");
    public static readonly Error E_0014 = new Error(nameof(E_0014),"Transaction cannot be found!.");
    public static readonly Error E_0015 = new Error(nameof(E_0015),"Invalid Transaction!.");
}
