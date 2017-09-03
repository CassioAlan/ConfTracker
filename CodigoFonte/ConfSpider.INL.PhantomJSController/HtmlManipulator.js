// ==== teste ====
// var genericFunctions = require('./HMGenericFunctions.js');
// genericFunctions.test('from hm');
// phantom.page.injectJs('NodeInformation.js');

// var info = new NodeInformation();
// console.log(info.pTL);
// ==== teste ====

function analizeHTML(filepath, abstractPattern, paperPattern, notificationPattern, finalPattern, conferencePattern) {
  try {
    var fs = require('fs');

    if (fs.isFile(filepath) && filepath.indexOf('.html') != -1) {
        var page = require('webpage').create();

        page.onConsoleMessage = function (msg) {
            console.log(msg);
        };

        // recebe uma saída JSON do evaluate
        page.onCallback = function (data) {
            console.log("onCallback");
            console.log(data.a);
        };

        var dateNodesOutside = [];

        page.open('file:///' + filepath, function (status) {

            if (status === 'success') {

                //page.render("C:/_temp/a.png");

                page.includeJs('file:///' + fs.workingDirectory + '/jquery-2.1.4.js', function () {

                    // parâmetros a serem passados para o contexto do page.evaluate
                    var jsAsString = '';
                    jsAsString += fs.read('HMGenericFunctions.js');
                    jsAsString += '\n\n'
                    jsAsString += fs.read('NodeInformation.js');
                    // paramsAsString += '\n\n'
                    // paramsAsString += fs.read('jquery-2.1.4.js');

                    var dateNodes = [];
                    nodes = page.evaluate(function (jss, abstractPattern, paperPattern, notificationPattern, finalPattern, conferencePattern) {
                        //eval string para uso a baixo
                        eval(jss);

                        // console.log(abstractPattern);
                        // console.log(paperPattern);
                        // console.log(notificationPattern);
                        // console.log(finalPattern);
                        // console.log(conferencePattern);

                        // var relevantNodes = [];
                        // getNodeByText('proposals').each(function(){
                        //   relevantNodes.push(new NodeInformation($(this)));
                        //   console.log($(this).text());
                        // });

                        var dateNodes = [];
                        getNodeWithDate().each(function () {
                            //var nodeI = new NodeInformation($(this));
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                dateNodes.push(nodeI);
                            }
                        });

                        var abstractNodes = [];
                        getNodeWithPattern(abstractPattern).each(function () {
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                abstractNodes.push(nodeI);
                            }
                        });

                        var paperNodes = [];
                        getNodeWithPattern(paperPattern).each(function () {
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                paperNodes.push(nodeI);
                            }
                        });

                        var notificationNodes = [];
                        getNodeWithPattern(notificationPattern).each(function () {
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                notificationNodes.push(nodeI);
                            }
                        });

                        var finalNodes = [];
                        getNodeWithPattern(finalPattern).each(function () {
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                finalNodes.push(nodeI);
                            }
                        });

                        var conferenceNodes = [];
                        getNodeWithPattern(conferencePattern).each(function () {
                            var nodeI = new NodeInformation($(this));
                            if (nodeI.pC[0] != 0) {
                                conferenceNodes.push(nodeI);
                            }
                        });

                        //window.callPhantom({ a: document.title });

                        return [dateNodes, abstractNodes, paperNodes, notificationNodes, finalNodes, conferenceNodes];

                    }, jsAsString, abstractPattern, paperPattern, notificationPattern, finalPattern, conferencePattern);

                    //o arquivo txt de datas é gerado independente de haver data para saber que o .html já foi processado
                    var dateNodes = nodes[0];
                    var out = "#####";
                    dateNodes.forEach(function (e, i, a) {
                        // console.log(e.text);
                        // console.log(e.pTL);
                        // console.log(e.pTR);
                        // console.log(e.pBL);
                        // console.log(e.pTL);
                        // console.log(e.pC);
                        out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                        out += "\n#####";
                    });

                    fs.write(filepath + "_datesposition.txt", out, 'w');

                    //os arquivos txt de rótulos não são gerados desnecessariamente
                    var abstrNodes = nodes[1];
                    if (abstrNodes.length > 0)
                    {
                      var out = "#####";
                      abstrNodes.forEach(function (e, i, a) {
                          // console.log(e.text);
                          // console.log(e.pTL);
                          // console.log(e.pTR);
                          // console.log(e.pBL);
                          // console.log(e.pTL);
                          // console.log(e.pC);
                          out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                          out += "\n#####";
                      });

                      fs.write(filepath + "_abstractposition.txt", out, 'w');
                    }

                    var paperNodes = nodes[2];
                    if (paperNodes.length > 0)
                    {
                      var out = "#####";
                      paperNodes.forEach(function (e, i, a) {
                          // console.log(e.text);
                          // console.log(e.pTL);
                          // console.log(e.pTR);
                          // console.log(e.pBL);
                          // console.log(e.pTL);
                          // console.log(e.pC);
                          out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                          out += "\n#####";
                      });

                      fs.write(filepath + "_paperposition.txt", out, 'w');
                    }

                    var notificationNodes = nodes[3];
                    if (notificationNodes.length > 0)
                    {
                      var out = "#####";
                      notificationNodes.forEach(function (e, i, a) {
                          // console.log(e.text);
                          // console.log(e.pTL);
                          // console.log(e.pTR);
                          // console.log(e.pBL);
                          // console.log(e.pTL);
                          // console.log(e.pC);
                          out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                          out += "\n#####";
                      });

                      fs.write(filepath + "_notificationposition.txt", out, 'w');
                    }

                    var finalNodes = nodes[4];
                    if (finalNodes.length > 0)
                    {
                      var out = "#####";
                      finalNodes.forEach(function (e, i, a) {
                          // console.log(e.text);
                          // console.log(e.pTL);
                          // console.log(e.pTR);
                          // console.log(e.pBL);
                          // console.log(e.pTL);
                          // console.log(e.pC);
                          out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                          out += "\n#####";
                      });

                      fs.write(filepath + "_finalposition.txt", out, 'w');
                    }

                    var conferenceNodes = nodes[5];
                    if (finalNodes.length > 0)
                    {
                      var out = "#####";
                      conferenceNodes.forEach(function (e, i, a) {
                          // console.log(e.text);
                          // console.log(e.pTL);
                          // console.log(e.pTR);
                          // console.log(e.pBL);
                          // console.log(e.pTL);
                          // console.log(e.pC);
                          out += [e.text, e.pTL, e.pTR, e.pBL, e.pBR, e.pC].filter(function (val) { return val; }).join("#|||#");
                          out += "\n#####";
                      });

                      fs.write(filepath + "_conferenceposition.txt", out, 'w');
                    }

                    phantom.exit();
                });
            }
            else {
                console.log('Error reading archive: ' + filepath);
                phantom.exit();
            }
        });
    }
    else {
        console.log('Isnt an archive: ' + filepath);
        phantom.exit();
    }
  }
  catch(e){
    console.log('Phantomjs script exception: ' + e.message);
    phantom.exit();
  }
}

module.exports = {
    analizeHTML: analizeHTML
};
