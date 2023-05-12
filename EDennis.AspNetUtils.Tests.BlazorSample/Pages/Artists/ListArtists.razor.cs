using EDennis.AspNetUtils.Tests.BlazorSample.Pages.Songs;
using EDennis.AspNetUtils.Tests.BlazorSample.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Pages.Artists
{
    public partial class ListArtists
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
        /// performing CRUD operations on song records.
        /// Note: there was no need to extend CRUD service for song
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
        /// A reference to the RadzenDataGrid for artists (@ref="artistsGrid")  
        /// </summary>
        protected RadzenDataGrid<Artist> artistsGrid;

        /// <summary>
        /// A reference to the embedded RadzenDataGrid for songs (@ref="songsGrid")  
        /// </summary>
        protected RadzenDataGrid<Song> songsGrid;

        /// <summary>
        /// The artist records returned by LoadData below 
        /// This variable is bound to RadzenDataGrid's Data attribute (Data="@artists")
        /// </summary>
        protected IEnumerable<Artist> artists;

        /// <summary>
        /// The count of records across all pages, which is returned by 
        /// ArtistService.Get (see below).
        /// This variable is bound to RadzenDataGrid's Count attribute (Count="@recCount")
        /// </summary>
        protected int recCount = -1; //countType of records across all pages; only needed for paging;

        #endregion
        #region Component Event Handlers

        /// <summary>
        /// Retrieves artist records from <see cref="ArtistService"/>.  This method applies
        /// any filters (where), sorting (orderBy), and paging (skip, take) from the grid
        /// This method is bound to the RadzenDataGrid's LoadData attribute (LoadData="@LoadData")
        /// </summary>
        /// <param name="args">filters, sorting, and paging arguments</param>
        /// <returns></returns>
        public void LoadData(LoadDataArgs args)
        {
            (artists, recCount) = ArtistService.Get(
                where: args.Filter,
                orderBy: args.OrderBy,
                include: "Songs", //include associated song recs in the retrieved artist recs
                skip: args.Skip,
                take: args.Top,
                countType: CountType.Count //needed to support paging
            );
        }

        /// <summary>
        /// Creates a new artist record via <see cref="DialogService"/>
        /// and <see cref="AddArtist"/>.
        /// This method is bound to the Click event of a RadzenButton having Icon="add_circle_outline"
        /// (toward the top of ListArtists.razor)
        /// </summary>
        /// <param name="_">artist sent from MouseClick (unused here, so "_") </param>
        /// <returns></returns>
        protected async Task AddAsync(MouseEventArgs _)
        {
            await DialogService.OpenAsync<AddArtist>("New Artist", null);
            await artistsGrid.Reload();
        }

        /// <summary>
        /// Allows updating an existing artist record via <see cref="DialogService"/>
        /// and <see cref="EditArtist"/>.
        /// This method is bound to the Click event of a RadzenButton having Icon="edit"
        /// (toward the bottom of ListArtists.razor)
        /// </summary>
        /// <param name="artist">the artist record to edit</param>
        /// <param name="_">artist sent from MouseClick (unused here, so "_")</param>
        /// <returns></returns>
        protected async Task EditAsync(Artist artist, MouseEventArgs _)
        {
            var result = await DialogService.OpenAsync<EditArtist>("Edit Artist", new Dictionary<string, object> { { "Id", artist.Id } });
            if (result != null)
            {
                await artistsGrid.Reload();
            }
        }


        /// <summary>
        /// Allows updating an existing artist record via <see cref="DialogService"/>
        /// and <see cref="EditArtist"/>.
        /// This method is bound to the RadzenDataGrid's RowDoubleClick attribute/event (RowDoubleClick="@EditAsync")
        /// (toward the bottom of ListArtists.razor)
        /// </summary>
        /// <param name="args">artist sent from MouseClick, here containing the clicked artist record</param>
        /// <returns></returns>
        protected async Task EditAsync(DataGridRowMouseEventArgs<Artist> args)
            => await EditAsync(args.Data, null);


        /// <summary>
        /// Allows deleting an existing artist record via <see cref="DialogService"/>
        /// This method is bound to the Click event of a RadzenButton having Icon="delete"
        /// (toward the bottom of ListArtist.razor)
        /// </summary>
        /// <param name="artist">the artist record to delete</param>
        /// <param name="_">artist sent from MouseClick (unused here, so "_") </param>
        /// <returns></returns>
        protected async Task DeleteAsync(Artist artist, MouseEventArgs _)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = ArtistService.Delete(artist.Id);

                    if (deleteResult != null)
                    {
                        await artistsGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
#if DEBUG
                    Summary = $"{ex.GetType().Name}: {ex.Message}",
#else
                    Summary = $"Error",
#endif
                    Detail = $"Unable to delete Artist"
                });
            }
        }
    


        /// <summary>
        /// Gets the song for the target artist
        /// </summary>
        /// <param name="artist">The associated artist record</param>
        /// <returns></returns>
        protected void GetSongs(Artist artist)
        {
            //artist = song;
            var (songs, _) = SongService.Get(
                where: $"ArtistId eq {artist.Id}",
                orderBy: "ReleaseDate",
                include: "Artist",
                countType: CountType.None
            );

            if (songs != null)
            {
                artist.Songs = songs.ToList();
            }
        }

        /// <summary>
        /// Creates a song record for the given artist via
        /// the <see cref="DialogService"/> and the <see cref="AddSong"/> Blazor component/page
        /// </summary>
        /// <param name="artist">the artist record</param>
        /// <param name="_">unused argument</param>
        /// <returns></returns>
        protected async Task AddSongAsync(Artist artist, MouseEventArgs _)
        {
            await DialogService.OpenAsync<AddSong>("Add Songs", new Dictionary<string, object> { { "ArtistId", artist.Id } });
            GetSongs(artist);
            await songsGrid.Reload();
        }

        /// <summary>
        /// Allows editing a song record for the given artist via
        /// the <see cref="DialogService"/> and the <see cref="EditSong"/> Blazor component/page
        /// </summary>
        /// <param name="artist">the artist record</param>
        /// <param name="song">the song to edit</param>
        /// <returns></returns>
        protected async Task EditSongAsync(Artist artist, Song song)
        {
            await DialogService.OpenAsync<EditSong>("Edit Songs", new Dictionary<string, object> { { "ArtistId", artist.Id }, { "Id", song.Id }});
            GetSongs(artist);
            await songsGrid.Reload();
        }

        /// <summary>
        /// Allows deleting a song record for 
        /// </summary>
        /// <param name="artist">the relevant artist</param>
        /// <param name="song">the song to delete</param>
        /// <returns></returns>
        protected async Task DeleteSongAsync(Artist artist, Song song)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = SongService.Delete(song.Id);

                    GetSongs(artist);

                    if (deleteResult != null)
                    {
                        await songsGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
#if DEBUG
                    Summary = $"{ex.GetType().Name}: {ex.Message}",
#else
                    Summary = $"Error",
#endif
                    Detail = $"Unable to delete Song"
                });
            }
        }
        #endregion

    }
}