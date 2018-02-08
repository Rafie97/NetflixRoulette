using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Netflix
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MoviesPage : ContentPage
	{
        private MovieService _service = new MovieService();

        // IsSearching must be implemented as a bindable property
        private BindableProperty IsSearchingProperty =
            BindableProperty.Create("IsSearching", typeof(bool), typeof(MoviesPage), false);
        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        public MoviesPage()
        {
            BindingContext = this;

            InitializeComponent();
        }

        async void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //In case of cancel which sets NewTextValue to null
            if (e.NewTextValue == null || e.NewTextValue.Length < MovieService.MinSearchLength)
                return;

            await FindMovies(actor: e.NewTextValue);
        }

        async Task FindMovies(string actor)
        {
            try
            {
                IsSearching = true;

                var movies = await _service.FindMoviesByActor(actor);
                moviesListView.ItemsSource = movies;
                moviesListView.IsVisible = movies.Any();
                notFound.IsVisible = !moviesListView.IsVisible;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not retrieve the list of movies.", "OK"); //This is all the app displays since netflixroulette no longer works
            }
            finally
            {
                IsSearching = false;
            }
        }

        async void OnMovieSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var movie = e.SelectedItem as Movie;

            moviesListView.SelectedItem = null;

            await Navigation.PushAsync(new MovieDetailsPage(movie));
        }
    }
}