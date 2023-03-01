public interface IAucRepo
{

    public string AddUser(User user);
    public IEnumerable<Item> GetAllItems();
    public Item GetItemByid(int id);
    public Item AddItem(Item username);
    public IEnumerable<Item> GetAllItemsAdmin();
    public bool ItemCheck(int id, string name);
    public bool ValidLogin(string userName, string password);
    public bool ValidLoginAdmin(string userName, string password);
    public void SaveChanges();
}