
with open("data2.csv", 'r+') as f:
    d = f.readlines()
    f.seek(0)
    for i in d:
        if not('R')in i:
            f.write(i)
    f.truncate()

f.close()


with open("data2.csv", 'r+') as f:
    d = f.readlines()
    f.seek(0)
    for i in d:
        if i.split():
            f.write(i)
    f.truncate()

f.close()


