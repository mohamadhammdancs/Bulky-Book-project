using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
		public IActionResult Details(int orderId)
		{
			OrderVM orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return View(orderVM);
		}

		#region API CALLS

		[HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeadrers = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser").ToList();

			switch (status)
			{
				case "pending":
                    objOrderHeadrers = objOrderHeadrers.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
					break;
				case "inprocess":
					objOrderHeadrers = objOrderHeadrers.Where(u => u.PaymentStatus == SD.StatusInProcess);
					break;
				case "completed":
					objOrderHeadrers = objOrderHeadrers.Where(u => u.PaymentStatus == SD.StatusRefunded);
                    break;
				case "approved":
					objOrderHeadrers = objOrderHeadrers.Where(u => u.PaymentStatus == SD.StatusApproved);
                    break;
				default:
					break;
			}

			return Json(new { data = objOrderHeadrers });
        }


        

        #endregion

    }
}
