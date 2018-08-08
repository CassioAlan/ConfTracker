
README

Sistema desenvolvido na Dissertação de Mestrado
"Extração de informações de conferências em páginas web"
https://www.lume.ufrgs.br/handle/10183/170942

Linguagem utilizada: C#
Base de dados: MySQL



ETAPAS DA EXECUÇÃO DO PROCESSO:

1) IMPORTAÇÃO DAS CONFERÊNCIAS DA TABELA QUALIS PARA A BASE DE DADOS
	1.1 O módulo ConfSpider.UIConsole.ImporterToMySQL possui vários métodos de importação de dados
	1.2 O método principal é o ImportConferencesFromExcel
		1.2.1 Este método faz a importação das conferências, gera a edição que possui a query de consulta de URL

2) CONSULTA DE URLS e DOWNLOAD DO CONTEÚDO WEB
	1.2 Configurar e rodar o módulo ConfSpider.UIConsole.WebPagesDownloader 

3) PRÉ-PROCESSAMENTO - TRANSFORMAÇÃO DE ARQUIVO
	3.1 O módulo ConfSpider.UIConsole.HTMLtoCoNLL transforma os arquivos HTML para um formato de entrada para o CRF
	3.2 Gera um arquivo .csv para cada arquivo .html
	
4) ANOTAÇÃO MANUAL DAS DATAS
	4.1 Necessário apenas no treinamento
	4.2 Abrir arquivos .csv para manualmente e anotar as classes nos tokens

5) PRÉ-PROCESSAMENTO - GERAÇÃO DAS FEATURES
	5.1 Módulo ConfSpider.UIConsole.CoNLLFeatures utiliza os .csv e cria arquivos .csv1 com as features setadas

6) PRÉ-PROCESSAMENTO - Filtro de tokens (executar apenas se estiver sendo criado novo modelo)
	6.1 Para a correta geração do modelo, é necessário desconsiderar parte dos tokens sem classificação ou features
	6.2 Módulo ConfSpider.UIConsole.TokenFilter utiliza arquivos .csv1 e gera .csv11
	
7) PRÉ-PROCESSAMENTO
	7.1 Rodar ConfSpider.UIConsole.CSVsToDATA
		7.1.1 Quando em treinamento, configurar para utilizar arquivos .csv11
		7.1.2 Quando aplicando o modelom, configurar para utilizar arquivos .csv1
		
8) PRÉ-PROCESSAMENTO - Simplificação de labels
	8.1 Necessário apenas quando em treinamento
	8.2 Executar ConfSpider.UIConsole.SimplifyLabels
		Este módulo ajusta as labels anotadas manualmente 
		
9) PROCESSAMENTO - Executar treinamento do CRF via linha de comando
	Por exemplo:
	CRFSharpConsole.exe -encode -template "D:\template_window5.template" -trainfile "D:\train.data" -modelfile "D:\model_window5.model"
	
10) PROCESSAMENTO - Executar aplicação do modelo CRF via linha de comando
	Por exemplo:
	CRFSharpConsole.exe -decode -modelfile "D:\model_window5.model" -inputfile "D:\allSites.data" -outputfile "D:\result_window5.data" -maxword 5000
	
11) PÓS-PROCESSAMENTO - Interpretar classificação do CRF e extrair as datas
	A.1 Executar ConfSpider.UIConsole.ResultoToDB
		A.1.1 A partir dos arquivos de saída do CRF, extrai datas e armazena na base de dados
