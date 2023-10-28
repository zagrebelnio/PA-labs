# PA-labs
Laboratory works of 3rd semester Projecting of Algorithms. Option #12.

## Lab 1 "External sorting algorithms. Polyphase sort."
Program that implements external sorting algorithm polyphase sort in C#.
The input data is a txt file of int numbers one number each row.
The output data is a txt file of sorted numbers.
### Usage
In the command terminal in the project folder run:
```
dotnet run
```
There are 5 parameters:
- Input file path
- Output file path
- Tapes Count (temp files that are used in sorting)
- PreSort (algorithm modification that speeds its work up)
- FileSizeInMB (if need to generate input file)

### Input file path
```
dotnet run -i your_path
```
```
dotnet run --inputPath your_path
```
The default value is "input.txt"

### Output file path
```
dotnet run -o your_path
```
```
dotnet run --outputPath your_path
```
The default value is "output.txt"

### Tapes Count
```
dotnet run -t your_value
```
```
dotnet run --tapesCount your_value
```
The tapes count should be between 3 and 10. The default value is 5.

### Size of generated input file
```
dotnet run -s your_size_in_MB
```
```
dotnet run --fileSizeMB your_size_in_MB
```
The size should be between 1 and 1024. If no need to generate the input file just skip the parameter or set it to 0. The default value is 0.

### Pre Sort
```
dotnet run -p
```
```
dotnet run --preSort
```
The default value is false.

## Lab 2 "Non-informative, informative and local search. N-Queen problem. BFS, A*. Heuristics without regard to visibility"
Program that solves N-Queen problem using BFS and A* algorithms. The start state there is all the Queens are on the board one Queen each column. The start state can be generated given the size of board N or entered into the board.txt file.

The board txt file format should be the following: 0,2,1,3

This input data will be implemented like the next chess board:

| - | 0 | 1 | 2 | 3 |
| :---: |:---:| :---:| :---:| :---:|
|  0  | Q | - | - | - |
|  1  | - | - | Q | - |
|  2  | - | Q | - | - |
|  3  | - | - | - | Q |

So each number should represent the index of row where the Queen is in the column.

The program uses BFS and A* algorithms to find one solution given the start state. You can choose one of the algorithms to solve the problem or both of them. The program limits RAM usage. It can be modified. So if you run out of limited RAM the program throws an exception.

### Usage
In the command terminal in the project folder run:
```
dotnet run
```
There are 4 parameters:
- Number of Queens
- AStar
- BFS
- Memory limit

### Number of Queens
```
dotnet run -q your_value
```
```
dotnet run --nQueens your_value
```

The number of Queens should be in range (0 - 20). Set the value to 0 to use board.txt data and not a generated one.
The default value is 8.

### AStar
```
dotnet run --AStar
```
Use AStar to solve the problem.
The default value is false.

### BFS
```
dotnet run --BFS
```
Use BFS to solve the problem.
The default value is false.

### Memory limit
```
dotnet run -m your_limit_in_GB
```
```
dotnet run --MemoryGB your_limit_in_GB
```
The limit should be in range (1 - 10).
The default value is 4.