function getNodeByText(str) {
  return $('*').contents().filter(
    function() {
      return this.wholeText && this.wholeText.indexOf(str) >= 0
    }).parent();
}

function getNodeWithDate(str) {
  return $('*').contents().filter(
    function() {

      // Pattern pattern = Pattern.compile("((\\d{2}|\\d{1})[st|nd|rd|th]*.(January|February|March|April|May|June|July|August|September|October|November|December).{0,5}\\d{4})"
      //         + "|(janeiro|fevereiro|março|abril|maio|junho|julho|agosto|setembro|outubro|novembro|dezembro)"
      //         + "|((January|February|March|April|May|June|July|August|September|October|November|December).(\\d{2}|\\d{1})[st|nd|rd|th]*.{0,5}\\d{4})"
      //         + "|(\\d{2}.\\d{2}.\\d{4})"
      //         + "|((January|February|March|April|May|June|July|August|September|October|November|December).(\\d{2}|\\d{1})[st|nd|rd|th]*)"
      //         + "|((\\d{2}|\\d{1})-(\\d{2}|\\d{1}).(January|February|March|April|May|June|July|August|September|October|November|December).{0,5}(\\d{4}?))"
      //         + "|((Jan.*|Feb.*|Mar.*|Apr.*|May.*|Jun.*|Jul.*|Aug.*|Sep.*|Sept.*|Oct.*|Nov.*|Dec.*).(\\d{2}|\\d{1}))|(\\d{2}.\\d{2}.(\\d{4}|\\d{2}))"
      //         + "|((Jan.*|Feb.*|Mar.*|Apr.*|May.*|Jun.*|Jul.*|Aug.*|Sep.*|Sept.*|Oct.*|Nov.*|Dec.*).(\\d{2}|\\d{1})[st|nd|rd|th]*.{0,5}\\d{4})", Pattern.CASE_INSENSITIVE);


      // var p = /((\d{2}|\d{1})[st|nd|rd|th]*.(January|February|March|April|May|June|July|August|September|October|November|December).{0,5}\d{4})|(janeiro|fevereiro|março|abril|maio|junho|julho|agosto|setembro|outubro|novembro|dezembro)|((January|February|March|April|May|June|July|August|September|October|November|December).(\d{2}|\d{1})[st|nd|rd|th]*.{0,5}\d{4})|(\d{2}.\d{2}.\d{4})|((January|February|March|April|May|June|July|August|September|October|November|December).(\d{2}|\d{1})[st|nd|rd|th]*)|((\d{2}|\d{1})-(\d{2}|\d{1}).(January|February|March|April|May|June|July|August|September|October|November|December).{0,5}(\d{4}?))|((Jan.*|Feb.*|Mar.*|Apr.*|May.*|Jun.*|Jul.*|Aug.*|Sep.*|Sept.*|Oct.*|Nov.*|Dec.*).(\d{2}|\d{1}))|(\d{2}.\d{2}.(\d{4}|\d{2}))|((Jan.*|Feb.*|Mar.*|Apr.*|May.*|Jun.*|Jul.*|Aug.*|Sep.*|Sept.*|Oct.*|Nov.*|Dec.*).(\d{2}|\d{1})[st|nd|rd|th]*.{0,5}\d{4})/

      var p = /(january|february|march|april|may|june|july|august|september|october|november|december).(\d{2}|\d{1}),.\d{4}/i;
      var pattern = new RegExp(p);
      return this.wholeText && pattern.test(this.wholeText);
    }).parent();
}

function test(t)
{
  console.log('teste GenericFunctions: ' + t);
}

// usando try, pois o arquivo é executado via eval
// neste caso dá erro neste trecho, porém neste caso também não faz falta
// já se o arquivo for utilizado via require, este trecho é necessário
try {
  module.exports = {
    getNodeByText: getNodeByText,
    getNodeWithDate: getNodeWithDate,
    test: test
  }
}
catch(e){}
