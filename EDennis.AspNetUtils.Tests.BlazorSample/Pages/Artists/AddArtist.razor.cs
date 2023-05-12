using EDennis.AspNetUtils.Tests.BlazorSample.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Pages.Artists
{
    public partial class AddArtist
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
        /// CrudService of <see cref="EntityFrameworkService{HitsContext, Artist}"/> for
        /// performing CRUD operations on artist records
        /// </summary>
        [Inject]
        public ICrudService<Artist> ArtistService { get; set; }


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
        /// The current artist record being added.  
        /// This property is bound to the RadzenTemplateForm's Data attribute (Data="@artist")
        /// </summary>
        protected Artist artist;

        /// <summary>
        /// Whether errors should be shown on the page.  
        /// This property is bound to the RadzenAlert's Visible attribute (Visible="@errorVisible)
        /// </summary>
        protected bool errorVisible;

        #endregion
        #region Component Event Handlers


        /// <summary>
        /// Creates a new artist record 
        /// </summary>
        /// <returns></returns>
        protected override void OnInitialized()
        {
            artist = new Artist();
        }


        /// <summary>
        /// Inserts the artist record in the database or shows errors
        /// Bound to RadzenTemplateForm's Submit attribute/event
        /// </summary>
        /// <returns></returns>
        protected void FormSubmit()
        {
            try
            {
                ArtistService.Create(artist);
                DialogService.Close(artist);
            }
            catch (Exception)
            {
                errorVisible = true;
            }
        }

        /// <summary>
        /// Closes the Dialog when relevant.
        /// Bound to the Click event of a RadzenButton having Text="Cancel"
        /// (toward the bottom of the page)
        /// </summary>
        /// <param name="_">Unused</param>
        protected void CancelButtonClick(MouseEventArgs _)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}