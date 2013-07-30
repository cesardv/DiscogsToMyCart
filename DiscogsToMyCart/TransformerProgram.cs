
namespace DiscogsToMyCart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.IO;
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
        private static List<string> SqlDataInserts { get; set; }

        private static List<string> SqlTrackInsertData { get; set; }

        /// <summary>
        /// Gets or sets the MyCart album Id to the Release Id for adding later use when filling track names
        /// </summary>
        private static List<Tuple<int, int>> AlbumId2ReleaseId { get; set; }

        /// <summary>
        /// Main entry in to the program.
        /// </summary>
        /// <param name="args">string things that you can enter but do nothing.</param>
        public static void Main(string[] args)
        {
            SetGenreArtists();

            SqlDataInserts = new List<string>();
            SqlTrackInsertData = new List<string>();

            AlbumId2ReleaseId = new List<Tuple<int, int>>();
            
            GetMysqlAlbums();
            WriteToAlbumsFile();

            PrepareTrackInserts();
            WriteTrackInsertData();

            Console.WriteLine("We're done... press any key to exit...");
            Console.ReadKey();
        }

        private static void PrepareTrackInserts()
        {
            using (var conn = new MySqlConnection("server=localhost;user=adminuser;database=discogs;port=3306;password=123adminu;default command timeout=60"))
            {
                conn.Open();
                foreach (var tuple in AlbumId2ReleaseId)
                {
                    var getTrackSql = string.Format("SELECT `position`,`title`,`duration` FROM  `releases_tracks` WHERE `release`={0};", tuple.Item2);
                    var sqlcmd = new MySqlCommand(getTrackSql, conn);

                    var rdr = sqlcmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        var trackNumber = rdr[0];
                        var trackname = rdr[1];
                        var trkDuration = rdr[3];
                        SqlTrackInsertData.Add(string.Format("INSERT INTO `album_meta` (trackname, tracklen, tracknum, albumid) VALUES ('{0}',{1},{2},{3});", trackname, trkDuration ?? 0, trackNumber ?? 0, tuple.Item1));
                    }

                }

            }
        }

        private static void WriteTrackInsertData()
        {
            //generates timebased filename
            var now = DateTime.Now;
            var filepath = Environment.ExpandEnvironmentVariables("%userprofile%") + @"\Desktop\InsertTacksSQL-" + now.Year
                + now.Month + now.Day + "-" + now.Hour + now.Minute + "." + now.Millisecond + ".txt";



            File.WriteAllLines(filepath, SqlTrackInsertData);

            Console.WriteLine("It's done...\n Find the output here: {0}", filepath);
            Console.ReadKey();
        }

        private static void WriteToAlbumsFile()
        {
            //generates timebased filename
            var now = DateTime.Now;
            var filepath = Environment.ExpandEnvironmentVariables("%userprofile%") + @"\Desktop\ALBUMdata-" + now.Year
                + now.Month + now.Day + "-" + now.Hour + now.Minute + "." + now.Millisecond + ".txt";

            File.WriteAllLines(filepath, SqlDataInserts);

            Console.WriteLine("It's done...\n Find the output here: {0}", filepath);
            Console.ReadKey();
        }

        private static void GetMysqlAlbums()
        {
            using (var conn = new MySqlConnection("server=localhost;user=adminuser;database=discogs;port=3306;password=123adminu;default command timeout=240"))
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
                lists.AddRange(new List<List<string>>() { electronicArtist, hiphop, misc, jazz, rocksArtists, latin, funk, pop, reggae });
                conn.Open();
                var catindex = 1;
                foreach (var list in GenreArtists)
                {
                    // Console.WriteLine(list.Key);
                    // Console.WriteLine(GenreArtists.ElementAt(5).Key);
                    var genreArtists = list.Value;
                    var albumNo = 1;


                    foreach (var artist in genreArtists)
                    {
                        // conn.open()
                        var sql_artistId = string.Format("SELECT DISTINCT `releases`.id, `joined_artists`, `title`,uri, uri150, notes FROM  `releases` JOIN `releases_images` ON (`releases`.id=`releases_images`.release) WHERE  `joined_artists` =  '{0}' AND  `country` =  'US'", artist);
                        var cmd = new MySqlCommand(sql_artistId, conn);
                        /* 
                          SELECT `releases`.id,`joined_artists`,`title`,uri, uri150 
                          FROM  `releases` JOIN `releases_images` ON (`releases`.id=`releases_images`.release) 
                          WHERE  `joined_artists` =  '{0}' AND  `country` =  'US'                         
                         */
                        
                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            var srt = reader[0].ToString();
                            var r = new Random();
                            AlbumId2ReleaseId.Add(new Tuple<int, int>(albumNo, Convert.ToInt32(srt)));
                            SqlDataInserts.Add(string.Format("INSERT INTO `albums` (artist, name, picurl600, picurl, description, category, price, popular ) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6}, '{7}');", reader[1], reader[2], reader[3], reader[4], catindex, "9.99", r.Next(1, 6), reader[5]));
                        }

                        albumNo++;
                        reader.Close();
                    } // now all artist ID are the Keys of ArtistIdReleases
                    catindex++;
                }

            } // end of connection
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
