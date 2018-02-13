using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Web;
using System.Text;
using System.IO;
using Newtonsoft.Json;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Threading;
using System.Net;
using System.Net.Mail;

using System.Web.Script.Serialization;

namespace CryptoPriceAnalyzer1
{
    /// <summary>
    /// Scalable storage solution 
    /// </summary>
    class MegaStorage
    {
        /// <summary>
        /// Just call this function to store some data in the mega container
        /// </summary>
        public static void addPriceData(CryptoPriceData d)
        {
            LinkedList<CryptoPriceData> temp = null; ;

            //if we have at least 1 container left 
            //first check if this price is already added so we dont have to use a new container 

            foreach (var LinkedList_CryptoPriceData in Program.coinContainersUsed)
            {
                if (LinkedList_CryptoPriceData.First.Value.name == (d.name))//if a container has already been created for the price 
                {
                    temp = LinkedList_CryptoPriceData;
                    LinkedList_CryptoPriceData.AddFirst(d);
                    return;                                                          //should be unique 
                }
            }
            if (temp == null) //if the price has not been added yet 
            {

                if (Program.coinContainersLeft.Count == 0)
                {
                    return; //were out of data storage needs scaling!
                }
                else
                {
                    //add the new price to the new container 
                    temp = Program.coinContainersLeft.Pop();
                    temp.AddFirst(d);
                    Program.coinContainersUsed.Push(temp);
                }

            }
        }

        /// <summary>
        /// Returns the data set for the given crypto
        /// </summary>
        /// <param name="crypto_name">name of the cryptocurrency</param>
        /// <returns></returns>
        public static LinkedList<CryptoPriceData> getpriceDataSet(string crypto_name)
        {
            foreach (var LinkedList_CryptoPriceData in Program.coinContainersUsed)
            {
                if (LinkedList_CryptoPriceData.First.Value.name == (crypto_name))//if this is the container we need insert the new price into it 
                {
                    return LinkedList_CryptoPriceData;
                }
            }
            return null;
        }



    }
    /// <summary>
    /// NOT PREPARED, HAVE TO BE PREPARED
    /// </summary>
    class CryptoPriceData
    {
        //what crypto is this?
        public string name;
        //what the price is at this timestamp 
        public double price;
        //the time this price was recorded 
        public DateTime timestamp;
        //check this value to know if this instance is initialized or not 
        public bool init = false;
        //how long should we wait before checking the price again (in seconds) eg if 
        public int interval;
        //hourly price change -+
        public double hourly_change;
        //because notifications have to have some delay we will store the last time a notification was sent for THIS crypto currency into a static dictionary. 
        //otherwise there is no way to know exactly when a notification was sent. 
        public static Dictionary<string, DateTime> lastNotification_timestamp;
        //the notification interval
        public int notification_interval;

        /// <summary>
        /// Call this to check if the price for 
        /// </summary>
        /// <returns>returns true if the price should be updated</returns>
        public bool isPriceExpired()
        {
            if (init)
            {
                if ((DateTime.Now - timestamp).TotalSeconds > (double)interval) //if this returns true notification update can be sent 
                {
                    return true;
                }
            }
            else
            {
                return true;
            }


            return false;
        }
        /// <summary>
        /// Returns true if a push notification can be sent for this coin
        /// </summary>
        /// <returns></returns>
        public bool isNotificationTime()
        {
            if (lastNotification_timestamp.Count > 0)
            {
                if ((DateTime.Now - lastNotification_timestamp[name]).TotalSeconds > (double)notification_interval) //if this returns true notification update can be sent 
                {
                    return true;
                }
            }
            return false;
        }
    }

    class CoinExchangeDataComparer : IEqualityComparer<KeyValuePair<string, List<KeyValuePair<string, double>>>>
    {
        public bool Equals(KeyValuePair<string, List<KeyValuePair<string, double>>> b1, KeyValuePair<string, List<KeyValuePair<string, double>>> b2)
        {
            if (b2.Key == null && b1.Key == null)
                return true;
            else if (b1.Key == null | b2.Key == null)
                return false;
            else if (b1.Key == b2.Key)
                return true;
            else
                return false;
        }

        public int GetHashCode(KeyValuePair<string, List<KeyValuePair<string, double>>> bx)
        {
            int hCode = bx.Key.GetHashCode();
            return hCode;
        }
    }

    class Program
    {
        static Window1 Window = new Window1();
        System.Windows.Forms.Application a;
        

        public static LinkedList<CryptoPriceData> lastHundredBTCPrices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin1Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin2Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin3Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin4Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin5Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin6Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin7Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin8Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin9Prices = new LinkedList<CryptoPriceData>();
        public static LinkedList<CryptoPriceData> lastHundredCoin10Prices = new LinkedList<CryptoPriceData>();

        //contains intervals of time where prices should be checked 
        public static Dictionary<string, int> price_check_intervals = new Dictionary<string, int>();

        //contains intervals of time which set the max notifications that can be sent per hour 
        public static Dictionary<string, int> notification_interval = new Dictionary<string, int>();

        //contains a stack of empty! containers which hold crypto prices
        public static Stack<LinkedList<CryptoPriceData>> coinContainersLeft = new Stack<LinkedList<CryptoPriceData>>();

        //contains a stack of containers with crypto prices in them
        public static Stack<LinkedList<CryptoPriceData>> coinContainersUsed = new Stack<LinkedList<CryptoPriceData>>();

        //holds the names of the cryptos to monitor eg bitcoin, dash , monero 
        public static List<string> cryptos = new List<string>();

        //holds the urls of prices to be checked in the next hour 
        public static Stack<string> pricesToCheck = new Stack<string>();

        //holds the urls of prices checked in the past hour 
        public static Stack<string> pricesChecked = new Stack<string>();



        static List<string> urls = new List<string>();
        static string filename = @"C:\Users\Admin\Documents\NodejsWebApp1\NodejsWebApp1\filee.txt";
        static int threadFlag1 = 0;
        static monitorData sendme = new monitorData();
        static int botPause = 1;

        static void Main(string[] args)
        {
            Task.Run(() => {
                System.Windows.Forms.Application.Run(Window);
            });  


            lookForArbitrage();

            //there must be plenty of containers to store stuff in. 
            coinContainersLeft.Push(lastHundredCoin1Prices);
            coinContainersLeft.Push(lastHundredCoin2Prices);
            coinContainersLeft.Push(lastHundredCoin3Prices);
            coinContainersLeft.Push(lastHundredCoin4Prices);
            coinContainersLeft.Push(lastHundredCoin5Prices);
            coinContainersLeft.Push(lastHundredCoin6Prices);
            coinContainersLeft.Push(lastHundredCoin7Prices);
            coinContainersLeft.Push(lastHundredCoin8Prices);
            coinContainersLeft.Push(lastHundredCoin9Prices);
            coinContainersLeft.Push(lastHundredCoin10Prices);
            for (int i = 0; i < 200; i++)
            {
                coinContainersLeft.Push(new LinkedList<CryptoPriceData>());
            }

            #region init 
            cryptos.Add("internet-of-people");
            cryptos.Add("nxt");
            cryptos.Add("neo");
            cryptos.Add("monaco");
            cryptos.Add("ethereum");
            cryptos.Add("bitcoin-cash");
            cryptos.Add("groestlcoin");
            cryptos.Add("edgeless");
            cryptos.Add("voxels");
            cryptos.Add("einsteinium");
            cryptos.Add("vertcoin");
            cryptos.Add("syscoin");
            cryptos.Add("omisego");
            cryptos.Add("digibyte");
            cryptos.Add("hempcoin");
            cryptos.Add("okcash");
            cryptos.Add("storj");
            cryptos.Add("lisk");
            cryptos.Add("power-ledger");
            cryptos.Add("bitcoin-gold");
            cryptos.Add("rise");
            cryptos.Add("litecoin");
            cryptos.Add("ethereum-classic");
            cryptos.Add("ripple");
            cryptos.Add("adx-net");
            cryptos.Add("ardor");
            cryptos.Add("ark");
            cryptos.Add("nem");
            cryptos.Add("salt");
            cryptos.Add("qtum");
            cryptos.Add("status");
            cryptos.Add("dash");
            cryptos.Add("verge");
            cryptos.Add("monero");
            cryptos.Add("cofound-it");
            cryptos.Add("golem-network-tokens");
            cryptos.Add("memetic");
            cryptos.Add("nav-coin");
            cryptos.Add("waves");
            cryptos.Add("zcoin");
            cryptos.Add("civic");
            cryptos.Add("bitbay");
            cryptos.Add("augur");
            cryptos.Add("feathercoin");
            cryptos.Add("dopecoin");
            cryptos.Add("metal");
            cryptos.Add("enigma-project");
            cryptos.Add("monacoin");
            cryptos.Add("decent");
            cryptos.Add("humaniq");
            cryptos.Add("steem");
            cryptos.Add("potcoin");
            cryptos.Add("komodo");
            cryptos.Add("zcash");
            cryptos.Add("mercury");
            cryptos.Add("tokes");
            cryptos.Add("basic-attention-token");
            cryptos.Add("clubcoin");
            cryptos.Add("blocktix");
            cryptos.Add("pivx");
            cryptos.Add("triggers");
            cryptos.Add("singulardtv");
            cryptos.Add("zencash");
            cryptos.Add("library-credit");
            cryptos.Add("syndicate");
            cryptos.Add("global-currency-reserve");
            cryptos.Add("viacoin");
            cryptos.Add("elastic");
            cryptos.Add("lomocoin");
            cryptos.Add("decred");
            cryptos.Add("gamecredits");
            cryptos.Add("whitecoin");
            cryptos.Add("cannabiscoin");
            cryptos.Add("ubiq");
            cryptos.Add("viberate");
            cryptos.Add("cardano");
            cryptos.Add("firstblood");
            cryptos.Add("numeraire");
            cryptos.Add("byteball");
            cryptos.Add("dogecoin");
            cryptos.Add("factom");
            cryptos.Add("reddcoin");
            cryptos.Add("siacoin");
            cryptos.Add("synereo");
            cryptos.Add("safe-exchange-coin");
            cryptos.Add("transfercoin");
            cryptos.Add("agoras-tokens");
            cryptos.Add("circuits-of-value");
            cryptos.Add("patientory");
            cryptos.Add("peercoin");
            cryptos.Add("2give");
            cryptos.Add("monetaryunit");
            cryptos.Add("maidsafecoin");
            cryptos.Add("korecoin");
            cryptos.Add("bancor");
            cryptos.Add("stellar");
            cryptos.Add("bitsend");
            cryptos.Add("funfair");
            cryptos.Add("wings");
            cryptos.Add("mysterium");
            cryptos.Add("digixdao");
            cryptos.Add("salus");
            cryptos.Add("bitswift");
            cryptos.Add("magi");
            cryptos.Add("nexus");
            cryptos.Add("nubits");
            cryptos.Add("rlc");
            cryptos.Add("guppy");
            cryptos.Add("district0x");
            cryptos.Add("steem-dollars");
            cryptos.Add("status");
            cryptos.Add("radium");
            cryptos.Add("vericoin");
            cryptos.Add("particl");
            cryptos.Add("exclusivecoin");
            cryptos.Add("solarcoin");
            cryptos.Add("tenx");
            cryptos.Add("shift");
            cryptos.Add("eboostcoin");
            cryptos.Add("tokencard");
            cryptos.Add("xaurum");
            cryptos.Add("expanse");
            cryptos.Add("pinkcoin");
            cryptos.Add("aragon");
            cryptos.Add("blocknet");
            cryptos.Add("iocoin");
            cryptos.Add("counterparty");
            cryptos.Add("veriumreserve");
            cryptos.Add("vcash");
            cryptos.Add("parkbyte");
            cryptos.Add("foldingcoin");
            cryptos.Add("ripio-credit-network");
            cryptos.Add("zclassic");
            cryptos.Add("sibcoin");
            cryptos.Add("crown");
            cryptos.Add("qwark");
            cryptos.Add("musicoin");

            //-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------



            price_check_intervals.Add("internet-of-people", 1800);
            price_check_intervals.Add("nxt", 1800);
            price_check_intervals.Add("cardano", 1800);
            price_check_intervals.Add("neo", 1800);
            price_check_intervals.Add("monaco", 1800);
            price_check_intervals.Add("siacoin", 1800);
            price_check_intervals.Add("guppy", 1800);
            price_check_intervals.Add("ethereum", 1800);
            price_check_intervals.Add("bitcoin-cash", 1800);
            price_check_intervals.Add("groestlcoin", 1800);
            price_check_intervals.Add("edgeless", 1800);
            price_check_intervals.Add("voxels", 1800);
            price_check_intervals.Add("einsteinium", 1800);
            price_check_intervals.Add("vertcoin", 1800);
            price_check_intervals.Add("stratis", 1800);
            price_check_intervals.Add("syscoin", 1800);
            price_check_intervals.Add("omisego", 1800);
            price_check_intervals.Add("stellar", 1800);
            price_check_intervals.Add("digibyte", 1800);
            price_check_intervals.Add("hempcoin", 1800);
            price_check_intervals.Add("okcash", 1800);
            price_check_intervals.Add("storj", 1800);
            price_check_intervals.Add("lisk", 1800);
            price_check_intervals.Add("power-ledger", 1800);
            price_check_intervals.Add("bitcoin-gold", 1800);
            price_check_intervals.Add("rise", 1800);
            price_check_intervals.Add("litecoin", 1800);
            price_check_intervals.Add("ethereum-classic", 1800);
            price_check_intervals.Add("ripple", 1800);
            price_check_intervals.Add("adx-net", 1800);
            price_check_intervals.Add("ripio-credit-network", 1800);
            price_check_intervals.Add("ardor", 1800);
            price_check_intervals.Add("ark", 1800);
            price_check_intervals.Add("nem", 1800);
            price_check_intervals.Add("salt", 1800);
            price_check_intervals.Add("qtum", 1800);
            price_check_intervals.Add("status", 1800);
            price_check_intervals.Add("dash", 1800);
            price_check_intervals.Add("verge", 1800);
            price_check_intervals.Add("tenx", 1800);
            price_check_intervals.Add("monero", 1800);
            price_check_intervals.Add("cofound-it", 1800);
            price_check_intervals.Add("golem-network-tokens", 1800);
            price_check_intervals.Add("memetic", 1800);
            price_check_intervals.Add("nav-coin", 1800);
            price_check_intervals.Add("waves", 1800);
            price_check_intervals.Add("zcoin", 1800);
            price_check_intervals.Add("civic", 1800);
            price_check_intervals.Add("bitbay", 1800);
            price_check_intervals.Add("augur", 1800);
            price_check_intervals.Add("feathercoin", 1800);
            price_check_intervals.Add("dopecoin", 1800);
            price_check_intervals.Add("metal", 1800);
            price_check_intervals.Add("enigma-project", 1800);
            price_check_intervals.Add("monacoin", 1800);
            price_check_intervals.Add("decent", 1800);
            price_check_intervals.Add("humaniq", 1800);
            price_check_intervals.Add("steem", 1800);
            price_check_intervals.Add("potcoin", 1800);
            price_check_intervals.Add("komodo", 1800);
            price_check_intervals.Add("zcash", 1800);
            price_check_intervals.Add("mercury", 1800);
            price_check_intervals.Add("tokes", 1800);
            price_check_intervals.Add("basic-attention-token", 1800);
            price_check_intervals.Add("clubcoin", 1800);
            price_check_intervals.Add("blocktix", 1800);
            price_check_intervals.Add("pivx", 1800);
            price_check_intervals.Add("triggers", 1800);
            price_check_intervals.Add("singulardtv", 1800);
            price_check_intervals.Add("zencash", 1800);
            price_check_intervals.Add("library-credit", 1800);
            price_check_intervals.Add("syndicate", 1800);
            price_check_intervals.Add("global-currency-reserve", 1800);
            price_check_intervals.Add("viacoin", 1800);
            price_check_intervals.Add("elastic", 1800);
            price_check_intervals.Add("lomocoin", 1800);
            price_check_intervals.Add("decred", 1800);
            price_check_intervals.Add("gamecredits", 1800);
            price_check_intervals.Add("whitecoin", 1800);
            price_check_intervals.Add("cannabiscoin", 1800);
            price_check_intervals.Add("ubiq", 1800);
            price_check_intervals.Add("viberate", 1800);
            price_check_intervals.Add("firstblood", 1800);
            price_check_intervals.Add("numeraire", 1800);
            price_check_intervals.Add("byteball", 1800);
            price_check_intervals.Add("dogecoin", 1800);
            price_check_intervals.Add("factom", 1800);
            price_check_intervals.Add("reddcoin", 1800);
            price_check_intervals.Add("synereo", 1800);
            price_check_intervals.Add("safe-exchange-coin", 1800);
            price_check_intervals.Add("transfercoin", 1800);
            price_check_intervals.Add("agoras-tokens", 1800);
            price_check_intervals.Add("circuits-of-value", 1800);
            price_check_intervals.Add("patientory", 1800);
            price_check_intervals.Add("peercoin", 1800);
            price_check_intervals.Add("2give", 1800);
            price_check_intervals.Add("korecoin", 1800);
            price_check_intervals.Add("monetaryunit", 1800);
            price_check_intervals.Add("maidsafecoin", 1800);
            price_check_intervals.Add("bancor", 1800);
            price_check_intervals.Add("bitsend", 1800);
            price_check_intervals.Add("funfair", 1800);
            price_check_intervals.Add("wings", 1800);
            price_check_intervals.Add("mysterium", 1800);
            price_check_intervals.Add("digixdao", 1800);
            price_check_intervals.Add("salus", 1800);
            price_check_intervals.Add("bitswift", 1800);
            price_check_intervals.Add("magi", 1800);
            price_check_intervals.Add("nexus", 1800);
            price_check_intervals.Add("nubits", 1800);
            price_check_intervals.Add("rlc", 1800);
            price_check_intervals.Add("district0x", 1800);
            price_check_intervals.Add("steem-dollars", 1800);
            price_check_intervals.Add("radium", 1800);
            price_check_intervals.Add("vericoin", 1800);
            price_check_intervals.Add("particl", 1800);
            price_check_intervals.Add("exclusivecoin", 1800);
            price_check_intervals.Add("solarcoin", 1800);
            price_check_intervals.Add("shift", 1800);
            price_check_intervals.Add("eboostcoin", 1800);
            price_check_intervals.Add("tokencard", 1800);
            price_check_intervals.Add("xaurum", 1800);
            price_check_intervals.Add("expanse", 1800);
            price_check_intervals.Add("pinkcoin", 1800);
            price_check_intervals.Add("aragon", 1800);
            price_check_intervals.Add("blocknet", 1800);
            price_check_intervals.Add("iocoin", 1800);
            price_check_intervals.Add("counterparty", 1800);
            price_check_intervals.Add("veriumreserve", 1800);
            price_check_intervals.Add("vcash", 1800);
            price_check_intervals.Add("parkbyte", 1800);
            price_check_intervals.Add("foldingcoin", 1800);
            price_check_intervals.Add("zclassic", 1800);
            price_check_intervals.Add("sibcoin", 1800);
            price_check_intervals.Add("crown", 1800);
            price_check_intervals.Add("qwark", 1800);
            price_check_intervals.Add("musicoin", 1800);

            //-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------//-----------------
            //this is not used for anything. 
            notification_interval.Add("bitcoin", 300);
            notification_interval.Add("ethereum", 300);
            notification_interval.Add("dash", 300);
            notification_interval.Add("bitbean", 300);
            notification_interval.Add("nem", 300);
            notification_interval.Add("bitcoin-cash", 300);
            notification_interval.Add("monero", 300);
            notification_interval.Add("iota", 300);
            notification_interval.Add("dogecoin", 300);
            notification_interval.Add("bitcoin-gold", 400);
            notification_interval.Add("gnosis-gno", 400);
            notification_interval.Add("potcoin", 300);
            notification_interval.Add("ethereum-classic", 300);
            notification_interval.Add("zcash", 300);

            #endregion

            pnClass pn = new pnClass();
            pn.Message = "home server";
            pn.TagMsg = "tag ur it ";
            pn.token = "token";
            PushNotification pn1 = new PushNotification(pn);



            //start all our threads here
            // Thread T = new Thread(new ThreadStart(monitor), 1000000000);                //1gb stack size 
            //Thread D = new Thread(new ThreadStart(sendPushNotification), 1000000000);
            //  T.Start();
            //  D.Start();

            //get user input here
            while (true)
            {
                Console.WriteLine("welcome to the crypto bot + push notifications n stuff TM here is the menu:");
                Console.WriteLine("\nPress 1 to pause the bot.");
                int data = Console.Read();
                switch (data)
                {
                    case 1:
                        Console.WriteLine("ok pausing bot press 2 to start it again");
                        botPause = 0;
                        data = -1;
                        break;
                    case 2:
                        Console.WriteLine("starting up the bot again, monitored coins are: ");
                        data = -1;
                        foreach (var item in cryptos)
                        {
                            Console.WriteLine(item);
                        }
                        botPause = 1;
                        break;

                    default:
                        data = -1;
                        break;
                }
            }

        }


        static Dictionary<string, LinkedList<double>> DATA = new Dictionary<string, LinkedList<double>>();

        static Dictionary<string, LinkedList<string>> User_Coin_monitorInfo = new Dictionary<string, LinkedList<string>>();

        static Dictionary<string, LinkedList<dynamic>> HistoricData = new Dictionary<string, LinkedList<dynamic>>();

        #region add_remove
        //run this code like every 30 minutse to update if a user has added new coins to monitor or to remove specified coins 
        public static void add_or_remove_coins()
        {
            const string accounts_PATH = ""; // a list of account names, with each account name corresponding to a filename containing a list of coins to monitor 
                                             // const string coinsToAdd_PATH = ""; //this is already obtained from the filenames in accounts_path 
            const string coinsToRemove_PATH = ""; //a list of account names, with each name corresponding to a filename containing  a list of coins to remove. 

            /*
             file format 
             abcdefg1234=filename1*abdjfjd34332=filename2*
             * */
            IEnumerable<string> USER_ACCOUNTS = SplitStarTokenFileToStrings(accounts_PATH); //contains the names of the user accounts 


            //check for coins to add 
            List<string> allCoins = new List<string>();
            foreach (var UserAccountName in USER_ACCOUNTS)
            {
                allCoins.Clear();
                IEnumerable<string> coins = SplitStarTokenFileToStrings(UserAccountName); //read the coin names for each account and put them in a collection 
                allCoins.AddRange(coins); //add all the coins to a list does not matter if they repaet 
                foreach (var coin in allCoins)
                {
                    if (!User_Coin_monitorInfo.ContainsKey(coin))//if the coin has not been added yet create a new entry and add the current user account to it 
                    {
                        User_Coin_monitorInfo.Add(coin, new LinkedList<string>());
                        User_Coin_monitorInfo[coin].AddFirst(UserAccountName); //add the user to the list to be notified of the coin 
                    }
                    else
                    {
                        if (!User_Coin_monitorInfo[coin].Contains(UserAccountName)) //add the user account name if it does not exist 
                        {
                            User_Coin_monitorInfo[coin].AddFirst(UserAccountName); //add the user to the list to be notified of the coin 
                        }
                    }
                }
            }

            //check for coins to remove 
            USER_ACCOUNTS = SplitStarTokenFileToStrings(coinsToRemove_PATH); //contains the names of the user accounts 
            allCoins = new List<string>();
            foreach (var UserAccountName in USER_ACCOUNTS)
            {
                allCoins.Clear();
                IEnumerable<string> coins = SplitStarTokenFileToStrings(UserAccountName); //read the coin names for each account and put them in a collection 
                allCoins.AddRange(coins); //add all the coins to a list
                foreach (var coin in allCoins)
                {
                    if (User_Coin_monitorInfo.ContainsKey(coin))//if the coin is contained, remove it 
                    {
                        try
                        {
                            User_Coin_monitorInfo[coin].Remove(UserAccountName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Yeilds a collection of strings which represent either filenames or coins 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        static IEnumerable<string> SplitStarTokenFileToStrings(string filepath)
        {
            string lines = System.IO.File.ReadAllText("path");
            IEnumerable<string> splitLines = lines.Split(new char[] { '*' });
            foreach (var item in splitLines)
            {
                item.Trim(new char[] { '*' }); //remove any remaining separator characters 
            }

            return splitLines;
        }
        #endregion

        #region analyze
        /// <summary>
        /// Get all nesessary data from server in here and compare it to past data, look for aths 
        /// https://api.coinmarketcap.com/v1/ticker/
        /// </summary>
        public static void analyze_()
        {
            const string url = "https://api.coinmarketcap.com/v1/ticker/";

            while (true)
            {
                IEnumerable<string> c = User_Coin_monitorInfo.Keys.AsEnumerable(); //item eg: monero, bitconnect, vericoin 

                HttpClient theclient = new HttpClient();
                HttpResponseMessage resp;
                HttpContent cont;
                dynamic parsedJson = null;

                List<string> urls = new List<string>();
                //create a url for each crypto currency on the list to be monitored 

                while (true)
                {
                    /*
                     * Fetch Data
                     * */
                    foreach (var item in c)//get data 
                    {
                        resp = theclient.GetAsync(url + item).Result;
                        cont = resp.Content;
                        string jsonData = "";
                        jsonData = cont.ReadAsStringAsync().Result;
                        parsedJson = JsonConvert.DeserializeObject(jsonData);
                        //extract data from parsedJson and put it in containers for historic reference
                        //then perform some analysys 
                        //if analysis returns some positive results send notifications to users. 
                        //examples of analysis results: ath, large volume, big 24h growth. 

                        //                        parsedJson[i].something

                        try
                        {
                            string coinName = parsedJson[0].id;
                            if (!HistoricData.ContainsKey(coinName))//add the new coin , else just add new data 
                            {
                                HistoricData.Add(coinName, new LinkedList<dynamic>());
                                HistoricData[coinName].AddFirst(parsedJson);
                            }
                            else
                            {
                                HistoricData[coinName].AddFirst(parsedJson);
                            }
                            //take the last price data recieved and look at its 24h volume
                            if (parsedJson[0].percent_change_24h > 10.0)
                            {
                                if (User_Coin_monitorInfo.Keys.Contains((string)parsedJson[0].id)) //if the coin has been added send pn to its clients 
                                {
                                    pnClass pnClass = new pnClass();
                                    pnClass.Message = "Large Volume spike in " + parsedJson[0].id + " !";
                                    pnClass.TagMsg = "Volume Spike!";
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("error with data download!");
                        }

                    }


                    //-------------------------------
                }
            }
        }

        bool is24HVolumeGood(string coinName)
        {


            return false;
        }

        bool isATH(string coinName)
        {

            return false;
        }

        bool shouldAlert(string coinName)
        {
            return false;
        }

        void alert(string coinName)
        {

        }


        #endregion

        static Dictionary<string, List<KeyValuePair<exchanges, string>>> exchangeURLs = new Dictionary<string, List<KeyValuePair<exchanges, string>>>(); //string - coin name, exchange and url for exchange. 
        enum exchanges
        {
            GDAX =1 ,
            binance = 2,
            topia = 3,
            bittrex =4
        };
        static void addCoinMonitoringData(exchanges ex, string coinName, string url)
        {
            if (!exchangeURLs.Keys.Contains(coinName))//not present so add it to the list!
            {
                exchangeURLs.Add(coinName, new List<KeyValuePair<exchanges, string>>());
                exchangeURLs[coinName].Add(new KeyValuePair<exchanges, string>(ex, url));
                exchangeURLs[coinName] = new List<KeyValuePair<exchanges, string>>(exchangeURLs[coinName].Distinct()); // no clue if this is efficient 
            }
            else
            {
                //coin is already added
                exchangeURLs[coinName].Add(new KeyValuePair<exchanges, string>(ex, url));
                exchangeURLs[coinName] = new List<KeyValuePair<exchanges, string>>(exchangeURLs[coinName].Distinct()); // no clue if this is efficient 
            }
        }
        public static void lookForArbitrage()
        {
            DateTime now = DateTime.Now;
            
            HttpClient theclient = new HttpClient();
            HttpResponseMessage resp;
            HttpContent cont;
            string jsonData = "";

            addCoinMonitoringData( exchanges.binance, "bitcoin", "https://www.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT");
            addCoinMonitoringData(exchanges.bittrex, "bitcoin", "https://bittrex.com/api/v1.1/public/getmarketsummary?market=usdt-btc");
            addCoinMonitoringData(exchanges.topia, "bitcoin", "https://www.cryptopia.co.nz/api/GetMarket/BTC_USDT");
            addCoinMonitoringData(exchanges.GDAX, "bitcoin", "http://localhost:1337/");

            addCoinMonitoringData(exchanges.binance, "ethereum", "https://www.binance.com/api/v1/ticker/24hr?symbol=ETHUSDT");
            addCoinMonitoringData(exchanges.bittrex, "ethereum", "https://bittrex.com/api/v1.1/public/getmarketsummary?market=usdt-eth");
            addCoinMonitoringData(exchanges.topia, "ethereum", "https://www.cryptopia.co.nz/api/GetMarket/ETH_USDT");
            addCoinMonitoringData(exchanges.GDAX, "ethereum", "http://localhost:1337/");

            addCoinMonitoringData(exchanges.binance, "litecoin", "https://www.binance.com/api/v1/ticker/24hr?symbol=LTCUSDT");
            addCoinMonitoringData(exchanges.bittrex, "litecoin", "https://bittrex.com/api/v1.1/public/getmarketsummary?market=usdt-ltc");
            addCoinMonitoringData(exchanges.topia, "litecoin", "https://www.cryptopia.co.nz/api/GetMarket/LTC_USDT");
            addCoinMonitoringData(exchanges.GDAX, "litecoin", "http://localhost:1337/");
            //just get exchange data from the top ones like every 5 mins and if the diff is like >3% send pn .
            while (true)
            {
                if (DateTime.Now > now ) //want the current time to be greater than some future time. 
                {
                    #region work 
                    /*
                     https://www.cryptopia.co.nz/api/GetMarkets
                     https://bittrex.com/api/v1.1/public/getmarketsummaries
                     https://www.binance.com/api/v1/ticker/24hr

                and gdax..
                    HttpClient theclient = new HttpClient();
                    HttpResponseMessage resp;
                    HttpContent cont;

                    resp = theclient.GetAsync("http://localhost:1337/").Result;
                    cont = resp.Content;
                    string jsonData = "";
                    jsonData = cont.ReadAsStringAsync().Result;
                    Console.WriteLine(jsonData);
                     */

                    //1. Get Bitcoin 
                    Dictionary<string, List<KeyValuePair<string, double>>> coinPricesOnExchanges = new Dictionary<string, List<KeyValuePair<string, double>>>();//container for working data <COIN NAME, list<keyval<EXCHANGE NAME, PRICE AT EXCHANGE>>>
                    foreach (var item in exchangeURLs)
                    {
                        dynamic pj_binance = null;
                        dynamic pj_bttrex = null;
                        dynamic parsedJson_Topia = null;
                        string[] gdax_prices;
                        double[] gdax_pric = new double[10];

                        coinPricesOnExchanges.Add(item.Key, new List<KeyValuePair<string, double>>());
                        foreach (var key in item.Value)//per coin grab data from exchange 
                        {
                            if (key.Key == exchanges.binance)
                            {
                                pj_binance = grabExcjamgeData(key.Value);

                                double binanceBTCPrice = pj_binance != null ? pj_binance.lastPrice : 0;
                                coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("binance", binanceBTCPrice));

                            }
                            if (key.Key == exchanges.bittrex)
                            {
                                pj_bttrex = grabExcjamgeData(key.Value);
                                double bittrexBTCPrice = pj_bttrex != null ? pj_bttrex.result[0].Last : 0;
                                coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("bittrex", bittrexBTCPrice));
                            }
                            if (key.Key == exchanges.topia)
                            {
                                parsedJson_Topia = grabExcjamgeData(key.Value);
                                double topiaBTCPrice = parsedJson_Topia != null ? parsedJson_Topia.Data.LastPrice : 0;

                                coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("topia", topiaBTCPrice));
                            }
                            if (key.Key == exchanges.GDAX)
                            {
                                //gdax is special.. 
                                resp = theclient.GetAsync("http://localhost:1337/").Result;
                                cont = resp.Content;
                                jsonData = "";
                                jsonData = cont.ReadAsStringAsync().Result;
                                gdax_prices = jsonData.Split(';'); //btc ltc eth 
                                gdax_pric = new double[10];
                                int i = 0;
                                foreach (string item3 in gdax_prices)
                                {
                                    try
                                    {
                                        gdax_pric[i] = Convert.ToDouble(item3);
                                    }
                                    catch (Exception e )
                                    {
                                        if (e.GetType() != typeof(FormatException))
                                        {
                                            Console.WriteLine(e.ToString());
                                        }
                                    }
                                    i++;
                                }
                            }

                        }
                        if (item.Key == "bitcoin")
                        {
                            coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("GDAX", gdax_pric[0])); //gdax_pric[0] = gdax_pric[0];
                        }
                        if (item.Key == "litecoin")
                        {
                            coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("GDAX", gdax_pric[1])); //gdax_pric[0] = gdax_pric[0];
                        }
                        if (item.Key == "ethereum")
                        {
                            coinPricesOnExchanges[item.Key].Add(new KeyValuePair<string, double>("GDAX", gdax_pric[2])); //gdax_pric[0] = gdax_pric[0];
                        }

                        Comparison<KeyValuePair<string, double>> comparison = new Comparison<KeyValuePair<string, double>>(priceComp);
                        //sort the values
                        foreach (var item5 in coinPricesOnExchanges)
                        {
                            item5.Value.Sort(comparison);
                        }
                    }
                    //print everything and little analysis ; for each coin print the price data on exchanges 
                    IEnumerable<KeyValuePair<string, List<KeyValuePair<string, double>>>> distinct = coinPricesOnExchanges.Distinct(new CoinExchangeDataComparer());
                    //   coinPricesOnExchanges.Clear();
                    foreach (var item8 in distinct)
                    {
                        //     coinPricesOnExchanges.Add(item8.Key, item8.Value);
                    }
                    foreach (var item4 in coinPricesOnExchanges)
                    {
                        Console.WriteLine(item4.Key);//coin name 
                        Window.update1MinPrices(item4.Key);
                        foreach (var item6 in item4.Value) //foreach price on exchange x 
                        {
                            Console.WriteLine(item6.Key + "  " + item6.Value);//prices of coin on exchange

                        }
                        if (item4.Value[item4.Value.Count - 1].Value / item4.Value[0].Value < 0.973) //prices on different exchanges 
                        {
                            Console.WriteLine("" + item4.Value[item4.Value.Count - 1].Key + " and " + item4.Value[0].Key + " is " + ((1 - item4.Value[item4.Value.Count - 1].Value / item4.Value[0].Value) * 100).ToString() + "%");
                        }
                    }

                    #endregion

                    now = now.AddMinutes(1);
                }
                else
                {

                    Thread.Sleep(30000);
                    
                }

            }

        }

        static int priceComp(KeyValuePair<string ,double> a, KeyValuePair<string,double> b)
        {
            if (a.Value < b.Value)
            {
                return 1;
            }
            else if (a.Value>b.Value)
            {
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// Retrieves data from a server url must be full 
        /// </summary>
        /// <param name="url">Full url of the exchange api </param>
        /// <returns></returns>
        public static dynamic grabExcjamgeData(string url)
        {
            HttpClient theclient = new HttpClient();
            HttpResponseMessage resp = null;
            HttpContent cont;
            string jsonData = "";
            dynamic parsedJson_Topia = null;

            //EXCEPTION HANDLER 
            try
            {
                resp = theclient.GetAsync(url).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (resp != null)
            {
                cont = resp.Content;
                jsonData = "";
                jsonData = cont.ReadAsStringAsync().Result;
                //EXCEPTION HANDLER 
                try
                {
                    parsedJson_Topia = JsonConvert.DeserializeObject(jsonData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            

            return parsedJson_Topia;

            return null;
        }


        public static void monitor()
        {
            //put some dummy data to initialize stuff 
            foreach (var crypto_n in Program.cryptos)
            {
                //format our data 
                CryptoPriceData data = new CryptoPriceData
                {
                    name = crypto_n,
                    price = 0,
                    timestamp = DateTime.Now,
                    interval = Program.price_check_intervals[crypto_n],
                    init = true
                };

                MegaStorage.addPriceData(data);
            }
            int Five_minpassed = 0;
            int Thirty_minpassed = 0;
            int Hour_passed = 0;


            while (true)
            {
                HttpClient theclient = new HttpClient();
                HttpResponseMessage resp;
                HttpContent cont;
                dynamic parsedJson;

                List<string> urls = new List<string>();
                //create a url for each crypto currency on the list to be monitored 

                while (true)
                {
                    /*
                     * Fetch Data
                     * */


                    resp = theclient.GetAsync("https://bittrex.com/api/v1.1/public/getmarketsummaries").Result;
                    cont = resp.Content;
                    string jsonData = "";
                    jsonData = cont.ReadAsStringAsync().Result;
                    parsedJson = JsonConvert.DeserializeObject(jsonData);
                    int length = 0;
                    string[] splited = jsonData.Split(new string[] { "MarketName" }, 5000, StringSplitOptions.None);

                    int i = 0;
                    if (jsonData != "")
                    {
                        while (i < splited.Length - 2)
                        {
                            string data1 = parsedJson.result[i].MarketName;

                            //trim the list if its getting too long 
                            if (DATA.Count != 0)
                            {
                                if (DATA.First().Value.Count > 100)
                                {
                                    foreach (var item in DATA)
                                    {
                                        item.Value.RemoveLast();
                                    }
                                }
                            }

                            if (!DATA.ContainsKey(data1))
                            {
                                DATA.Add(data1, new LinkedList<double>());
                                
                                DATA[data1].AddFirst(Convert.ToDouble(parsedJson.result[i].Last));
                            }
                            else
                            {
                                DATA[data1].AddFirst(Convert.ToDouble(parsedJson.result[i].Last));
                            }
                            i++;
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("No internet connection");
                        return;
                    }

                    /*
                     * Format data and print it
                     * */

                    List<KeyValuePair<string, LinkedList<double>>> mylist = DATA.ToList();
                    Comparison<KeyValuePair<string, LinkedList<double>>> listt = delegate (
                        KeyValuePair<string, LinkedList<double>> a,
                      KeyValuePair<string, LinkedList<double>> b)
                    {
                        if (a.Value.First.Value < b.Value.First.Value)
                        {
                            return 1;
                        }
                        return -1;
                    };

                    //sort the list of key value pairs 
                    mylist.Sort(listt);
                    //cast the sorted list into a linked list 
                    IEnumerable<KeyValuePair<string, LinkedList<double>>> myenum = mylist.AsEnumerable();
                    //a linked list of key value pairs eg the different crypto currencies 
                    LinkedList<KeyValuePair<string, LinkedList<double>>> ll1 = new LinkedList<KeyValuePair<string, LinkedList<double>>>(myenum);
                    //points to the first crypto 
                    LinkedListNode<KeyValuePair<string, LinkedList<double>>> node1 = ll1.First;

                    //sorted list with the highest change in price at the top. 
                    List<KeyValuePair<string, double>> topChange = new List<KeyValuePair<string, double>>();

                    //update prices for the 1 min mark 
                    topChange.Clear();
                    int c = 0;
                    while (c < 10)
                    {
                        //get the biggest change in the last minute 
                        if (node1.Value.Value.Count > 1)
                        {
                            KeyValuePair<string, double> crypto = new KeyValuePair<string, double>(node1.Value.Key, 2.0 - node1.Value.Value.First.Next.Value / node1.Value.Value.First.Value);
                            topChange.Add(crypto);
                        }
                        node1 = node1.Next; //get the next crypto 
                        c++;
                    }


                    //Used to sort the list of price change %s 
                    Comparison<KeyValuePair<string, double>> changeComparator = delegate (
                       KeyValuePair<string, double> a,
                     KeyValuePair<string, double> b)
                     {
                         if (a.Value < b.Value)
                         {
                             return 1;
                         }
                         return -1;
                     };


                    topChange.Sort(changeComparator); //default should be gud 
                    topChange.Reverse();
                    Console.WriteLine("\n1 Min Mark");
                    foreach (var item in topChange)
                    {
                        Console.WriteLine(item.Key + " " + item.Value.ToString());
                    }
                    Console.Beep();

                    //  Window.update1MinPrices("stuff");
                    Five_minpassed += 0;
                    Thirty_minpassed += 0;
                    Hour_passed += 0;

                    //check if the above time intervals have passed if so display information 
                    if (Five_minpassed >= 60000 * 5)
                    {
                        Five_minpassed = 0;
                        topChange.Clear();
                        c = 0;
                        while (c < 10)
                        {
                            //get the biggest change in the last minute 
                            if (node1.Value.Value.Count > 1)
                            {
                                LinkedListNode<double> compareTo = node1.Value.Value.First;
                                for (int ri = 0; ri < 4; ri++)
                                {
                                    compareTo = compareTo.Next;
                                }
                                KeyValuePair<string, double> crypto = new KeyValuePair<string, double>(node1.Value.Key, compareTo.Value / node1.Value.Value.First.Value);
                                topChange.Add(crypto);
                            }
                            node1 = node1.Next; //get the next crypto 
                            c++;
                        }


                        topChange.Sort(changeComparator); //default should be gud 

                        Console.WriteLine("");
                        Console.WriteLine("5 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 5");
                        Console.WriteLine("5 min changes:");
                        Console.WriteLine("5 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 55 5 5 5 5 5 5 5 5");
                        foreach (var item in topChange)
                        {
                            Console.WriteLine(item.Key + " " + item.Value.ToString());
                        }

                    }

                    //check if the above time intervals have passed if so display information 
                    if (Thirty_minpassed >= 60000 * 30)
                    {
                        Thirty_minpassed = 0;
                        topChange.Clear();
                        c = 0;
                        while (c < 10)
                        {
                            //get the biggest change in the last minute 
                            if (node1.Value.Value.Count > 1)
                            {
                                LinkedListNode<double> compareTo = node1.Value.Value.First;
                                for (int ri = 0; ri < 28; ri++)
                                {
                                    compareTo = compareTo.Next;
                                }
                                KeyValuePair<string, double> crypto = new KeyValuePair<string, double>(node1.Value.Key, compareTo.Value / node1.Value.Value.First.Value);
                                topChange.Add(crypto);
                            }
                            node1 = node1.Next; //get the next crypto 
                            c++;
                        }


                        topChange.Sort(changeComparator); //default should be gud 

                        Console.WriteLine("");
                        Console.WriteLine("30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 ");
                        Console.WriteLine("30 min changes:");
                        Console.WriteLine("30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 ");
                        foreach (var item in topChange)
                        {
                            Console.WriteLine(item.Key + " " + item.Value.ToString());
                        }

                    }

                    //check if the above time intervals have passed if so display information 
                    if (Hour_passed >= 60000 * 60)
                    {
                        Hour_passed = 0;
                        topChange.Clear();
                        c = 0;
                        while (c < 10)
                        {
                            //get the biggest change in the last minute 
                            if (node1.Value.Value.Count > 1)
                            {
                                LinkedListNode<double> compareTo = node1.Value.Value.First;
                                for (int ri = 0; ri < 58; ri++)
                                {
                                    compareTo = compareTo.Next;
                                }
                                KeyValuePair<string, double> crypto = new KeyValuePair<string, double>(node1.Value.Key, compareTo.Value / node1.Value.Value.First.Value);
                                topChange.Add(crypto);
                            }
                            node1 = node1.Next; //get the next crypto 
                            c++;
                        }


                        topChange.Sort(changeComparator); //default should be gud 

                        Console.WriteLine("");
                        Console.WriteLine("60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 ");
                        Console.WriteLine("60 min changes:");
                        Console.WriteLine("60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 60 ");
                        foreach (var item in topChange)
                        {
                            Console.WriteLine(item.Key + " " + item.Value.ToString());
                        }

                    }

                    Thread.Sleep(60000 * 60);
                }
            }
        }

        public static void sendPushNotification()
        {
            while (true)
            {
                if (threadFlag1 == 1)
                {
                    /*
                    //when ready to send a notification 
                    string token = File.ReadAllText(filename);
                    pnClass p1 = new pnClass()
                    {
                        token = token,
                        Message = "master.. btc price is    " ,
                        TagMsg = "lol"
                    };
                    PushNotification pn1 = new PushNotification(p1);*/

                    Random r = new Random((int)DateTime.Now.ToBinary() % 5000);

                    const string fromPassword = "g89d7r9y54df53nk64i";
                    const string subject = "WAKE UP LOL";
                    var fromAddress = new MailAddress("gorillamaster45@gmail.com", "PRICE" + DateTime.Now.ToLocalTime().ToBinary().ToString());
                    var toAddress = new MailAddress("thisisntlongonlytomakelifehard@gmail.com", "You");

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

                    };




                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = sendme.price.ToString()
                    })
                    {
                        try
                        {
                            smtp.Send(message);
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("error too many messages have to wait now lol");
                        }

                    }



                    threadFlag1 = 0;

                }
                Thread.Sleep(5000);
            }

        }
    }


    public class monitorData
    {
        public double price;
    }


    public class pnClass
    {
        /// <summary>
        /// Message content 
        /// </summary>
        public string Message;
        /// <summary>
        /// Message title 
        /// </summary>
        public string TagMsg;
        /// <summary>
        /// Does not do anything. 
        /// </summary>
        public string token;

        /// <summary>
        /// THe id of the user account to send this data to. 
        /// </summary>
        public string deviceID;

    }

    public class PushNotification
    {
        public PushNotification(pnClass obj)
        {

            try
            {
                var applicationID = "AAAAuXWK_bk:APA91bEN3TO3_odnYnbx1aJaVebPErgaKBr-cKtZ-75xQ0Z062lxwvf_cnQQsEt-yro6N9NcMaqnDfO47CMBGVrfKywYaHfuy1O4KN9ftXzS419f_HtLf9aLG56CEaWpDx4Tab2jEyKo";

                var senderId = "796540992953";

                //string deviceId = "dh4PTbsTah0:APA91bEYPt1pcBiSK0zqOp3qoFVbDvNUMazfJyOokL0mcdseDSW1Di5i_rrwSXgUQS7hGd_qZhKioKWxEfp5moVP2uoqCesrLAWR4FWr8j2FNFuPiyahNlZk927Ua6uCB779BYC77Qh0";
                string deviceId = obj.deviceID; // "cZxyP6HdIlI:APA91bF9tbUp4xH62RSHlun2eaVLMxan5YI0Tm9cfSaKpTuBXyOqhdX0FbkX2PjTb4Sx1AyKlKxR3qXuk18ACx4eAgMAvKKdZKMPf2rXz044GRkZeR1WUIXnzHBw0xdggts0p44rfAl6";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = deviceId,

                    notification = new

                    {

                        body = obj.Message,

                        title = obj.TagMsg,

                        icon = "myicon"

                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;
                                Console.WriteLine(str);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                string str = ex.Message;

            }

        }
    }


    public class socketListener
    {
        TcpListener server = null;

        public void listen()
        {
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 1337;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }





            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

    }

}
