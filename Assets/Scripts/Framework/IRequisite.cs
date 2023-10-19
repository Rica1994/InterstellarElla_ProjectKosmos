public interface IRequisite
{
    bool IsRequisiteMet();
}
public enum RequisiteLogic
{
    All,
    Any
}