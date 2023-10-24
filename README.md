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
