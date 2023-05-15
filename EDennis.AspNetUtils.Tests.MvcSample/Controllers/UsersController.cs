using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils.Tests.MvcSample.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ICrudService<AppUser> _appUserService;
        private readonly ICrudService<AppRole> _appRoleService;
        private readonly SecurityOptions _securityOptions;

        public UsersController(ICrudService<AppUser> appUserService,
            ICrudService<AppRole> appRoleService,  
            IOptionsMonitor<SecurityOptions> securityOptions)
        {
            _appUserService = appUserService;
            _appRoleService = appRoleService;
            _securityOptions = securityOptions.CurrentValue;
        }

        public async Task<IActionResult> Index()
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            var (recs, _) = await Task.Run(() => _appUserService.GetAsync());

            return View(recs.ToList());
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            if (id == null)
                return NotFound();

            var appUser = await Task.Run(() => _appUserService.FindAsync(id));

            if (appUser == null)
                return NotFound();

            return View(appUser);
        }

        private List<AppRole> GetAppRoles() 
            => _appRoleService.GetAsync().Result.Data;

        // GET: AppUsers/CreateAsync
        public IActionResult Create()
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            ViewData["Role"] = new SelectList(GetAppRoles(), "Id", "RoleName");
            ViewData["AllowMultipleRoles"] = _securityOptions.AllowMultipleRoles;
            return View();
        }

        // POST: AppUsers/CreateAsync
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("UserName,RoleId,Id")] AppUser appUser)
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            if (ModelState.IsValid)
            {
                await Task.Run(() => _appUserService.CreateAsync(appUser));
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(GetAppRoles(), "RoleName", "RoleName", appUser.Role);
            ViewData["AllowMultipleRoles"] = _securityOptions.AllowMultipleRoles;
            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            if (id == null)
                return NotFound();

            var appUser = await Task.Run(() => _appUserService.FindAsync(id));
            if (appUser == null)
                return NotFound();

            ViewData["Role"] = new SelectList(GetAppRoles(), "RoleName", "RoleName", appUser.Role);
            ViewData["AllowMultipleRoles"] = _securityOptions.AllowMultipleRoles;
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserName,Role,Id,SysGuid,SysUser")] AppUser appUser)
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            if (id != appUser.Id)
                return NotFound();

            if (ModelState.IsValid)                
            {
                await Task.Run(() => _appUserService.UpdateAsync(appUser, id));
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(GetAppRoles(), "RoleName", "RoleName", appUser.Role );
            ViewData["AllowMultipleRoles"] = _securityOptions.AllowMultipleRoles;
            return View(appUser);
        }

        // GET: AppUsers/DeleteAsync/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id);
        }

        // POST: AppUsers/DeleteAsync/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //workaround -- Authorization with Roles or Policy, not working 
            if (!HttpContext.User.IsInRole("IT") && !HttpContext.User.IsInRole("admin"))
                return new StatusCodeResult(403);

            await Task.Run(() => _appUserService.DeleteAsync(id));

            return RedirectToAction(nameof(Index));
        }

    }
}
