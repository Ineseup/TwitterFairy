using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TwitterFairy.Models;
using SentimentAnalyzer;

namespace TwitterFairy.Services.Twitter
{
    public class TwitterService : ITwitterService
    {
        private int minutes = 60;
        private string maxResultsCount = "10";
        private HttpClient client = new HttpClient();

    public string CreateQuery(string[] keywords, string[] accountsFrom, string[] hashtags, string[] cashtags) 
        {
            var queryStr = "";
            var groupsOfFilters = new List<string>();
            if (hashtags.Where(h=> !string.IsNullOrEmpty(h)).ToList().Count > 0)
            {
                var hasthagsStr = string.Join(" OR ", hashtags);
                groupsOfFilters.Add($"({hasthagsStr})");
            }

            if (cashtags.Where(h => !string.IsNullOrEmpty(h)).ToList().Count > 0)
            {
                var casthagsStr = string.Join(" OR ", cashtags);
                groupsOfFilters.Add($"({casthagsStr})");
            }

            if (keywords.Where(h => !string.IsNullOrEmpty(h)).ToList().Count > 0)
            {
                var keywordsStr = string.Join(" OR ", keywords);
                groupsOfFilters.Add($"({keywordsStr})");
            }

            if (accountsFrom.Where(h => !string.IsNullOrEmpty(h)).ToList().Count > 0)
            {
                var newAccounts= accountsFrom.ToList().Select(k => string.Concat("from:", k)).ToList();
                var accountsStr = string.Join(" OR ", newAccounts);
                groupsOfFilters.Add($"({accountsStr})");
            }
            var groupsStr = string.Join(" ", groupsOfFilters);
            return queryStr + groupsStr;
        }

        public async Task<List<TwitterOutput>> GetTweets(string query) 
        {
            var maxResults = $"&max_results={maxResultsCount}&tweet.fields=created_at";
            string bearerToken = Environment.GetEnvironmentVariable("TWITTER_BEARER_TOKEN");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var postUrl = "https://api.twitter.com/2/tweets/search/recent?query=" + HttpUtility.UrlEncode(query) + maxResults;
            var post = await client.GetAsync(postUrl);
            var result= post.Content.ReadAsStringAsync().Result;
            return createTweetResponse(result);
        }
        private async Task<bool> connectToBinanceFairy(List<TwitterOutput> twitterResult)
        {
            var randomObject = new { triggers = twitterResult };
            var postUrl = "http://10.2.113.134:8886/api/trigger/check";
            var postContent = new StringContent(JsonConvert.SerializeObject(randomObject), Encoding.UTF8, "application/json");

            var post = await client.PostAsync(postUrl, postContent);
            var result = post.Content.ReadAsStringAsync().Result;

            return post.IsSuccessStatusCode;
        }

        private List<TwitterOutput> createTweetResponse(string responseStr) 
        {
            var response = new List<TwitterOutput>();
            var json = JObject.Parse(responseStr);
            var data=json["data"];
            if (data == null || data.Count() < 1)
                throw new KeyNotFoundException("No twitter data for this request");

            foreach (var item in data)
            {
                response.Add(new TwitterOutput
                {
                    TweetTime = item.Value<string>("created_at"),
                    IntervalMinutes = minutes,
                    PositiveSentiment = Sentiments.Predict(item.Value<string>("text")).Prediction,
                    TweetText = item.Value<string>("text"),
                    Coins = coinList.Where(c => item.Value<string>("text").Contains(c)).Distinct().ToArray()
                }); 
            }
            connectToBinanceFairy(response);
            return response;
        }
        private List<string> coinList = @"
Bitcoin
BTC
Ethereum
ETH
Tether
USDT
USD Coin
USDC
BNB
BNB
Binance USD
BUSD
Cardano
ADA
XRP
XRP
Solana
SOL
Dogecoin
DOGE
Dai
DAI
Polkadot
DOT
TRON
TRX
Wrapped Bitcoin
WBTC
UNUS SED LEO
LEO
SHIBA INU
SHIB
Avalanche
AVAX
Litecoin
LTC
FTX Token
FTT
Polygon
MATIC
Chainlink
LINK
Cronos
CRO
Stellar
XLM
Uniswap
UNI
Bitcoin Cash
BCH
NEAR Protocol
NEAR
Algorand
ALGO
Bitcoin BEP2
BTCB
Monero
XMR
Ethereum Classic
ETC
Cosmos
ATOM
VeChain
VET
Decentraland
MANA
Hedera
HBAR
Flow
FLOW
Helium
HNT
TrueUSD
TUSD
Internet Computer
ICP
Tezos
XTZ
Theta Network
THETA
Filecoin
FIL
Elrond
EGLD
ApeCoin
APE
KuCoin Token
KCS
Bitcoin SV
BSV
The Sandbox
SAND
Pax Dollar
USDP
EOS
EOS
Zcash
ZEC
BitTorrent
BTTOLD
Axie Infinity
AXS
Neutrino USD
USDN
Huobi Token
HT
Aave
AAVE
USDD
USDD
Maker
MKR
IOTA
MIOTA
BitTorrent (New)
BTT
eCash
XEC
The Graph
GRT
Klaytn
KLAY
OKB
OKB
PAX Gold
PAXG
Neo
NEO
Quant
QNT
Fantom
FTM
Chiliz
CHZ
THORChain
RUNE
Waves
WAVES
Basic Attention Token
BAT
Loopring
LRC
Stacks
STX
Dash
DASH
Fei USD
FEI
Zilliqa
ZIL
PancakeSwap
CAKE
Kusama
KSM
Gala
GALA
Enjin Coin
ENJ
XDC Network
XDC
Amp
AMP
Terra Classic
LUNC
Celo
CELO
STEPN
GMT
Holo
HOT
NEM
XEM
WEMIX
WEMIX
Nexo
NEXO
Kava.io
KAVA
Mina
MINA
Curve DAO Token
CRV
Decred
DCR
Kadena
KDA
Harmony
ONE
1inch
1INCH
HUSD
HUSD
GateToken
GT
Theta Fuel
TFUEL
BinaryX
BNX
Bitcoin Gold
BTG
".Split(new string[] { "\r\n", "\r", "\n" },
    StringSplitOptions.None).Where(c=>!string.IsNullOrEmpty(c)).ToList();
    }
}
