function NodeInformation(element){
  //top left
  this.pTL = [element.position().top, element.position().left];
  // this.pTL = getTL(element[0]);
  // console.log("getTL:" + this.pTL);
  // console.log("jQuery:" + [element.position().top, element.position().left]);

  //top right
  this.pTR = [element.position().top, element.position().left + element.width()];
  //botton left
  this.pBL = [element.position().top + element.height(), element.position().left];
  //botton right
  this.pBR = [element.position().top + element.height(), element.position().left + element.width()];
  //center
  this.pC = [element.position().top + (element.height() / 2), element.position().left + (element.width() / 2)];

  // elemento DOM - a partir de determinado ponto nao consigo acessar... por isso criei a propriedade text a baixo
  this.domElement = element;
  this.text = element.text();

  //pensei que poderia ter um retorno diferente das posições, mas é a mesma coisa
  // function getTL(el) {
  //   for (var left=0, top=0;
  //        el != null;
  //        left += el.offsetLeft, top += el.offsetTop, el = el.offsetParent);
  //   return [top,left];
  // };
}
