import sys
import pandas as pd

f1 = open(sys.argv[1], 'r', encoding='utf8')
actual = []
predic = []
for line in f1:
	if line == '\n' or line.split()==[]:
		continue
	x = line.strip().split()
	actual.append(x[-2])
	predic.append(x[-1])

y_actu = pd.Series(actual, name='Actual')
y_pred = pd.Series(predic, name='Predicted')
df_confusion = pd.crosstab(y_actu, y_pred, rownames=['Actual'], colnames=['Predicted'], margins=True)

print(df_confusion)

resultFile = open('confusion_matrix_result.txt', 'w')
resultFile.write(str(df_confusion))
resultFile.close()

f1.close()