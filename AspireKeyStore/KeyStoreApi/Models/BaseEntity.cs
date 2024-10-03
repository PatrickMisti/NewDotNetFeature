namespace KeyStoreApi.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public bool Deleted { get; set; } = false;
}