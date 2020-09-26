# B-Ram-Machine
The Ram Machine interpreter and editor
![](https://github.com/Biegus/B-Ram-Machine/blob/master/Images/RamMachineScreen.png)

## [Stable bulid version](https://github.com/Biegus/B-Ram-Machine/releases)

# Documentation

This is the implementation of interpreter of Ram Machine's code, pseudo assembly code in abstract space, designed to learn basic of algorithms.

Main runtime's memory is stored in some kind of an array, which you can access by index number. The array can be both modified and read.

Input is defined before the program starts. It is read-only from the machine's code.

The output is the result of the program. It is write-only from the machine's code.

All math operations can be done only in the 0th index.
There's only one type - integer.
Ram machine's code has twelve instructions + labels.


## load
load x - loads value from xth index to the 0th index (m[0]=m[x])

load =x - loads exact x value to the 0th index (m[0]=x)

load ^x or load\*x - loads value from xth index to the 0th index (m[0]=m[m[x]])

## store
store x - moves value from 0th index to the xth index (m[x]=m[0])

store ^x or store\*x - moves value from 0th to the index from xth0 index (m[m[x]]=m[0])

## read
gets first available value from input and puts into:

read x - the xth index (m[x]=input)

read ^x or read \*x - the index in the xth index (m[m[x]]=input)

## write
adds value to output:
write x - from the xth index (write(m[x])

write =x - which is the exact x value (write(x))

write ^x or write\*x - from the index from xth index (write(m[m[x]))

## halt 
Ends the program

## math operations
mult (multiplication)

div (integer division)

sub  (substraction)

add (addition)

Math operations are doing operations on zeroth index. (m[0]=m[0] operation z)

opr means any of that math operations

opr =x - does operation with exact x value (m[0]= m[0] operation x)

opr x - does operation with value from xth index (m[0]=m[0] operation m[x])

opr ^x or operation \*x - does operation with value from the index from the xth index (m[0]=m[0] operation m[m[x]]


## label
nameOfLabel: - Label means a point where you can jump in a certain situation. It can be put before the line or inside its line

## jump
jump nameOfExistingLabel - jumps to the line with that label

## jzero
jzero nameOfExistingLabel - jumps to the line with that label if 0th memory equals 0 (if m[0]==0 =>jump nameOfExistingLabel)

## jgtz
jgtz nameOfExistingLabel - jumps to the line with that label if oth mempory is greater than zero (if m[0]>0 => jump nameOfExistingLabel)


# Examples of basic program

## calculating abs value

```

read 0
jgtz skip
mult =-1
skip:
write 0
```
## calculating modulo
```

read 1
read 2
load 1
div 2
mult 2
mult =-1
add 1
write 0
```





