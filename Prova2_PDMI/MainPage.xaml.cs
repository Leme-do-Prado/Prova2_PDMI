using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace Prova2_PDMI;

public partial class MainPage : ContentPage
{
    private SqliteConnection database;
    private ObservableCollection<Livro> livros;

    public MainPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "bdlivros.db3");
        database = new SqliteConnection($"Data Source={dbPath}");
        database.Open();

        using (var command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Livro (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nome TEXT, NomeAutor TEXT, EmailAutor TEXT, ISBN TEXT)", database))
        {
            command.ExecuteNonQuery();
        }

        livros = new ObservableCollection<Livro>(CarregarLivros());
        LivrosListView.ItemsSource = livros;
    }

    private void Salvar_Clicked(object sender, EventArgs e)
    {
        Livro novoLivro = new Livro
        {
            Nome = NomeLivroEntry.Text,
            NomeAutor = NomeAutorEntry.Text,
            EmailAutor = EmailAutorEntry.Text,
            ISBN = ISBNEntry.Text
        };

        using (var command = new SqliteCommand("INSERT INTO Livro (Nome, NomeAutor, EmailAutor, ISBN) VALUES (@Nome, @NomeAutor, @EmailAutor, @ISBN)", database))
        {
            command.Parameters.AddWithValue("@Nome", novoLivro.Nome);
            command.Parameters.AddWithValue("@NomeAutor", novoLivro.NomeAutor);
            command.Parameters.AddWithValue("@EmailAutor", novoLivro.EmailAutor);
            command.Parameters.AddWithValue("@ISBN", novoLivro.ISBN);

            command.ExecuteNonQuery();
        }

        livros.Add(novoLivro);
    }

    private void Listar_Clicked(object sender, EventArgs e)
    {
        livros.Clear();
        foreach (var livro in CarregarLivros())
        {
            livros.Add(livro);
        }
    }

    private async void MostrarLocalizacao_Clicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                string coordenadas = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}";
                await DisplayAlert("Localização Atual", coordenadas, "OK");
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível obter a localização atual.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter a localização: {ex.Message}");
        }
    }

    private IEnumerable<Livro> CarregarLivros()
    {
        List<Livro> livrosCarregados = new List<Livro>();

        using (var command = new SqliteCommand("SELECT * FROM Livro", database))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Livro livro = new Livro
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        NomeAutor = Convert.ToString(reader["NomeAutor"]),
                        EmailAutor = Convert.ToString(reader["EmailAutor"]),
                        ISBN = Convert.ToString(reader["ISBN"])
                    };

                    livrosCarregados.Add(livro);
                }
            }
        }

        return livrosCarregados;
    }
}

