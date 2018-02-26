// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.MemoryTests
{
    public static partial class ReadOnlyMemoryTests
    {
        [Fact]
        public static void SameObjectsHaveSameHashCodes()
        {
            int[] a = { 91, 92, 93, 94, 95 };
            var left = new ReadOnlyMemory<int>(a, 2, 3);
            var right = new ReadOnlyMemory<int>(a, 2, 3);

            int[] b = { 1, 2, 3, 4, 5 };
            var different = new ReadOnlyMemory<int>(b, 2, 3);

            Assert.Equal(left.GetHashCode(), right.GetHashCode());
            Assert.NotEqual(left.GetHashCode(), different.GetHashCode());
        }

        [Fact]
        public static void HashCodeIncludesLength()
        {
            int[] a = { 91, 92, 93, 94, 95 };
            var left = new ReadOnlyMemory<int>(a, 2, 1);
            var right = new ReadOnlyMemory<int>(a, 2, 3);

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public static void HashCodeIncludesBase()
        {
            int[] a = { 91, 92, 93, 94, 95 };
            var left = new ReadOnlyMemory<int>(a, 1, 3);
            var right = new ReadOnlyMemory<int>(a, 2, 3);

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public static void HashCodesDifferentForSameContent()
        {
            var left = new ReadOnlyMemory<int>(new int[] { 0, 1, 2 }, 1, 1);
            var right = new ReadOnlyMemory<int>(new int[] { 0, 1, 2 }, 1, 1);

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public static void EmptyMemoryHashCodeNotUnified()
        {
            var left = new ReadOnlyMemory<int>(new int[0]);
            var right = new ReadOnlyMemory<int>(new int[0]);

            ReadOnlyMemory<int> memoryFromNonEmptyArrayButWithZeroLength = new Memory<int>(new int[1] { 123 }).Slice(0, 0);

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
            Assert.NotEqual(left.GetHashCode(), memoryFromNonEmptyArrayButWithZeroLength.GetHashCode());
            Assert.NotEqual(right.GetHashCode(), memoryFromNonEmptyArrayButWithZeroLength.GetHashCode());

            // Empty property hashcode is equal
            left = ReadOnlyMemory<int>.Empty;
            right = ReadOnlyMemory<int>.Empty;

            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public static void DefaultMemoryHashCode()
        {
            ReadOnlyMemory<int> memory = default;
            Assert.Equal(0, memory.GetHashCode());
            ReadOnlyMemory<int> memory2 = default;
            Assert.Equal(memory2.GetHashCode(), memory.GetHashCode());
        }


        [Fact]
        public static void MemoryFromStringHashCode()
        {
            string testString = "abcdefg";
            ReadOnlyMemory<char> memory = testString.AsReadOnlyMemory();
            Assert.Equal(testString.GetHashCode(), memory.GetHashCode());
            Assert.NotEqual(testString.GetHashCode(), memory.Slice(1).GetHashCode());
        }

        [Fact]
        public static void MemoryFromStringSubstringHashCode()
        {
            string testString = "abcdefg";
            ReadOnlyMemory<char> memory = "xyzabcdefgxyz".AsReadOnlyMemory();
            Assert.Equal(testString.GetHashCode(), memory.Slice(3, 7).GetHashCode());
            Assert.NotEqual(testString.GetHashCode(), memory.GetHashCode());

            Assert.True(testString.AsReadOnlyMemory().Equals(memory.Slice(3, 7)), "A");
            Assert.False(testString.AsReadOnlyMemory().Equals(memory), "B");
        }

        public static string[] myCities =
        {
            "Tabaret", "Tabati�re", "Taber", "Table Bay", "Table Head", "Table Head", "Tableland", "Tabor", "Tabor", "Tabusintac", "Tach�", "Tach�", "Tacheeda", "Tachie", "Tacks Beach", "Tadanac", "Tadmor", "Tadmore", "Tadoule Lake", "Tadoussac", "Taft", "Tafton", "Taggart", "Taghum", "Tagish", "Tahltan", "Tahsis", "Taillon", "Taits Beach", "Takhini", "Takhini Hotspring", "Takipy", "Takla", "Takla Landing", "Tako", "Taku", "Takysie Lake", "Talbot", "Talbot", "Talbot", "Talbot", "Talbotville Royal", "Talcville", "Tallheo", "Tallman", "Tall Pines", "Talmage", "Talon", "Talon", "Taloyoak", "Talzie", "Tamarac Estates", "Tamarack", "Tamarisk", "Tam O'Shanter", "Tam O'Shanter Ridge", "Tamworth", "Tancook Island", "Tancredia", "Tangent", "Tangier", "Tangleflags", "Tanglewood", "Tanglewood Beach", "Tanguay", "Tanguishene", "Tankville", "Tannahill", "Tanners Settlement", "Tannin", "Tanquary Camp", "Tansley", "Tansleyville", "Tantallon", "Tantallon", "Tanu", "Tapley", "Tapley", "Tapley Mills", "Tapleys", "Tapleys Mills", "Tapleytown", "Taplow", "Tappen", "Tara", "Taradale", "Taradale", "Tarantum", "Tara Siding", "Tarbert", "Tarbot", "Tarbotvale", "Targettville", "Tar Island", "Tarnopol", "Tarrtown", "Tarrys", "Tartan", "Tartan Lane", "Tartigou", "Tarzwell", "Taschereau", "Taschereau", "Taschereau-Fortier", "Ta:shiis", "Tashme", "Tashota", "Tasialuup Itillinga", "Tasialuup Sijjanga", "Tasikutaaraaluup Sitialungit", "Tasiqanngituq", "Tasirmiuviup Sijjangit", "Tasiujaq", "Tasu", "Ta Ta Creek", "Tatagwa", "Tatainaq", "Tatalrose", "Tatamagouche", "Tatamagouche Mountain", "Tate", "Tate Corners", "Tatehurst", "Tatla Lake", "Tatlayoko Lake", "Tatlock", "Tatlow", "Tatnall", "Tatogga", "Tatsfield", "Tatton", "Tattons Corner", "Taunton", "Taurus", "Tavani", "Taverna", "Tavistock", "Tawa", "Tawatinaw", "Taxis River", "Tay", "Tay", "Tay Creek", "Tay Falls", "Taylor", "Taylor", "Taylor", "Taylor Beach", "Taylor Corners", "Taylor Road", "Taylors", "Taylor's Bay", "Taylor's Head", "Taylors Head", "Taylorside", "Taylors Road", "Taylor Statten", "Taylorton", "Taylor Village", "Taylorville", "Tay Mills", "Taymouth", "Tay Settlement", "Tayside", "Tay Valley", "Tchesinkut Lake", "Tc�b�tik S�gik", "Tcorecik", "Tea Cove", "Teahans Corner", "Tea Hill", "Teakerne Arm", "Teakle", "Tebo", "Tecumseh", "Teddington", "Teeds Mill", "Teeds Mills", "Tee Lake", "Teepee", "Teepee Creek", "Tees", "Teeswater", "Teeterville", "Tehkummah", "Teko", "Telachick", "Telegraph Cove", "Telegraph Creek", "Telegraph Point", "Telfer", "Telford", "Telford", "Telfordville", "Telkwa", "Tellier", "Telly Road Crossing", "Teltaka", "Temagami", "Temagami North", "T�miscaming", "T�miscouata-sur-le-Lac", "Temiskaming Shores", "Temperance Vale", "Temperanceville", "Tempest", "Temple", "Temple", "Temple", "Temple Hill", "Templeman", "Templemead", "Templeton", "Templeton-Est", "Templeton-Ouest", "Tempo", "Tenaga", "Tenby", "Tenby Bay", "Ten Mile", "Ten Mile", "Tenmile Cabin", "Ten Mile Creek", "Tenmile House", "Ten Mile Lake", "Ten Mile Lake", "Tennant Cove", "Tennants Cove", "Tennessee", "Tennion", "Tennycape", "Tennyson", "Tent City", "Terence", "Terence Bay", "Terence Bay River", "Terminal Beach", "Terminus", "Terrace", "Terrace Bay", "Terrace Heights", "Terrace Heights", "Terrace Hill", "Terra Cotta", "Terrains de L'�v�que", "Terra Nova", "Terra Nova", "Terra Nova", "Terrasse-Bellevue", "Terrasse-Bigras", "Terrasse-Campbell", "Terrasse-Charbonneau", "Terrasse-de-Luxe", "Terrasse-des-Pins", "Terrasse-Dusseault", "Terrasse-Duvernay", "Terrasse-Legault", "Terrasse-Pr�fontaine", "Terrasse-Raymond", "Terrasse-Robillard", "Terrasse-Saint-Fran�ois", "Terrasse-Samson", "Terrasse-Th�oret", "Terrasse-Vaudreuil", "Terra View Heights", "Terre-�-Fer", "Terrebonne", "Terrenceville", "Terre Noire", "Terres-Rompues", "Territoire-Coburn", "Teslin", "Teslin Crossing", "Teslin Lake", "Teslin River", "Tesseralik", "Tessier", "Teston", "Tetachuk", "Tetagouche North", "Tetagouche North Side", "Tetagouche South", "Tetagouche South Side", "Tetana", "T�te-�-la-Baleine", "T�te-de-l'�le", "T�te Jaune", "T�te Jaune Cache", "T�treaultville", "Teulon", "Teviotdale", "Tewkesbury", "Texas", "Thabor", "Thackeray", "Thaddeus", "Thalberg", "Thalia", "Thamesford", "Thames River Siding", "Thames Road", "Thamesville", "Thaxted", "Thayendanegea", "The Annex", "The Back Settlement", "The Battery", "The Battery", "The Beaches", "The Beaches", "The Beaver Lodge", "The Block", "The Bluff", "The Bluffs", "The Boyne", "The Broads", "The Bush", "The Cache", "The Cedars", "The Cedars", "The Corners", "The Corra", "The Cottage", "The Cottages", "The Cross", "The Crossroads", "The Crutch", "The Delta", "The Depot", "Thedford", "The District of Lakeland No. 521", "The Dock", "The Donovan", "The Droke", "The Elbow", "The Falls", "The Flats", "The Flume", "The Forks", "The Fort", "The Front", "The Gib", "The Glades", "The Glebe", "The Glen", "The Glen", "The Golden Mile", "The Gore", "The Gore", "The Gorge", "The Grange", "The Grant", "The Gravels", "The Green", "The Grove", "The Groves", "The Gully", "The Gully", "The Halfway", "The Halfway", "The Hawk", "The Highlands", "The Highlands", "The Houser", "The Island", "The Junction", "The Junction", "The Keys", "The Kingsway", "The Lakehead", "The Ledge", "The Light", "Thelma", "The Lodge", "The Lookoff", "The Lots", "The Manor", "The Maples", "The Maples", "The Meadows", "The Meadows", "The Medway", "The Mines", "The Motion", "The Narrows", "The Narrows", "The Ninth", "Theodore", "Theodosia Arm", "The Outlet", "The Pas", "The Pas Airport", "The Pines", "The Points West Bay", "The P Patch", "The Range", "Theresa", "Theriault", "Theriault", "Th�riault", "The Ridge", "The Ridge", "Therien", "The Rock", "The Rollway", "The Sheds", "The Sixth", "The Slash", "Thessalon", "The Tannery", "Thetford Mines", "Thetford-Partie-Sud", "The Thicket", "The Tickles", "Thetis Island", "The Two Rivers", "The Willows", "Thibaudeau", "Thibault", "Thibeault Terrace", "Thibeauville", "Thicket Portage", "Third Lake", "Thirsk", "Thirty Mountain Church", "Thistle", "Thistle Creek", "Thistletown", "Thivierge", "Thoburn", "Thode", "Thomas", "Thomas Brook", "Thomasburg", "Thomaston Corner", "Thomasville", "Thom Bay", "Thomond", "Thompson", "Thompson", "Thompson", "Thompson", "Thompson Corner", "Thompson Hill", "Thompson Junction", "Thompson Lake", "Thompson Lake", "Thompson Landing", "Thompsons Mills", "Thompson Sound", "Thompsonville", "Thomson Hill", "Thomson Station", "Thomstown", "Thorah Beach", "Thorah Island", "Thorburn", "Thorburn Lake", "Thorburn Road", "Thordarson", "Thorel House", "Thorhild", "Thorlake", "Thor Lake", "Thornbrook", "Thornbury", "Thornby", "Thorncliff", "Thorncliffe", "Thorncliffe", "Thorncliffe", "Thorncliffe", "Thorncrest Village", "Thorndale", "Thorndale", "Thorndyke", "Thorne", "Thorne", "Thorne", "Thorne Centre", "Thorne Cove", "Thorne Lake", "Thorner", "Thornes Cove", "Thornetown", "Thornhill", "Thornhill", "Thornhill", "Thornhill", "Thornlea", "Thornloe", "Thornton", "Thornton Yard", "Thornyhurst", "Thorold", "Thorold Park", "Thorold South", "Thoroughfare", "Thorpe", "Thorsby", "Thorton Woods", "Thrasher", "Thrasher's Corners", "Three Arms", "Three Bridges", "Three Brooks", "Three Brooks", "Three Creeks", "Three Fathom Harbour", "Three Forks", "Three Hills", "Threehouse", "Three Mile Plains", "Three Mile Rock", "Three Tree Creek", "Three Valley", "Thresher Corners", "Throne", "Throoptown", "Thrope", "Thrums", "Thunder Bay", "Thunder Bay", "Thunder Beach (Baie-du-Tonnerre)", "Thunderbird", "Thunderchild", "Thunder Creek", "Thunder Hill", "Thunderhill Junction", "Thunder River", "Thurlow", "Thurlow", "Thurso", "Thurston Bay", "Thurston Harbour", "Thurstonia Park", "Thwaites", "Thwaytes", "Tibbets", "Tibbos Hill", "Tiblemont", "Ticehurst Corners", "Tichborne", "Tichfield", "Tichfield Junction", "Tickle Cove", "Tickle Harbour", "Tickle Harbour Station", "Tickle Road", "Tickles", "Ticouap�", "Tidal", "Tiddville", "Tide Head", "Tidnish", "Tidnish Bridge", "Tidnish Cross Roads", "Tieland", "Tiffin", "Tiffin", "Tiger", "Tiger Hills", "Tiger Lily", "Tignish", "Tignish Corner", "Tignish Shore", "Tiili Landing", "Tiilis", "Tiilis Landing", "Tika", "Tilbury", "Tilbury", "Tilbury Centre", "Tilbury Dock", "Tilbury Station", "Tilden Lake", "Tilley", "Tilley", "Tilley", "Tilley Road", "Tillicum", "Tillsonburg", "Tillsonburg Junction", "Tilly", "Tilney", "Tilsonburg", "Tilston", "Tilt Cove", "Tilt Cove", "Tilting", "Tilton", "Tilts", "Timagami", "Timagami Lodge", "Timber Bay", "Timberlea", "Timberlea Trail", "Timberlost", "Timber River", "Timberton", "Timbrell", "Timeu", "Timmijuuvinirtalik", "Timmins", "Timmins-Porcupine", "Tims Harbour", "Tincap", "Tinchebray", "Tingwick", "Tinker", "Tintagel", "Tintern", "Tin Town", "Tiny", "Tioga", "Tionaga", "Tipaskan", "Tipella", "Tipitu Pachistuwakan", "Tipperary", "Tisdale", "Tisdall", "Titanic", "Tittle Road", "Titus", "Titus Mills", "Titusville", "Tiverton", "Tiverton", "Tl'aaniiwa'a", "Tlakmaqis", "Tlell", "Tl'isnachis", "Tl'itsnit", "Tloohat-a", "Toad River", "Toanche", "Toba", "Tobacco Lake", "Tobermory", "Tobey", "Tobiano", "Tobin Lake", "Tobique Narrows", "Tobique River", "Toby Creek", "Tochty", "Tod", "Todaro", "Tod Creek", "Todds Island", "Tod Inlet", "Todmorden", "Tofield", "Tofino", "Togo", "Toimela", "Tokay", "Toketic", "Toledo", "Tolland", "Tollendal", "Tollgate", "Tolman", "Tolmie", "Tolmies Corners", "Tolsmaville", "Tolsta", "Tolstad", "Tolstoi", "Tomahawk", "Tomahawk Lake", "Tomawapocokanan", "Tom Cod", "Tomelin Bluffs", "Tomifobia", "Tomiko", "Tomkinsville", "Tomkinville", "Tom Longboat Corners", "Tompkins", "Tom-Rule's Ground", "Tomslake", "Toms Savannah", "Tomstown", "Tondern", "Toney Mills", "Toney River", "Toniata", "Tonkas", "Tonkin", "Toogood Arm", "Tooleton", "Tootinaowaziibeeng", "Topcliff", "Tophet", "Topland", "Topley", "Topley Landing", "Topping", "Topsail", "Torbay", "Torbay", "Tor Bay", "Torbrook", "Torbrook East", "Torbrook Mines", "Torbrook West", "Torch River", "Torlea", "Tormore", "Tornea", "Toronto", "Toronto", "Toronto Junction", "Torquay", "Torrance", "Torrent", "Torrington", "Torryburn", "Tory Hill", "Toslow", "Tothill", "Totnes", "Totogon", "Tottenham", "Totzke", "Touchwood", "Touraine", "Tour-Bois-Blanc", "Tour-Boissinot", "Tour-Butney", "Tour-Chicotte", "Tour-de-Jupiter", "Tour-de-la-Rivi�re-�-l'Huile", "Tour-des-Hauteurs", "Tour-des-Lacs-George", "Tour-du-Cinquante-Milles", "Tour-du-Lac-Orignal", "Tour-du-Nord", "Tourelle", "Tour-Fraser", "Tour-Galienne", "Tour-Maher", "Tourond", "Tour-Patap�dia", "Tour-Rita", "Tour-Sept-Milles", "Tour-Tableau", "Tour-Val-Marie", "Tourville", "Toutes Aides", "Towdystan", "Tower Estates", "Tower Lake", "Tower Road", "Towers", "Tow Hill", "Town Hall", "Town Lake", "Townline", "Townsend", "Townsend", "Townsend Centre", "Toyehill", "Toyes Hill", "Tracadie", "Tracadie", "Tracadie", "Tracadie Beach", "Tracadie Camp", "Tracadie Cross", "Tracadie Road", "Tracadie-Sheila", "Tracard", "Tracey Mills", "Tracy", "Tracy", "Tracy Depot", "Tracyville", "Traders Cove", "Trafalgar", "Trafalgar", "Trafalgar", "Trafalgar Heights", "Trafford", "Trail", "Trails End", "Trait-Carr�", "Trait-Carr�", "Tralee", "Tramore", "Tramping Lake", "Tranquility", "Tranquille", "Transcona", "Transcona", "Transfiguration", "Trapper's Landing", "Trapp Road", "Travellers Rest", "Travers", "Traverse Bay", "Traverse-du-Remous", "Traverse Landing", "Traverston", "Travor Road", "Traynor", "Traytown", "Traytown", "Treadwell", "Treat", "Trecastle", "Tr�cesson", "Tree Farm", "Treelon", "Treesbank", "Treesbank Ferry", "Trefoil", "Tregarva", "Treherne", "Tremaine", "Tremaudan", "Tremblay", "Tremblay", "Tremblay", "Tremblay Settlement", "Trembleur", "Trembley", "Trembowla", "Tremont", "Trenche", "Trend Village", "Trenholm", "Trenholme", "Trentham", "Trenton", "Trenton", "Trenton Junction", "Trent River", "Trenville", "Tr�panier", "Trepassey", "Tr�s-Pr�cieux-Sang-de-Notre-Seigneur", "Tr�s-Saint-R�dempteur", "Tr�s-Saint-Sacrement", "Trevelyan", "Trevessa Beach", "Trewdale", "Triangle", "Triangle", "Tribune", "Trilby", "Tring", "Tring-Jonction", "Trinit�-des-Monts", "Trinity", "Trinity", "Trinity", "Trinity Bay North", "Trinity East", "Trinity Park", "Trinity Valley", "Trinny Cove", "Triple Bay Park", "Tripoli", "Tripp Settlement", "Triquet", "Tristram", "Triton", "Triton East", "Triton Island", "Triton West", "Triwood", "Trochu", "Trois-Lacs", "Trois-Lacs", "Trois-Pistoles", "Trois-Rives", "Trois-Rivi�res", "Trois-Rivi�res-Ouest", "Trois-Ruisseaux", "Trois-Ruisseaux", "Trois-Saumons", "Trois-Saumons-Station", "Trossachs", "Trottier", "Trou-�-Gardner", "Trou-�-P�pette", "Troup", "Trout Brook", "Trout Brook", "Trout Creek", "Trout Creek", "Trout Lake", "Trout Lake", "Trout Lake", "Trout Lake", "Trout Lake", "Trout Mills", "Trout River", "Trout River", "Trout River", "Trout Stream", "Trouty", "Trowbridge", "Troy", "Troy", "Troy", "Truax", "Trudeau", "Trudel", "Trudel", "Truemanville", "Truman", "Trump", "Trump Islands", "Truro", "Truro Heights", "Trutch", "Tryon", "Tryon Settlement", "Tsawwassen", "Tsawwassen Beach", "Ts'axq'oo-is", "Tsay Keh Dene", "Tshak Penatu Epit", "Tshiahahtunekamuk", "Tshiahkuehihat Peniauiht", "Tshinuatipish", "Tsiigehtchic", "Ts'iispoo-a", "Tuam", "Tuapaaluit", "Tubbs Corners", "Tuberose", "Tuchitua", "Tucker", "Tucks", "Tudor", "Tuffnell", "Tuft Cove", "Tufts Cove", "Tuftsville", "Tugaske", "Tugtown", "Tuktoyaktuk", "Tulameen", "Tulita", "Tullamore", "Tulliby Lake", "Tullis", "Tullochgorum", "Tullymet", "Tulsequah", "Tumbler Ridge", "Tummel", "Tunaville", "Tungsten", "Tuniit", "Tunis", "Tunnel", "Tunnel", "Tunney's Pasture", "Tunstall", "Tununuk", "Tupialuviniq", "Tupirvialuit", "Tupirviit", "Tupirvikallak", "Tupirvituqaaluttalik", "Tupirviturlik", "Tupirvivinirtalik", "Tupper", "Tupper", "Tupper Lake", "Tupperville", "Tupperville", "Turbine", "Turcot", "Turgeon", "Turgeon Station", "Turin", "Turin", "Turkey Point", "Turks Cove", "Turnberry", "Turnbull", "Turner", "Turner", "Turners", "Turner's Bight", "Turners Corners", "Turner Settlement", "Turnertown", "Turner Valley", "Turnerville", "Turnip Cove", "Turnor Lake", "Turn-Up Juniper", "Turriff", "Turtle", "Turtle Beach", "Turtle Creek", "Turtle Dam", "Turtleford", "Turtleford Junction", "Turtle Lake", "Turtle Lake South Bay", "Turtle Valley", "Tuscany", "Tusket", "Tusket Falls", "Tutela", "Tutela Heights", "Tutshi", "Tuttle", "Tuttles Hill", "Tuttusivik", "Tuurngaup Illuvininga", "Tuwanek", "Tuxedo", "Tuxedo Park", "Tuxedo Park", "Tuxford", "Tway", "Tweed", "Tweedie", "Tweedie", "Tweedie Brook", "Tweedle Place", "Tweedside", "Tweedside", "Tweedsmuir", "Tweedsmuir", "Tweedsmuir", "Tweedsmuir Village", "Twelve O'Clock Point", "Twentymile Cabin", "Twentyone Mile", "Twentysix Mile", "Twentysix Mile Landing", "Twickenham", "Twidwell Bend", "Twillingate", "Twin Brae", "Twin Butte", "Twin Butte", "Twin City", "Twin Creeks", "Twin Elm", "Twin Falls", "Twin Falls", "Twining", "Twin Islands", "Twin Lakes Beach", "Twin River", "Twin Rock Valley", "Twin Valley", "Two Brooks", "Two Creeks", "Two Creeks", "Two Guns", "Two Hills", "Two Islands", "Two Lakes", "Twomey", "Two Mile", "Two Mile Corner", "Two O'Clock", "Two Rivers", "Two Rivers", "Two Rivers", "Tyandaga", "Tye", "Tyee", "Tyndall", "Tyndall Park", "Tyndal Road", "Tynehead", "Tynemouth Creek", "Tyner", "Tyneside", "Tyne Valley", "Tyotown", "Tyranite", "Tyrconnell", "Tyrone", "Tyrone", "Tyrrell", "Tyrrell", "Tyson", "Tyvan", "Tzouhalem", "Tzuhalem"
            };

        //[Fact]
        private static void TestingOriginalPopulate()
        {
            Console.WriteLine("The highest generationA1 is {0}", GC.MaxGeneration);

            //GC.Collect();
            //GC.Collect(2);
            //var before = GC.GetTotalMemory(true);
            //Console.WriteLine("A: " + before);
            HashSet<string> citiesString = new HashSet<string>();
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < myCities.Length; k++)
                {
                    string city = myCities[k];
                    if (city.IndexOf(' ') == -1)
                    {
                        citiesString.Add(city);
                    }
                }
            }
            //Assert.Equal(2, citiesString.Count);
            //GC.Collect();
            //GC.Collect(2);
            //Assert.Equal(2, GC.GetTotalMemory(false));
            //GC.KeepAlive(citiesString);

            Console.WriteLine("GenerationA2: {0}", GC.GetGeneration(citiesString));
            Console.WriteLine("Total MemoryA2: {0}", GC.GetTotalMemory(false));
            GC.Collect(0);
            Console.WriteLine("GenerationA3: {0}", GC.GetGeneration(citiesString));
            Console.WriteLine("Total MemoryA3: {0}", GC.GetTotalMemory(false));
            GC.Collect(2);
            Console.WriteLine("GenerationA4: {0}", GC.GetGeneration(citiesString));
            Console.WriteLine("Total MemoryA4: {0}", GC.GetTotalMemory(false));
            //Assert.Equal(1, 2);
            citiesString.Clear();
        }

        [Fact]
        private static void TestingMemoryPopulate()
        {
            HashSet<ReadOnlyMemory<char>> citiesMemory = new HashSet<ReadOnlyMemory<char>>();
            for (int i = 0; i < 1; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    string city = myCities[k];
                    if (city.IndexOf(' ') == -1)
                    {
                        bool result = citiesMemory.Add(city.AsReadOnlyMemory());
                        Console.WriteLine("OK: " + citiesMemory.Count + " : " + result);
                    }
                }
            }
            string temp = "abcTaberTaborTablelandTabusintacxys";
            ReadOnlyMemory<char> tester = temp.AsReadOnlyMemory();

            Assert.False(citiesMemory.Contains(tester));
            Assert.False(citiesMemory.Contains(tester.Slice(0, 3)));
            Assert.True(citiesMemory.Contains(tester.Slice(3, 5)));
            Assert.True(citiesMemory.Contains(tester.Slice(8, 5)));
            Assert.True(citiesMemory.Contains(tester.Slice(13, 9)));
            Assert.True(citiesMemory.Contains(tester.Slice(22, 10)));
            Assert.False(citiesMemory.Contains(tester.Slice(32)));
            Assert.False(citiesMemory.Contains(tester.Slice(32, 3)));

            citiesMemory.Clear();
        }


        //[Fact]
        private static void TestingMemoryPopulate2()
        {

            Console.WriteLine("The highest generationB1 is {0}", GC.MaxGeneration);

            //GC.Collect();
            //GC.Collect(2);
            //var before = GC.GetTotalMemory(true);
            //Console.WriteLine("B: " + before);
            HashSet<ReadOnlyMemory<char>> citiesMemory = new HashSet<ReadOnlyMemory<char>>();
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < myCities.Length; k++)
                {
                    string city = myCities[k];
                    if (city.IndexOf(' ') == -1)
                    {
                        citiesMemory.Add(city.AsReadOnlyMemory());
                    }
                }
            }
            //Assert.Equal(1, citiesMemory.Count);
            //GC.Collect();
            //GC.Collect(2);
            //Assert.Equal(1, GC.GetTotalMemory(false));
            //GC.KeepAlive(citiesMemory);

            Console.WriteLine("GenerationB2: {0}", GC.GetGeneration(citiesMemory));
            Console.WriteLine("Total MemoryB2: {0}", GC.GetTotalMemory(false));
            GC.Collect(0);
            Console.WriteLine("GenerationB3: {0}", GC.GetGeneration(citiesMemory));
            Console.WriteLine("Total MemoryB3: {0}", GC.GetTotalMemory(false));
            GC.Collect(2);
            Console.WriteLine("GenerationB4: {0}", GC.GetGeneration(citiesMemory));
            Console.WriteLine("Total MemoryB4: {0}", GC.GetTotalMemory(false));
            //Assert.Equal(2, 1);

            citiesMemory.Clear();
        }
    }
}
