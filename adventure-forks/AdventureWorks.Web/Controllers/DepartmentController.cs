using System.Web.Mvc;
using AdventureWorks.Services.HumanResources;
using Microsoft.ApplicationInsights;

namespace AdventureWorks.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Departments
        public ActionResult Index()
        {
            var departmentGroups = _departmentService.GetDepartments();

            return View(departmentGroups);
        }

        // GET: Departments/Employees/{id}
        public ActionResult Employees(int id)
        {
            var departmentEmployees = _departmentService.GetDepartmentEmployees(id);
            var departmentInfo = _departmentService.GetDepartmentInfo(id);

            ViewBag.Title = "Employees in " + departmentInfo.Name + " Department";

            return View(departmentEmployees);
        }
    }
}
