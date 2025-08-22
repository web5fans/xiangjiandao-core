namespace Xiangjiandao.Web.Utils;

public class SignTransaction
{
    public List<SignTransactionWitness> Witnesses { get; set; } = [];
}

public class SignTransactionWitness
{
    public string Lock { get; set; } = string.Empty;
}