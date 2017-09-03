function main(filepath, abstractPattern, paperPattern, notificationPattern, finalPattern, conferencePattern) {

    phantom.page.injectJs('jquery-2.1.4.js');
    var fs = require('fs');

    // var casper = require('casper').create();
    var htmlManipulator = require(fs.workingDirectory + '/HtmlManipulator.js');

    //test1
    //htmlManipulator.saveHTML('http://www.smc2015.org/dates', 1, 'temp\\');
    //htmlManipulator.analizeHTML(fs.workingDirectory + '/temp/1/1.html', 1);

    //test2
    //htmlManipulator.saveHTML('https://www.engr.colostate.edu/mwscas2015/index.php?nav=papers', 2, 'temp\\');
    //console.log(filepath);
    htmlManipulator.analizeHTML(filepath, abstractPattern, paperPattern, notificationPattern, finalPattern, conferencePattern);

    //test3
    //htmlManipulator.saveHTML2(casper, 'http://www.smc2015.org/dates', 3, 'temp\\');

    //test();
}

function test() {
    //console.log('0');
    // if (typeof jQuery != 'undefined') {
    //     console.log('jQuery library is loaded!');
    // } else {
    //     console.log('jQuery library is not found!');
    // }

    // var p = /(January|February|March|April|May|June|July|August|September|October|November|December).(\d{2}|\d{1}),.\d{4}/
    //
    // var q = /d{2}
    //
    // var pattern = new RegExp(p);
    //
    // console.log(pattern.test('February 11, 2015 : Submission of proposals for special sessions'));
    //
    // phantom.exit();
}

var system = require('system');
var args = system.args;
if (args.length === 7) {

    main(args[1], args[2], args[3], args[4], args[5], args[6]);
}
else {
    console.log('São esperados 7 parâmetros.');
    phantom.exit();
}
//test();
