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

with open("data2.csv", "r+") as input:
      
    # Creating "gfg output file.txt" as output
    # file in write mode
    with open("data6.csv", "w+") as output:
        d = input.readlines()
        input.seek(0)
        last_epoch = []
        for i in d:
            i = i.replace("_", "," )
            epoch_array = i.split(',')[0]
            if len(epoch_array) == 2:
                i= last_epoch+','+i
                print(last_epoch)
            last_epoch = i.split(',')[0]
            output.write(i)
        output.truncate()
 

