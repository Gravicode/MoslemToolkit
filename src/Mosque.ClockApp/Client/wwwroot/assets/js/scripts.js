//var countDownSpan = 30 ; // total minutes to display countdown before prayer time. Set to 0 to always show countdown;
var _prayerTimes = [];
var _closestPrayerTime = null;

// media type :
// image  : 'path/image.jpg'
// slider : ['path/image.jpg','path/video.mp4']
// video  : 'path/video.mp4' 
// ------------------------
// note : don't use external url like youtube or vimeo for video
function initPage(media) {
    if (!media) media = 'assets/img/bg01.png'; // default
    var isSlider = Array.isArray(media);
    var isImage = !isSlider && (['png','jpeg','bmp','jpg','gif','svg','apng','afiv','webp'].filter(a=>a==media.toLowerCase().split('.').pop()).length>0);
    var isVideo = !isImage && media.toString().toLowerCase().split('.').pop() == 'mp4';
    var type = (isSlider && 'slider') || (isImage && 'image') || (isVideo && 'video');
    var tgt = $('.page-background').empty();

    switch(type) {
        case 'slider' :
            var slider = $('<div class="owl-carousel owl-theme manual"></div>');
            for(var x in media) {
                var m = media[x];
                if (m.toLowerCase().split('.').pop() == 'mp4') {
                    var itm = $('<div class="item-video"><video preload autoplay muted loop><source src="" type="video/mp4" /></video></div>');
                    $('source',itm).attr('src',m);
                    slider.append(itm);
                } else { 
                    var itm = $('<div class="item"><img src="" alt=""></div>');
                    $('img',itm).attr('src',m);
                    slider.append(itm);
                }
            }
            tgt.append(slider);
            $('.owl-carousel').owlCarousel({
                loop:true,
                items:1,
                autoplay: true,
                interval:10000,
                autoplayTimeout:5000,
            });
            break;
        case 'video' : // single video
            var itm = $('<video preload autoplay muted loop><source src="" type="video/mp4" /></video>');
            $('source',itm).attr('src',media);
            tgt.append(itm);
            break;
        case 'image' : // default type
        default :   
            var itm = $('<img src="" alt="">');
            itm.attr('src',media);
            tgt.append(itm);
            break;
    }
}

function getClosestPrayerTime(){
    var dt = new Date();
    var dthour = ('0'+dt.getHours()).slice(-2)+':'+('0'+dt.getMinutes()).slice(-2);
    var closestIdx = -1;
    var closestTitle = '';
    var time = '';
    var tspan = 0;
    var countSpan = 0;
    var timeSpan = '';
    /*
    if (_closestPrayerTime!=null && _closestPrayerTime.tspan>0) {
        var tsp = _closestPrayerTime.tspan - dt.getTime();
        var xhours = tsp / (1000 * 60 * 60);
        var hours = Math.round(xhours);
        var xmins = xhours - hours;
        var mins = Math.round(xmins * 60);
        _closestPrayerTime.countDown = ('0'+hours).slice(-2)+':'+('0'+mins).slice(-2);
        _closestPrayerTime.countSpan = tsp;
    } else {
        */
       var complete = false;
        for(var x in _prayerTimes) {
            for(var y in _prayerTimes[x]) {
                if (dthour<_prayerTimes[x][y]) {
                    closestTitle = y;
                    time = _prayerTimes[x][y];
                    var dtspan = new Date(dt.getTime());
                    dtspan.setHours(parseInt(time.split(':')[0]));
                    dtspan.setMinutes(parseInt(time.split(':')[1]));
                    tspan = dtspan.getTime();
                    countSpan = tspan - dt.getTime();
                    var xhours = countSpan / (1000 * 60 * 60);
                    var hours = Math.floor(xhours);
                    var xmins = xhours - hours;
                    var mins = Math.round(xmins * 60);

                    timeSpan = ('0'+hours).slice(-2)+':'+('0'+mins).slice(-2);
                    complete = true;
                    break;
                }
            }
            closestIdx++;
            if (complete) break;
        }
        if (closestIdx==-1) {
            // set to index 0 
            closestIdx = 0;
            for(var x in _prayerTimes[0]) {
                closestTitle = x;
                time = _prayerTimes[0][x];
                var dtspan = new Date(dt.getTime());
                dtspan.setDate(dtspan.getDate()+1);
                dtspan.setHours(parseInt(time.split(':')[0]));
                dtspan.setMinutes(parseInt(time.split(':')[1]));
                tspan = dtspan.getTime();
                countSpan = tspan - dt.getTime();
                var xhours = countSpan / (1000 * 60 * 60);
                var hours = Math.floor(xhours);
                var xmins = xhours - hours;
                var mins = Math.round(xmins * 60);
                timeSpan = ('0'+hours).slice(-2)+':'+('0'+mins).slice(-2);
                break;
            }
             
        }
        _closestPrayerTime = {
            index : closestIdx,
            title : closestTitle,
            time : time,
            tspan : tspan,
            countSpan : countSpan,
            countDown : timeSpan
        }
    //}
    return _closestPrayerTime;
}

// time : [{'title':'time'},...]
function initPrayerTime(time) {
    var tgt = $('.prayer-schedule').empty();
    _prayerTimes = time;
    for(var x in time) {
        for(var y in time[x]) {
            var itm = $('<div class="box"><div class="header">'+y+'</div><div class="time alt-font">'+time[x][y]+'</div></div>');
            tgt.append(itm);
        }
    }
    
}

function initClock(){
    var dt = new Date();
    var hour = ('0'+dt.getHours()).slice(-2);
    var min = ('0'+dt.getMinutes()).slice(-2);
    var tgt = $('.clock');
    var tgtCountDown = $('.timer>.counter');
    var tgtSch = $('.prayer-schedule');
    var h = $('.hour',tgt);
    var m = $('.min',tgt);
    var s = $('.sec',tgt);
    if (h.text()!=hour) h.text(hour);
    if (m.text()!=min) m.text(min);

    var cp = getClosestPrayerTime();
    var tcd = cp.countDown.split(':');
    var th = $('.countdown>.hour',tgtCountDown);
    var tm = $('.countdown>.min',tgtCountDown);
    var tx = $('.target',tgtCountDown);
    var tb = $('>.box:nth('+cp.index+')',tgtSch);
    if (th.text()!= tcd[0]) th.text(tcd[0]);
    if (tm.text()!= tcd[1]) tm.text(tcd[1]);
    if (tx.text()!= cp.title.toUpperCase()) tx.text(cp.title.toUpperCase());
    if (!tb.hasClass('active')) {
        $('>.box.active',tgtSch).removeClass('active');
        tb.addClass('active');
    }

    if (s.is('.tick')) {
        s.removeClass('tick'); 
        $('.countdown>.sec',tgtCountDown).removeClass('tick'); 
    } else {
        s.addClass('tick');
        $('.countdown>.sec',tgtCountDown).addClass('tick'); 
    }    

    setTimeout(function(){
        if (tgtCountDown.parent().is(':hidden')) tgtCountDown.parent().show();
    },500);
    window.setTimeout(initClock,500);
}


$(function(){
    // override this in each page
    // initPage();

    initClock();
});