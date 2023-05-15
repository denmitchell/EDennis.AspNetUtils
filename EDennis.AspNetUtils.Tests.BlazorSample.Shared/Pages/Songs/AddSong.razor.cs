using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Shared.Pages.Songs
{
    public partial class AddSong
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

        /// <summary>
        /// An EDennis Service
        /// CrudService of <see cref="EntityFrameworkService{HitsContext, Song}"/> for
        /// performing CRUD operations on song records
        /// </summary>
        [Inject]
        public ICrudService<Song> SongService { get; set; }


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
        /// The primary key of the artist record for which the new
        /// song will be created.  This parameter is utilized by 
        /// <see cref="Radzen.DialogService"/> in <see cref="Artists.ListArtists"/>.
        /// NOTE: AddSong.razor includes ArtistId in the @page directive just in
        /// case there is a need to navigate to the AddSong page independent of
        /// the DialogService.
        /// </summary>
        [Parameter]
        public int ArtistId { get; set; }

        /// <summary>
        /// The current song record being added.
        /// This property is bound to the RadzenTemplateForm's Data attribute (Data="@song")
        /// </summary>
        protected Song song;

        /// <summary>
        /// Radzen allows entry of songs for a different artist when the ArtistId
        /// parameter isn't supplied. In this application, ArtistId is always 
        /// required.  That said, the Radzen pattern is preserved here just in 
        /// case the developer wants to make ArtistId optional.
        /// The hasArtistIdValue variable is bound to the 
        /// RadzenDropDown Disabled attribute (Disabled=@hasArtistIdValue).
        /// </summary>
        bool hasArtistIdValue;


        /// <summary>
        /// The list of artist options for the song.  In this case,
        /// there is only one artist option. This variable is bound to a 
        /// RadzenDropDown Data attribute (Data="@artistsForArtistId")
        /// </summary>
        protected IEnumerable<Artist> artistsForArtistId;


        /// <summary>
        /// Whether errors should be shown on the page.  
        /// This property is bound to the RadzenAlert's Visible attribute (Visible="@errorVisible)
        /// </summary>
        protected bool errorVisible;


        #endregion
        #region Component Event Handlers

        /// <summary>
        /// Retrieve the relevant artist and wrap it in an Artist array so that
        /// it can be used as the data source for a dropdown list.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            artistsForArtistId = new Artist[] { await ArtistService.FindAsync(ArtistId) };
        }

        /// <summary>
        /// This method is needed because the ArtistId parameter could be set after
        /// the component is initialized.  The method  ... 
        /// <list type="bullet">
        /// <item>creates a new <see cref="Song"/> instance</item>
        /// <item>sets the <see cref="hasArtistIdValue"/> variable</item>
        /// <item>sets the <see cref="Song.ArtistId"/> property value</item>
        /// </list>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            song = new Song();

            hasArtistIdValue = parameters.TryGetValue<int>("ArtistId", out var hasArtistIdResult);

            if (hasArtistIdValue)
            {
                song.ArtistId = hasArtistIdResult;
            }
            await base.SetParametersAsync(parameters);
        }


        /// <summary>
        /// Inserts a new song record using <see cref="EntityFrameworkService{TContext, TEntity}.Create(TEntity)"/>
        /// and closes the <see cref="Dialog"/>
        /// </summary>
        /// <returns></returns>
        protected void FormSubmit()
        {
            try
            {
                SongService.CreateAsync(song);
                DialogService.Close(song);
            }
            catch (Exception)
            {
                errorVisible = true;
            }
        }

        /// Closes the Radzen <see cref="Dialog"/>
        protected void CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}