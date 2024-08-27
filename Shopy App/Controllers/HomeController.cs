using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopy_App.Models;
using Shopy_App.Services;
using System.Linq;

namespace Shopy_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchQuery = "")
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery));
            }

            return View(products.ToList());
        }

        // Add product to cart
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            string cartId = GetCartId();

            var cartItem = _context.ShoppingCartItems
                                   .SingleOrDefault(c => c.ProductId == id && c.CartId == cartId);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = id,
                    Product = product,
                    Quantity = 1,
                    CartId = cartId
                };

                _context.ShoppingCartItems.Add(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {
            string cartId = GetCartId();
            var cartItems = _context.ShoppingCartItems
                                   .Include(c => c.Product)
                                   .Where(c => c.CartId == cartId)
                                   .ToList();
            return View(cartItems);
        }

        public IActionResult Checkout()
        {
            string cartId = GetCartId();
            var cartItems = _context.ShoppingCartItems
                                   .Include(c => c.Product)
                                   .Where(c => c.CartId == cartId)
                                   .ToList();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Save order details in the database
            string cartId = GetCartId();
            var cartItems = _context.ShoppingCartItems
                                   .Include(c => c.Product)
                                   .Where(c => c.CartId == cartId)
                                   .ToList();

            order.OrderDate = DateTime.Now;
            order.OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                Price = item.Product.Price
            }).ToList();

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the cart
            _context.ShoppingCartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        private string GetCartId()
        {
            // Generate a unique identifier for the cart, typically based on user ID or session
            if (User.Identity.IsAuthenticated)
            {
                return User.Identity.Name; // Use authenticated user name as CartId
            }
            else
            {
                // For anonymous users, generate a unique cart ID, for example, using session
                if (HttpContext.Session.GetString("CartId") == null)
                {
                    HttpContext.Session.SetString("CartId", Guid.NewGuid().ToString());
                }
                return HttpContext.Session.GetString("CartId");
            }
        }
    }
}
