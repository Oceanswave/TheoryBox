namespace NQG.TheoryBox.Crawler
{
    using NQG.TheoryBox.DomainModel;
    using NQG.TheoryBox.Extensions;
    using OpenQA.Selenium.PhantomJS;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.Threading.Tasks;

    public class TheoryBoxCrawler
    {
        public string UserAgent
        {
            get;
            set;
        }

        public bool Start()
        {
            AsyncPump.Run(() => Initialize());
            AsyncPump.Run(() => Crawl());
            return true;
        }

        public bool Stop()
        {
            return true;
        }

        protected async void Initialize()
        {
            UserAgent = ConfigurationManager.AppSettings["UserAgent"];

            //await Repository.DeleteCardCollectionAsync();
        }

        protected async void Crawl()
        {
            using (var phantomJsService = PhantomJSDriverService.CreateDefaultService())
            {
                var phantomJsOptions = new PhantomJSOptions();
                phantomJsOptions.AddAdditionalCapability("phantomjs.page.settings.userAgent", UserAgent);
                phantomJsOptions.AddAdditionalCapability("phantomjs.page.settings.loadImages", "false");

                using (var phantomJsDriverPool = new ResourcePool<PhantomJSDriver>(pool => pool.Count > 5
                                                                                               ? null
                                                                                               : new PhantomJSDriver(phantomJsService, phantomJsOptions)))
                {
                    phantomJsDriverPool.CleanupPoolItem = driver => driver.Quit();

                    //Obtain cards
                    NonBlockingConsole.WriteLine("Retrieving all cards...", ConsoleColor.Yellow);
                    var cardInfos = GetAllCompactCardInfos(phantomJsDriverPool);

                    NonBlockingConsole.WriteLine("Retrieving card details for all cards...", ConsoleColor.Yellow);
                    foreach(var cardInfo in cardInfos)
                    {
                        var hasUpdates = false;
                        var card = await Repository.GetCard(cardInfo.MultiVerseId);

                        if (card == null)
                        {
                            card = new Card
                            {
                                MultiverseId = cardInfo.MultiVerseId,
                                Details = { Name = cardInfo.Name },
                                GathererUrl = cardInfo.AbsoluteUrl.ToString(),
                                FirstSeen = DateTime.UtcNow
                            };
                            hasUpdates = true;
                        }

                        if (card.Details == null || card.Details.LastRetrieved.HasValue == false ||
                            card.Details.LastRetrieved.Value < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)))
                        {
                            NonBlockingConsole.WriteLine(String.Format("Retrieving Details for #{0} - {1}",
                                cardInfo.MultiVerseId, cardInfo.Name), ConsoleColor.Green);

                            try
                            {
                                var cardDetails = GetCardDetails(cardInfo.MultiVerseId, phantomJsDriverPool);
                                card.Details = cardDetails;
                            }
                            catch (Exception)
                            {
                                var error = String.Format("Error retrieving #{0}", cardInfo.MultiVerseId);
                                NonBlockingConsole.WriteLine(error, ConsoleColor.Red);

                                if (card.Log == null)
                                    card.Log = new CardLog();

                                if (card.Log.Errors == null)
                                    card.Log.Errors = new List<string>();

                                card.Log.Errors.Add(error);
                            }
                            

                            hasUpdates = true;
                        }

                        //TODO: History
                        //TODO: Printing
                        //TODO: Discussion

                        if (hasUpdates)
                            await Repository.CreateOrUpdateCardAsync(card);
                    }

                    NonBlockingConsole.WriteLine("All Done!", ConsoleColor.Yellow);
                }
            }
        }

        protected static IEnumerable<CompactCardInfo> GetAllCompactCardInfos(
            ResourcePool<PhantomJSDriver> driverPool)
        {
            foreach (var cardInfo in GetCompactCardInfos(Color.Black, driverPool))
                yield return cardInfo;

            foreach (var cardInfo in GetCompactCardInfos(Color.Blue, driverPool))
                yield return cardInfo;

            foreach (var cardInfo in GetCompactCardInfos(Color.Green, driverPool))
                yield return cardInfo;

            foreach (var cardInfo in GetCompactCardInfos(Color.Red, driverPool))
                yield return cardInfo;

            foreach (var cardInfo in GetCompactCardInfos(Color.White, driverPool))
                yield return cardInfo;
        }

        protected static IEnumerable<CompactCardInfo> GetCompactCardInfos(Color color, ResourcePool<PhantomJSDriver> driverPool, int? currentPage = null, int? maxPages = null)
        {
            var isInitial = currentPage == null;

            if (isInitial)
                currentPage = 0;

            var uriBuilder =
                new UriBuilder("http://gatherer.wizards.com/Pages/Search/Default.aspx");

            var queryString = new NameValueCollection
            {
                {"output", "compact"},
                {"page", currentPage.Value.ToString(CultureInfo.InvariantCulture)}
            };

            switch (color)
            {
                case Color.Black:
                    queryString.Add("color", "|[B]");
                    break;
                case Color.Blue:
                    queryString.Add("color", "|[U]");
                    break;
                case Color.Green:
                    queryString.Add("color", "|[G]");
                    break;
                case Color.Red:
                    queryString.Add("color", "|[R]");
                    break;
                case Color.White:
                    queryString.Add("color", "|[W]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown or Unsupported card color: " + color);
            }

            uriBuilder.Query = queryString.ToQueryString(false);
            var url = uriBuilder.Uri;

            using (var driverPoolItem = driverPool.GetItem())
            {
                var driver = driverPoolItem.Resource;

                driver.Navigate().GoToUrl(url);

                if (driver.IsJQueryEnabled() == false)
                    driver.JQuerify();

                var cardInfos = driver.ExecuteScriptWithResult<IList<CompactCardInfo>>(@"
var result = [];
jQuery('div.cardList > table.compact > tbody > tr.cardItem')
  .each(function(i, value) { 

    var cardInfo = {
        name: '',
        url: '',
    };

    var nameAnchor = jQuery(value).find('td.name > a').first();
    cardInfo.name = nameAnchor.text();
    cardInfo.url = nameAnchor.attr('href');

    result.push(cardInfo);
  });
return result;
");

                //If maxPages hasn't been specified, get it from the paging controls.
                if (maxPages == null)
                {
                    string lastPageUrl = driver.ExecuteScriptWithResult(@"
var result = jQuery('div.contentcontainer > div.smallGreyBorder > div.paging.bottom > div.pagingcontrols > a').last().attr('href');
return result;
");
                    var page = lastPageUrl.Substring(lastPageUrl.IndexOf('?') + 1).GetUrlEncodedKey("page");
                    int maxPageNumber;
                    if (int.TryParse(page, out maxPageNumber))
                        maxPages = maxPageNumber;
                    else
                        throw new InvalidOperationException("Unable to retrieve the max page number.");
                }

                foreach (var cardInfo in cardInfos)
                    cardInfo.BaseUri = url;

                foreach (var cardInfo in cardInfos)
                        yield return cardInfo;

                if (!isInitial)
                {
                    yield break;
                }

                for (var i = 1; i < maxPages + 1; i++)
                {
                    var pageCardInfos = GetCompactCardInfos(color, driverPool, i, maxPages);
                    foreach (var cardInfo in pageCardInfos)
                        yield return cardInfo;
                }
            }
        }

        protected static CardDetails GetCardDetails(string metaverseId, ResourcePool<PhantomJSDriver> driverPool)
        {
            var uriBuilder =
                new UriBuilder("http://gatherer.wizards.com/Pages/Card/Details.aspx");

            var queryString = new NameValueCollection
            {
                {"multiverseid", metaverseId}
            };

            uriBuilder.Query = queryString.ToQueryString(false);
            var url = uriBuilder.Uri;

            using (var driverPoolItem = driverPool.GetItem())
            {
                var driver = driverPoolItem.Resource;

                driver.Navigate().GoToUrl(url);

                if (driver.IsJQueryEnabled() == false)
                    driver.JQuerify();

                var cardDetails = driver.ExecuteScriptWithResult<CardDetails>(@"
var cardDetails = {
    name: '',
    manaCost: [],
    convertedManaCost: null,
    types: [],
    text: '',
    flavorText: '',
    set: '',
    rarity: '',
    artist: ''
};

cardDetails.name = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(0)').text().trim();
cardDetails.manaCost = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(1) > img').map(function(i, v) { return jQuery(v).attr('alt'); });
cardDetails.convertedManaCost = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(2)').text().trim();
cardDetails.types = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(3)').text().trim().split(',');
cardDetails.text = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(4)').text().trim();
cardDetails.flavorText = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(5)').text().trim();
cardDetails.set = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(6)').text().trim();
cardDetails.rarity = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(7)').text().trim();
cardDetails.artist = jQuery('tr > td.rightCol > div.smallGreyMono > div.row > div.value:eq(8)').text().trim();
return cardDetails;
");
                cardDetails.OriginalUrl = url.ToString();
                cardDetails.LastRetrieved = DateTime.UtcNow;

                return cardDetails;
            }
        }
    }
}
