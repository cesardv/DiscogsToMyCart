
namespace DiscogsToMyCart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;

    using MySql.Data.MySqlClient;

    /// <summary>
    /// This program grab data from the huge MySQL database and put into our nice little schema
    /// </summary>
    public class TransformerProgram
    {

        /// <summary>
        /// Gets or sets a dictionary that returns a list of artists depending on the genre.
        /// </summary>
        public static Dictionary<string, List<string>> GenreArtists { get; set; }

        /// <summary>
        /// Get or sets the data turned into mycart-ready inserts
        /// </summary>
        private static string SqlDataInserts { get; set; }

        private static Dictionary<int, List<int> ArtistIdReleasesList { get; set; }

        /// <summary>
        /// Main entry in to the program.
        /// </summary>
        /// <param name="args">string things that you can enter but do nothing.</param>
        public static void Main(string[] args)
        {
            SetGenreArtists();

            GetMysqlAlbums();



            Console.WriteLine("We're done... press any key to exit...");
            Console.ReadKey();
        }

        private static void GetMysqlAlbums()
        {
            using (var conn = new MySqlConnection("server=localhost;user=adminuser;database=discogs;port=3306;password=123adminu;"))
            {
                var electronicArtist = GenreArtists["Electronic"];
                var hiphop = GenreArtists["Hip-Hop"];
                var misc = GenreArtists["Misc"];
                var jazz = GenreArtists["Jazz"];
                var rocksArtists = GenreArtists["Rock"];
                var latin = GenreArtists["Latin"];
                var funk = GenreArtists["Funk"];
                var pop = GenreArtists["Pop"];
                var reggae = GenreArtists["Reggae"];
                
                var lists = new ArrayList();
                lists.AddRange(new List<List<string>>() { electronicArtist, hiphop, misc, jazz, rocksArtists, latin, funk,pop, reggae });
                conn.Open();
                var catindex = 1;
                foreach (var list in GenreArtists)
                {
                    // Console.WriteLine(list.Key);
                    // Console.WriteLine(GenreArtists.ElementAt(5).Key);
                    var genreArtists = list.Value;
                    foreach (var artist in genreArtists)
                    {
                        // conn.open()
                        var sql_artistId = string.Format("SELECT id FROM `artists` WHERE name='{0}'", artist);
                        var cmd = new MySqlCommand(sql_artistId, conn);
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            ArtistIdReleasesList.Add((int)reader[0], new List<int>());
                        }

                    } // now all artist ID are the Keys of ArtistIdReleases
                    catindex++;
                }

            }
        }

        /// <summary>
        /// Creates and fills in a dictionary of genre artists. 
        /// </summary>
        private static void SetGenreArtists()
        {
            GenreArtists = new Dictionary<string, List<string>>();

            /* 1 */
            GenreArtists.Add(
                "Electronic",
                new List<string>()
                    {
                        "Daft Punk",
                        "Will.i.am",
                        "Zedd",
                        "Calvin Harris",
                        "Icona Pop",
                        "Avicii",
                        "David Guetta",
                        "Afrojack",
                        "Swedish House Mafia",
                        "Sebastian Ingrosso",
                        "Tiësto",
                        "Porter Robinson",
                        "Florence",
                        "Alesso",
                        "The Machine",
                        "Fedde le Grand"
                    });

            /* 2 */
            GenreArtists.Add(
                "Hip-Hop",
                new List<string>()
                    {
                        "Lil Wayne",
                        "Eminem",
                        "Linkin Park",
                        "Robin Thicke",
                        "Macklemore & Ryan Lewis",
                        "PSY",
                        "Jay Z",
                        "Pharrell",
                        "Akon",
                        "Pitbull",
                        "Wiz Khalifa",
                        "Master P",
                        "50 Cent",
                        "T.I.",
                        "Drake"
                    });

            /* 3 */
            GenreArtists.Add(
                "Misc",
                new List<string>()
                    {
                        "Yanni"
                    });

            /* 4 */
            GenreArtists.Add(
                "Jazz",
                new List<string>()
                    {
                        "Michael Buble",
                        "Preservation Hall Jazz Band",
                        "Miles Davis",
                        "Harry Connick, Jr.",
                        "George Duke",
                        "Natalie Cole",
                        "Dave Koz",
                        "Boney James",
                        "Dave Brubeck",
                        "Maysa",
                        "bwb",
                        "George Benson",
                        "Tony Bennett",
                    });

            /* 5 */
            GenreArtists.Add(
                "Rock",
                new List<string>()
                    {
                        "Imagine Dragons",
                        "Matt Nathanson",
                        "Sick Puppies",
                        "Skillet",
                        "Black Sabbath",
                        "Phillip Phillips",
                        "Fall Out Boy",
                        "The Lumineers",
                        "The Beatles",
                        "Jimi Hendrix",
                        "The Rolling Stones",
                        "Elvis Presley",
                        "Led Zeppelin",
                        "The Who",
                        "Eagles",
                        "Metallica",
                        "The Beach Boys",
                        "Guns N' Roses",
                        "AC/DC",
                        "Aeroslave",
                        "Nirvana",
                        "Chuck Berry",
                        "David Bowie",
                        "Queen",
                        "Nickelback",
                        "Bruce Springsteen ",
                        "Foo Fighters ",
                        "Rage Against The Machine",
                        "Shinedown",
                        "Chevelle ",
                        "Nine Inch Nails",
                        "Red Hot Chili Peppers",
                        "Van Halen"
                    });

            /* 6 */
            GenreArtists.Add(
                "Latin",
                new List<string>()
                    {
                        "Shakira",
                        "Juanes",
                        "Marc Antony",
                        "Celia Cruz",
                        "Jose Feliciano",
                        "Romeo Santos",
                        "Mana",
                        "La Ley",
                        "Soda Stereo",
                        "Jenni Rivera ",
                        "Enanitos Verdes",
                        "Ricardo Arjona",
                        "Michel Telo",
                        "Enrique Iglesias",
                        "Cristian Castro"

                    });

            /* 7 */
            GenreArtists.Add(
                "Funk",
                new List<string>()
                    {
                        "James Brown",
                        "George Clinton",
                        "Kool And The Gang",
                        "Funkadelic",
                        "Ohio Players",
                        "Bootsy Collins",
                        "Prince",
                        "Earth, Wind & Fire",
                        "Rick James",
                        "The Meters",
                        "The Bar-Kays"
                    });
            /* 8 */
            GenreArtists.Add(
                "Pop",
                new List<string>()
                    {
                        "Michael Jackson",
                        "Justin Bieber",
                        "Rihanna",
                        "Britney Spears",
                        "Lady Gaga",
                        "Katy Perry",
                        "Beyoncé",
                        "Madonna",
                        "Christina Aguilera",
                        "Pink",
                        "Justin Timberlake",
                        "Bruno Mars",
                        "Miley Cyrus",
                        "Maroon 5",
                        "Celine Dion",
                        "Jennifer Lopez",
                        "Taylor Swift",
                        "Adele",
                        "One Direction",
                        "Gotye"
                    });

            /* Reggae #9 */
            GenreArtists.Add(
                "Reggae",
                new List<string>()
                    {
                        "Bob Marley",
                        "Peter Tosh",
                        "Toots and the Maytals",
                        "Jimmy Cliff",
                        "Bunny Wailer",
                        "Black Uhuru",
                        "Sly and Robbie",
                        "Third World",
                        "Alton Ellis",
                        "Shaggy",
                    });

            /* 10 
            GenreArtists.Add(
                "Latin",
                new List<string>()
                    {
                        ""
                    });
             */
            
        } /* end of Set GenreArtists*/
    }
}
