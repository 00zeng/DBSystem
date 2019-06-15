using Base.Code;
using System.Web.Mvc;

namespace ProjectWeb
{
    public abstract class ControllerBase : Controller
    {
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult New()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Add()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Update()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Edit()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Show()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Approve() 
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApproveIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult HistoryIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ImportHistoryIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmpIndex()
        {
            return View();
        }
        protected virtual ActionResult Success(string message)
        {
            return Content(new AjaxResult { state = ResultType.success.ToString(), message = message }.ToJson());
        }
        protected virtual ActionResult Success(string message, object data)
        {
            return Content(new AjaxResult { state = ResultType.success.ToString(), message = message, data = data }.ToJson());
        }
        protected virtual ActionResult Error(string message)
        {
            return Content(new AjaxResult { state = ResultType.error.ToString(), message = message }.ToJson());
        }
    }
}
