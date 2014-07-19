
// div id used to display topic information
var topicIds = ["topic1", "topic2", "topic3", "topic4", "topic5"];

// tweets are counted how often?
var tweetWindow = " tw/5 sec";

// highlight color and duration to use when information about a topic changes
var highlightColor = "#EC008C", backgroundColor = "#68217A", highlightDuraction = 2000;

// host and ports used for web sockets
var wsHost = "localhost";
var wsTotalPort = "8181", wsTopicPort = "8182", wsTweetPort = "8183";

var chart;

// this function executes after the DOM is ready but not yet fully loaded
$(document).ready(function () {


    Highcharts.setOptions({
        global: {
            useUTC: false
        }
    });

    /* create new HIGHCHART */
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'chart-container',
            type: 'spline',
            backgroundColor: '#00BCF2'
        },
        colors: [
            '#FF8C00', // orange
            '#FFF100', // yellow
            '#009E49', // green
            "#68217A", // purple
            '#00198F', // blue
            "#EC008C" // pink
        ],
        title: { text: null },
        xAxis: {
            type: 'datetime'
        },
        yAxis: {
            title: { text: null },
            min: 0
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.series.name + '</b><br/>' +
                Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '<br/>' +
                Highcharts.numberFormat(this.y, 2);
            }
        },
        legend: { enabled: true  },
        series: [
            {
                name: 'Total Tweets',
                data: (function () {
                    // generate an array of random data
                    var data = [],
                        time = (new Date()).getTime(),
                        i;
                    for (i = -9; i <= 0; i++) { 
                        data.push({
                            x: time + i * 1000,
                            y: 0 //Math.random()
                        });
                    }
                    return data;
                })()
            }
            ,{
                name: 'Avg Sentiment',
                data: (function () {
                    // generate an array of random data
                    var data = [],
                        time = (new Date()).getTime(),
                        i;
                    for (i = -9; i <= 0; i++) { 
                        data.push({
                            x: time + i * 1000,
                            y: 0 //Math.random()
                        });
                    }
                    return data;
                })()
            }
        ]
    });


    /* function to MAXIMIZE DETAILS WINDOW */
    $("#maximize-details-window").click(function (e) {
        e.preventDefault();

        // hide the summary window first
        $("#tweet-summary").addClass("hidden");

        $("#tweet-details").animate({ width: "100%" }, "slow", function () {
            //show reset command
            $("#reset-details-window").removeClass("hidden");

            //hide the rest of the other commands
            $("#maximize-summary-window").addClass("hidden");
            $("#maximize-details-window").addClass("hidden");
            $("#reset-summary-window").addClass("hidden");

            updateScrollBar();
        });
    });

    /* function to RESET DETAILS WINDOW */
    var tweet_details_width = $("#tweet-details").width();
    $("#reset-details-window").click(function (e) {
        e.preventDefault();

        if ($("#tweet-details").width() != tweet_details_width) {
            $("#tweet-details").animate({ width: tweet_details_width }, "slow", function () {
                //show relevant commands and window
                $("#tweet-summary").removeClass("hidden");
                $("#maximize-summary-window").removeClass("hidden");
                $("#maximize-details-window").removeClass("hidden");

                //hide the rest of the other commands
                $("#reset-summary-window").addClass("hidden");
                $("#reset-details-window").addClass("hidden");

                updateScrollBar();
            });
        }
    });


});

// this function executes after the DOM content is fully loaded
$(window).load(function () {

    /********************************************************************************/
    /* WEB SOCKETS                                                                  */
    /********************************************************************************/
    //subscribe to web sockets

    var totalSocket = new WebSocket("ws://" + wsHost + ":" + wsTotalPort);
    var topicSocket = new WebSocket("ws://" + wsHost + ":" + wsTopicPort);
    var tweetSocket = new WebSocket("ws://" + wsHost + ":" + wsTweetPort);

    totalSocket.onmessage = function (evt) {
        var dataContainer = JSON.parse(evt.data);
        //var target = $("#tweet-summary .padding");
        //target.append("<p> Total Tweet Count:" + dataContainer.TweetCount + " - Average Sentiment:" + dataContainer.AvgSentiment + "</p>");

        var tweetCountSeries = chart.series[0];
        var avgSentimentSeries = chart.series[1];
        var x = (new Date()).getTime(); // current time
        tweetCountSeries.addPoint([x, dataContainer.TweetCount], true, true);
        avgSentimentSeries.addPoint([x, dataContainer.AvgSentiment], true, true);
    };

    topicSocket.onmessage = function (evt) {
        var dataContainer = JSON.parse(evt.data);
        addTopic(dataContainer.Topic, dataContainer.TweetCount, dataContainer.AvgSentiment.toFixed(1));
    };

    tweetSocket.onmessage = function (evt) {
        var dataContainer = JSON.parse(evt.data);
        addTweets(dataContainer.ProfileImageUrl, dataContainer.Text, dataContainer.SentimentScore);
    };


    /********************************************************************************/
    /* CUSTOM SCROLLBAR                                                             */
    /********************************************************************************/
    /* create new customer scroll bar for elements with live-tweet CSS class */
    $(".live-tweets").mCustomScrollbar({
        set_width: "95%",
        scrollButtons: { enable: false }
    });

});


// add tweet to the tweet container
function addTweets(profileImageUrl, tweet, sentiment) {
    var container = $(".tweet-container");
    var element;

    // let's limit to only a certain # of tweets.  Any more than that, then we'll remove the top one
    var paragraphs = container.find("p");
    if (container.find("p").length > 50) {
        container.find("p:last-child").remove();
    }

    // create new p element containing profile image and tweet
    switch (sentiment) {
        case 4: element = "<p class='positive'>"; break;
        case 0: element = "<p class='negative'>"; break;
        default: element = "<p class='neutral'>"; break;
    }
    element = element + "<img class='profile-image' src='" + profileImageUrl + "'/>" + myEscape(tweet) + "</p><div style='clear:both;'/>";

    container.prepend(element);
    updateScrollBar();
}


function updateScrollBar() {
    $(".live-tweets").mCustomScrollbar("update");
    //$(".live-tweets").mCustomScrollbar("scrollTo", "bottom");

}

// add or update topic content divs
function addTopic(name, tweetCount, avgSentiment) {

    name = trim(name);
    tweetCount = isNaN(tweetCount) ? 0 : tweetCount;
    var foundTopic = findTopic(name);

    // check to see if a topic with matching name already exists.  
    // if so, then update the tweet count and sentiment
    if (null != foundTopic) {
        $("#" + foundTopic + " .topic-tweet-count").text(tweetCount + tweetWindow);
        $("#" + foundTopic + " .topic-sentiment").text(avgSentiment + "/4.0");
        // flash div background
        $("#" + foundTopic).animateHighlight(highlightColor, backgroundColor, highlightDuraction);
    }

    // this is a new topic
    else {
        var emptyTopic = findEmptyTopic();

        // is there any empty slot available.  if so then add new topic there.
        if (null != emptyTopic) {
            //alert(emptyTopic + " is empty.  add new topic here");
            $("#" + emptyTopic + " .topic-name").text(name);
            $("#" + emptyTopic + " .topic-tweet-count").text(tweetCount + tweetWindow);
            $("#" + emptyTopic + " .topic-sentiment").text(avgSentiment + "/4.0");
            // flash div background
            $("#" + emptyTopic).animateHighlight(highlightColor, backgroundColor, highlightDuraction);
        }
        // all slots are taken
        else {
            // if the current topic tweet count is more than an existing topic (with the least tweet count) 
            // then replace it.  otherwise, ignore the topic
            var topicWithLessTweets = findTopicWithFewerTweetCount(tweetCount);
            if (null != topicWithLessTweets) {
                //alert("found a topic with less tweets: " + topicWithLessTweets);
                $("#" + topicWithLessTweets + " .topic-name").text(name);
                $("#" + topicWithLessTweets + " .topic-tweet-count").text(tweetCount + tweetWindow);
                $("#" + topicWithLessTweets + " .topic-sentiment").text(avgSentiment + "/4.0");
                // flash div background
                $("#" + topicWithLessTweets).animateHighlight(highlightColor, backgroundColor, highlightDuraction);
            }
            else {
                //alert("didn't find any slots for current topic");
            }
        }
    }
}

// find a "topic" div where the text value of child element <div class="topic-name"> matches the input parameter
function findTopic(name) {
    for (var id in topicIds) {
        if ($("#" + topicIds[id] + " .topic-name:contains('" + name + "')").length > 0) {
            return topicIds[id];
        }
    }
    return null;
}

// find a "topic" div where the text value of child element <div class="topic-name"> is empty
function findEmptyTopic() {
    for (var id in topicIds) {
        if ($("#" + topicIds[id] + " .topic-name").text() == "") {
            return topicIds[id];
        }
    }
    return null;
}

// find an existing topic which has the smallest tweet count and is less than tweetCount input param
function findTopicWithFewerTweetCount(tweetCount) {
    var finalTopic = "";
    var finalTweetCount = 10000;

    for (var id in topicIds) {
        var topicTweetCount = parseInt($("#" + topicIds[id] + " .topic-tweet-count").text());
        // if topic tweet count less than input parameter, then choose it
        if (topicTweetCount < tweetCount) {
            // if chosen topic has less tweet count then one found earlier, then take this one
            // otherwise, keep the one we found earlier
            if (topicTweetCount < finalTweetCount) {
                finalTweetCount = topicTweetCount;
                finalTopic = topicIds[id];
            }
        }
    }
    if (finalTopic == "" || finalTweetCount == 10000) return null;
    else return finalTopic;
}

// replaces &, <, >, ", ', / with the appropriate escape characters
function myEscape(someString) {
    return someString
            .replace("&", "&amp;")
            .replace("<", "&lt;")
            .replace(">", "&gt;")
            .replace('"', "&quot;")
            .replace("'", "&#x27;")
            .replace("/", "&#x2F;");
}

// remove multiple, leading or trailing spaces 
function trim(s) {
    s = s.replace(/(^\s*)|(\s*$)/gi, "");
    s = s.replace(/[ ]{2,}/gi, " ");
    s = s.replace(/\n /, "\n"); return s;
}

// uses jQuery animate feature to change background of an element
$.fn.animateHighlight = function (highlightColor, bgColor, duration) {
    var highlightBg = highlightColor || "#FFFF9C";
    var animateMs = duration || 1500;
    //var originalBg = this.css("backgroundColor");
    this.stop().css("background-color", highlightBg).animate({ backgroundColor: bgColor }, animateMs);
}