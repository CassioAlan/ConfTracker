import sys

#usage : python accuracy.py <file>

csv = 'AVALIATION;correctly tagged words;incorrectly tagged words;total words;total sentences;Accuracy\n'

#TOTAL
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	count = count + 1
	x = line.strip().split()
	if(x[-1]==x[-2]):
		true=true+1
	else:
		false = false+1

print('TOTAL REPORT')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TOTAL REPORT;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

#ANY TAG DIFFFERENT FROM 0
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] != "0"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAGs DIFFERENT FROM 0')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAGs DIFFERENT FROM 0;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG LBL_ABS
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "LBL_ABS"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG LBL_ABS')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG LBL_ABS;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG LBL_PPR
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "LBL_PPR"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG LBL_PPR')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG LBL_PPR;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG LBL_ACC
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "LBL_ACC"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG LBL_ACC')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG LBL_ACC;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG LBL_CAM
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "LBL_CAM"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG LBL_CAM')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG LBL_CAM;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG LBL_EVE
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "LBL_EVE"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG LBL_EVE')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG LBL_EVE;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG VLU_ABS
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "VLU_ABS"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG VLU_ABS')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG VLU_ABS;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG VLU_PPR
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "VLU_PPR"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG VLU_PPR')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG VLU_PPR;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG VLU_ACC
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "VLU_ACC"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG VLU_ACC')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG VLU_ACC;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG VLU_CAM
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "VLU_CAM"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG VLU_CAM')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG VLU_CAM;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

# TAG VLU_EVE
f1 = open(sys.argv[1], 'r', encoding='utf8')
count = 0
true = 0
false = 0
sentences = 0
for line in f1:
	if line == '\n' or line.split()==[]:
		sentences+=1
		continue
	x = line.strip().split()
	if(x[-2] == "VLU_EVE"):
		count = count + 1
		if(x[-1]==x[-2]):
			true=true+1
		else:
			false = false+1

print('TAG VLU_EVE')
print('#correctly tagged words:', true)
print('#incorrectly tagged words:', false)
print('#total words', count)
print('#total sentences', sentences)
acc = ''
if count > 0:
	acc = 100.0*true/count
else:
	acc = 'NONE'
print('Accuracy = ', acc)
	
csv += 'TAG VLU_EVE;'+str(true)+';'+str(false)+';'+str(count)+';'+str(sentences)+';'+str(acc)+'\n'

print('=========================================')

csv = csv.replace('.', ',')
print(csv)
resultFile = open('accuracy_result.txt', 'w')
resultFile.write(csv)
resultFile.close()

f1.close()