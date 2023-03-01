using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
public class AucRepo:IAucRepo
{


    private readonly AucDBContext _dbContext;


    public AucRepo(AucDBContext dbContext)
    {
        _dbContext = dbContext;
    }


    public string AddUser(User user)
    {
        var present = _dbContext.Users.FirstOrDefault(e => e.UserName == user.UserName);
        var admin_check = _dbContext.Admins.FirstOrDefault(e => e.UserName == user.UserName);
        if (present == null && admin_check == null)
        {

            EntityEntry<User> e = _dbContext.Users.Add(user);
            User c = e.Entity;
            _dbContext.SaveChanges();
            return "User successfully registered.";

        }
        else
        {

            return "Username not available.";

        }


    }

    public IEnumerable<Item> GetAllItems()
    {
        IEnumerable<Item> new_items = _dbContext.Items.ToList<Item>();
        IEnumerable<Item> new_item = new_items.Where(e => e.State == "active");
        return new_item;
    }
    public IEnumerable<Item> GetAllItemsAdmin()
    {
        IEnumerable<Item> new_itemss = _dbContext.Items.ToList<Item>();
       
        return new_itemss;
    }


    public Item GetItemByid(int id)
    {
        Item item = _dbContext.Items.FirstOrDefault(e => e.Id == id);
        return item;

    }

    public Item AddItem(Item item)
    {

       
        EntityEntry<Item> e = _dbContext.Items.Add(item);
        Item f = e.Entity;
        _dbContext.SaveChanges();

        return f;
    }


    public bool ItemCheck(int id, string name)
    {
        var c = _dbContext.Items.FirstOrDefault(e => e.Id == id && e.Owner == name);
        if (c == null)
            return false;
        else
            return true;
    }





    public bool ValidLogin(string userName, string password)
    {
        var c = _dbContext.Users.FirstOrDefault(e => e.UserName == userName && e.Password == password);
        if (c == null)
            return false;
        else
            return true;
    }

    public bool ValidLoginAdmin(string userName, string password)
    {
        var c = _dbContext.Admins.FirstOrDefault(e => e.UserName == userName && e.Password == password);
        if (c == null)
            return false;
        else
            return true;
    }




    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

}