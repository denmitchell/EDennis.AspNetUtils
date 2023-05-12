using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Pages.Admin.Users
{
    /// <summary>
    /// Backing code for ListUsers.razor.
    /// Note: Radzen components are used on this page.
    /// When creating new Radzen projects, be sure to follow 
    /// https://blazor.radzen.com/get-started
    /// </summary>
    public partial class ListUsers
    {
        #region Injected Services

        /// <summary>
        /// A Radzen Service
        /// Used for pop-up forms for editing and creating records.
        /// Some online resources:
        /// <list type="bullet">
        /// <item><see href="https://blazor.radzen.com/docs/guides/getting-started/dialog.html?tabs=server-side"/></item>
        /// <item><see href="https://blazor.radzen.com/docs/api/Radzen.DialogService.html"/></item>
        /// <item><see href="https://blazor.radzen.com/dialog" /></item>
        /// <item><see href="https://blazor.radzen.com/docs/guides/components/dialog.html" /></item>
        /// </list>
        /// </summary>
        [Inject]
        protected DialogService DialogService { get; set; }

        /// <summary>
        /// A Radzen Service
        /// Used to support the Notification component.
        /// Some online resources:
        /// <list type="bullet">
        /// <item><see href="https://blazor.radzen.com/docs/guides/getting-started/notification.html?tabs=server-side"/></item>
        /// <item><see href="https://blazor.radzen.com/docs/api/Radzen.NotificationService.html"/></item>
        /// <item><see href="https://blazor.radzen.com/docs/guides/components/notification.html" /></item>
        /// </list>
        /// </summary>
        [Inject]
        protected NotificationService NotificationService { get; set; }

        /// <summary>
        /// An EDennis Service
        /// Implementation of <see cref="EntityFrameworkService{AppUserRolesContext, AppUser}"/> for
        /// performing CRUD operations on user records
        /// </summary>
        [Inject]
        protected ICrudService<AppUser> UserService { get; set; }



        #endregion
        #region Other Available Services

        ///// <summary>
        ///// A Radzen Service
        ///// Provides right-click context menus for components.
        ///// Some online resources:
        ///// <list type="bullet">
        ///// <item><see href="https://blazor.radzen.com/docs/guides/getting-started/context-menu.html?tabs=server-side"/></item>
        ///// <item><see href="https://blazor.radzen.com/docs/api/Radzen.ContextMenuService.html"/></item>
        ///// <item><see href="https://www.radzen.com/documentation/blazor/contextmenu/" /></item>
        ///// <item><see href="https://blazor.radzen.com/contextmenu" /></item>
        ///// </list>
        ///// </summary>
        //[Inject]
        //protected ContextMenuService ContextMenuService { get; set; }

        ///// <summary>
        ///// A Radzen Service
        ///// Provides tooltips for components.
        ///// Some online resources:
        ///// <list type="bullet">
        ///// <item><see href="https://blazor.radzen.com/docs/guides/getting-started/tooltip.html?tabs=server-side"/></item>
        ///// <item><see href="https://blazor.radzen.com/docs/api/Radzen.TooltipService.html"/></item>
        ///// <item><see href="https://blazor.radzen.com/docs/guides/components/tooltip.html" /></item>
        ///// <item><see href="https://blazor.radzen.com/tooltip" /></item>
        ///// </list>
        ///// </summary>
        //[Inject]
        //protected TooltipService TooltipService { get; set; }

        ///// <summary>
        ///// A Microsoft Service
        ///// Provides JavaScript Interoperability (e.g., call JavaScript from C# or vice-versa)
        ///// Some online resources:
        ///// <list type="bullet">
        ///// <item><see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-7.0"/></item>
        ///// <item><see href="https://blazor-university.com/javascript-interop/calling-javascript-from-dotnet/"/></item>
        ///// </list>
        ///// </summary>
        //[Inject]
        //protected IJSRuntime JSRuntime { get; set; }

        ///// <summary>
        ///// A Microsoft Service
        ///// Provides programmatic navigation
        ///// Some online resources:
        ///// <list type="bullet">
        ///// <item><see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-7.0"/></item>
        ///// <item><see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.navigationmanager?view=aspnetcore-7.0"/></item>
        ///// </list>
        ///// </summary>
        //[Inject]
        //protected NavigationManager NavigationManager { get; set; }

        #endregion
        #region Bound Variables

        /// <summary>
        /// A reference to the RadzenDataGrid (@ref="usersGrid")  
        /// </summary>
        protected RadzenDataGrid<AppUser> usersGrid;


        /// <summary>
        /// The user records returned by LoadDataAsync below 
        /// This variable is bound to RadzenDataGrid's Data attribute (Data="@users")
        /// </summary>
        protected IEnumerable<AppUser> users;


        /// <summary>
        /// The count of records across all pages, which is returned by 
        /// UserService.GetAsync (see below).
        /// This variable is bound to RadzenDataGrid's Count attribute (Count="@recCount")
        /// </summary>
        protected int recCount = -1;


        #endregion
        #region Component Event Handlers

        /// <summary>
        /// Retrieves user records from <see cref="UserService"/>.  This method applies
        /// any filters (where), sorting (orderBy), and paging (skip, take) from the grid
        /// This method is bound to the RadzenDataGrid's LoadData attribute (LoadData="@LoadDataAsync")
        /// </summary>
        /// <param name="args">filters, sorting, and paging arguments</param>
        /// <returns></returns>
        public async Task LoadDataAsync(LoadDataArgs args)
        {
            //Note: the return type of GetAsync is a two-variable tuple.
            //By using variable names defined above, the output is piped to
            //those two variables. 
            (users, recCount) = await UserService.GetAsync(
                where: args.Filter,
                orderBy: args.OrderBy,
                skip: args.Skip,
                take: args.Top,
                countType: CountType.Count //needed to support paging
            );

            //StateHasChanged must be called when the method is async
            //(when using LoadDataAsync, rather than LoadData)
            StateHasChanged(); 
        }

        /// <summary>
        /// Creates a new user record via <see cref="DialogService"/>
        /// and <see cref="AddUser"/>.
        /// This method is bound to the Click event of a RadzenButton having Icon="add_circle_outline"
        /// (toward the top of ListUsers.razor)
        /// </summary>
        /// <param name="_">data sent from MouseClick (unused here, so "_") </param>
        /// <returns></returns>
        protected async Task AddAsync(MouseEventArgs _)
        {
            await DialogService.OpenAsync<AddUser>("New User", null);
            await usersGrid.Reload();
        }

        /// <summary>
        /// Allows updating an existing user record via <see cref="DialogService"/>
        /// and <see cref="EditUser"/>.
        /// This method is bound to the Click event of a RadzenButton having Icon="edit"
        /// (toward the bottom of ListUsers.razor)
        /// </summary>
        /// <param name="_">data sent from MouseClick (unused here, so "_")</param>
        /// <param name="user">the user record to edit</param>
        /// <returns></returns>
        protected async Task EditAsync(AppUser user, MouseEventArgs _)
        {
            var result = await DialogService.OpenAsync<EditUser>("Edit User", new Dictionary<string, object> { { "Id", user.Id } });
            if (result != null)
            {
                await usersGrid.Reload();
            }
        }

        /// <summary>
        /// Allows updating an existing user record via <see cref="DialogService"/>
        /// and <see cref="EditUser"/>.
        /// This method is bound to the RadzenDataGrid's RowDoubleClick attribute/event (RowDoubleClick="@Edit")
        /// (toward the bottom of ListUsers.razor)
        /// </summary>
        /// <param name="args">data sent from MouseClick, here containing the clicked user record</param>
        /// <returns></returns>
        protected async Task Edit(DataGridRowMouseEventArgs<AppUser> args)
            => await EditAsync(args.Data, null);


        /// <summary>
        /// Allows deleting an existing user record via <see cref="DialogService"/>
        /// This method is bound to the Click event of a RadzenButton having Icon="delete"
        /// (toward the bottom of ListUsers.razor)
        /// </summary>
        /// <param name="user">the user record to delete</param>
        /// <param name="_">data sent from MouseClick (unused here, so "_") </param>
        /// <returns></returns>
        protected async Task DeleteAsync(AppUser user, MouseEventArgs _ )
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await UserService.DeleteAsync(user.Id);

                    if (deleteResult != null)
                    {
                        await usersGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
#if DEBUG
//Show the actual exception if compiled in DEBUG:
                    Summary = $"{ex.GetType().Name}: {ex.Message}",
#else
                    Summary = $"Error",
#endif
                    Detail = $"Unable to delete Incident"
                });
            }
        }

        #endregion
    }
}
