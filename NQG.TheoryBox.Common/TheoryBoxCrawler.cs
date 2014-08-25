namespace NQG.TheoryBox
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using NQG.TheoryBox.DomainModel;
    using NQG.TheoryBox.Extensions;
    using OpenQA.Selenium.PhantomJS;

    public sealed class TheoryBoxCrawler
    {
        public string UserAgent
        {
            get;
            set;
        }

        public async void Initialize()
        {
            UserAgent = ConfigurationManager.AppSettings["UserAgent"];

            //await Repository.DeleteCardCollectionAsync();
        }

        public async void Crawl()
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
                    foreach (var cardInfo in cardInfos)
                    {
                        var hasUpdates = false;
                        var card = await Repository.GetCard(cardInfo.MultiVerseId);

                        if (card == null)
                        {
                            card = new Card
                            {
                                MultiverseId = cardInfo.MultiVerseId,
                                Details = { Name = cardInfo.Name },
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
                                var error = String.Format("Error retrieving #{0} from {1}", cardInfo.MultiVerseId, cardInfo.AbsoluteUrl);
                                NonBlockingConsole.WriteLine(error, ConsoleColor.Red);

                                var t = Repository.GetLog(cardInfo.MultiVerseId);
                                t.Wait();

                                var cardLog = t.Result ?? new CardLog
                                {
                                    MultiverseId = cardInfo.MultiVerseId
                                };

                                if (cardLog.Errors == null)
                                    cardLog.Errors = new List<string>();

                                cardLog.Errors.Add(error);

                                var t1 = Repository.CreateOrUpdateCardLogAsync(cardLog);
                                t1.Wait();
                                continue;
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

        private static IEnumerable<CompactCardInfo> GetAllCompactCardInfos(
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

        private static IEnumerable<CompactCardInfo> GetCompactCardInfos(Color color, ResourcePool<PhantomJSDriver> driverPool, int? currentPage = null, int? maxPages = null)
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

        private static CardDetails GetCardDetails(string metaverseId, ResourcePool<PhantomJSDriver> driverPool)
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
    pt: {
        p: null,
        t: null
    },
    rarity: '',
    artist: ''
};

cardDetails.name = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_nameRow > .value').text().trim();
cardDetails.manaCost = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_manaRow > .value > img').map(function(i, v) { return jQuery(v).attr('alt'); });
cardDetails.convertedManaCost = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cmcRow > .value').text().trim();
if (cardDetails.convertedManaCost != null)
    cardDetails.convertedManaCost = parseFloat(cardDetails.convertedManaCost);

cardDetails.types = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_typeRow > .value').text().trim().split(',');

cardDetails.text = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_textRow > .value > .cardtextbox').map(function(i, v) { return jQuery(v).html(); });
if (typeof cardDetails.text == 'object' || typeof cardDetails.text == 'array')
    cardDetails.text = jQuery.makeArray(cardDetails.text).join('\n');

cardDetails.flavorText = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_flavorRow > .value > .cardtextbox').map(function(i, v) { return jQuery(v).html(); });
if (typeof cardDetails.flavorText == 'object' || typeof cardDetails.flavorText == 'array')
    cardDetails.flavorText = jQuery.makeArray(cardDetails.flavorText).join('\n');

cardDetails.set = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_setRow > .value').text().trim();
cardDetails.rarity = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_rarityRow > .value').text().trim();
cardDetails.cardNumber = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_numberRow > .value').text().trim();
cardDetails.artist = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_artistRow > .value').text().trim();

cardDetails.imageUrl = jQuery('tr > td.leftCol > div > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cardImage').first().attr('src');

var pt = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ptRow > .value').text().trim().split('/');
if (pt.length == 2) {
    cardDetails.pt.p = pt[0].trim();
    cardDetails.pt.t = pt[1].trim();
} else if (pt.length == 4) {
    cardDetails.pt.p = pt[0].trim() + '/' + pt[1].trim();
    cardDetails.pt.t = pt[2].trim() + '/' + pt[3].trim();
}

cardDetails.otherSets = jQuery('tr > td.rightCol > div.smallGreyMono > #ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_otherSetsRow > .value > div > a').map(function(i, v) { 
     var result  = {};
     result.metaverseId = jQuery(v).attr('href');
     if (result.metaverseId != '')
          result.metaverseId = result.metaverseId.substring(result.metaverseId.lastIndexOf('=')+1);
     result.set = jQuery(v).find('img').attr('alt');
     return result;
});

cardDetails.rulings = jQuery('#ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_rulingsRow .post').map(function(i, v) { 
     var result  = {};
     result.rulingDate = jQuery(v).find('td:eq(0)').text();
     result.rulingDate = new Date(result.rulingDate).toISOString();
     result.rulingText = jQuery(v).find('td:eq(1)').text().trim();
     return result;
});

return cardDetails;
");
                if (!String.IsNullOrEmpty(cardDetails.Text))
                    cardDetails.Text = cardDetails.Text.Trim();

                if (!String.IsNullOrEmpty(cardDetails.FlavorText))
                    cardDetails.FlavorText = cardDetails.FlavorText.Trim();

                cardDetails.OriginalUrl = url.ToString();
                Uri absImageUrl;
                if (Uri.TryCreate(url, cardDetails.ImageUrl, out absImageUrl))
                    cardDetails.ImageUrl = absImageUrl.ToString();

                cardDetails.LastRetrieved = DateTime.UtcNow;

                return cardDetails;
            }
        }
    }
}
