namespace final;

using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;




public partial class Yapilacaklar : ContentPage
{
    public ObservableCollection<TodoItem> Items { get; set; }
    static FirebaseClient client = new FirebaseClient("https://yapilacaklar-cd23d-default-rtdb.firebaseio.com/");
    public Yapilacaklar()
    {
        InitializeComponent();
        Items = new ObservableCollection<TodoItem>();
        BindingContext = this;
    }
    private async void OnAddClicked(object sender, EventArgs e)
    {
        

        string name = itemEntry.Text;
        DateTime date = datePicker.Date;
        TimeSpan time = timePicker.Time;
        string detail = detailEditor.Text;

        TodoItem item = new TodoItem
        {
            Name = name,
            Date = date,
            Time = time,
            Detail = detail
        };
        Items.Add(item);

        await client.Child("yapilacaklar").PostAsync(item);
        

        itemEntry.Text = "";
        datePicker.Date = DateTime.Today;
        timePicker.Time = TimeSpan.Zero;
        detailEditor.Text = "";
    }
    public async Task<List<TodoItem>> GetYapilacaklar()
    {
        return (await client 
            .Child("yapilacaklar")
            .OnceAsync<TodoItem>()).Select(item => new TodoItem
            {
                Name = item.Object.Name,
                Date = item.Object.Date,
                Time = item.Object.Time,
                Detail = item.Object.Detail,    
                ID = item.Key
            }).ToList();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var yapilacaklarListesi = await GetYapilacaklar();
        foreach(var yapilacak in yapilacaklarListesi)
        {
            Items.Add(yapilacak);
        }
    }
    private async void Duzenle_Clicked(object sender, EventArgs e)
    {
        var yapilacak = (sender as MenuItem).CommandParameter as TodoItem;
        int index = Items.IndexOf(yapilacak);
        await Navigation.PushAsync(new Duzenle(yapilacak));
        Items[index] = yapilacak; 
    }

    private async void Sil_Clicked(object sender, EventArgs e)
    {
        var yapilacak = (sender as MenuItem).CommandParameter as TodoItem;
        await client.Child("yapilacaklar").Child(yapilacak.ID).DeleteAsync();
        Items.Remove(yapilacak);
    }
}
public class TodoItem
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Detail { get; set; }
    public string ID { get; set; }

    public string Description
    {
        get
        {
            return $"{Date:dd/MM/yyyy} {Time:hh\\:mm} - {Detail}";
        }
    }
}

