using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;

[Route("api")]
[ApiController]
public class AuctionController : Controller
{

    private readonly IAucRepo _repository;

    public AuctionController(IAucRepo repository)
    {
        _repository = repository;
    }




    [HttpPost("Register")]
    public ActionResult<string> Register(User user)
    {

        User c = new User { UserName = user.UserName, Password = user.Password};
        string message = _repository.AddUser(user);
        return Ok(message);

    }

    [HttpGet("ListItems")]
    public ActionResult<IEnumerable<Item>> GetItems()
    {
        IEnumerable<Item> items = _repository.GetAllItems();
        IEnumerable<Item> query =
            items.OrderBy(e => e.StartBid).ThenBy(e => e.Id);

        return Ok(query);
    }

    [HttpGet("GetItemPhoto/{id}")]
    public IActionResult GetItemPhoto(long id)
    {
        long id1 = id;
        string the_id = id1.ToString();
        string path = Directory.GetCurrentDirectory();
        string imgDir = Path.Combine(path, "Photos");
        string fileName1 = Path.Combine(imgDir, the_id + ".jpeg");
        string fileName2 = Path.Combine(imgDir, the_id + ".gif");
        string fileName3 = Path.Combine(imgDir, the_id + ".png");
       



        if (System.IO.File.Exists(fileName1))
        {
            Byte[] s = System.IO.File.ReadAllBytes(fileName1);
            return File(s, "image/jpeg");

        }

        else if (System.IO.File.Exists(fileName2))
        {
            Byte[] s = System.IO.File.ReadAllBytes(fileName2);
            return File(s, "image/gif");
        }

        else if (System.IO.File.Exists(fileName3))
        {
            Byte[] s = System.IO.File.ReadAllBytes(fileName3);
            return File(s, "image/png");
        }


        

            return Ok("Item id is not recognizable");
        



    }



    [HttpGet("GetItem/{id}")]
    public ActionResult<Item> GetItem(int id)
    {
        Item item_wanted = _repository.GetItemByid(id);

       
            return Ok(item_wanted);
        

    }


    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    [HttpPost("AddItem")]
    public ActionResult<Item> Adding_Item(ItemInput input)
    {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
        var username = credentials[0];
        
        Item new_item = new Item { Title = input.title, Description = input.description };
        if (input.startBid == null) new_item.StartBid = 0;
        else new_item.StartBid = input.startBid;
        new_item.State = "active";
        new_item.Owner = username;
        
        Item new_added_item = _repository.AddItem(new_item);
        return Ok(new_added_item);


       

    }



    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("ListItemsAdmin")]
    public ActionResult<IEnumerable<Item>> ItemsForAdmin()
    {
        IEnumerable<Item> items1 = _repository.GetAllItemsAdmin();
        IEnumerable<Item> query2 = items1.OrderBy(e => e.Id);
            
        return Ok(query2);
    }



    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserAndAdmin")]
    [HttpGet("CloseAuction/{id}")]
    public ActionResult<string> CloseAuction(int id)
    {

        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
        var username = credentials[0];
        var password = credentials[1];
        Item moded_item = _repository.GetItemByid(id);
        if (moded_item == null) return "Auction does not exist.";
        if ( _repository.ValidLoginAdmin(username,password))
        {

           

            

            moded_item.State = "closed";
            _repository.SaveChanges();
            return "Auction closed.";


        }


        if(moded_item.Owner == username)
        {
            moded_item.State = "closed";
            _repository.SaveChanges();
            return "Auction closed.";

        }

        return "You are not the owner of the auction.";

    }



    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    [HttpPost("UploadImage")] // Image name must match item id to be accepted.
    public ActionResult<string> UploadImage(IFormFile file)
    {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
        var username = credentials[0];







        string path = Directory.GetCurrentDirectory();
        string imgDir = Path.Combine(path, "Photos");

        string filePath = Path.Combine(imgDir, file.FileName);
        var credential = file.FileName.Split(".");
        if (_repository.ItemCheck(Int32.Parse(credential[0]), username))
        {
            using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            return "Image uploaded successfully.";
        }

        else return "You do not own the item.";


    }





}