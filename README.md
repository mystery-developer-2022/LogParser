# LogParser

Interpret a web server log file and print a number of pre-determined metrics. 

## Usage

```
dotnet run -- --help
```
Display this help:
```
LogParser 1.0.0
Copyright (C) 2022 LogParser

  -f, --file      log file to parse or 'programming-task-example-data.log' by default

  -s, --stream    parse file in memory-efficient stream mode instead of DOM mode

  --help          Display this help screen.

  --version       Display version information.
```

```
dotnet run -- --file my-file.log --stream
```
Process the file `my-file.log` in stream mode

## DOM Vs Stream mode
The solution presents 2 processing models for working with log data.
### DOM Mode (default)
The entire file is parsed into an in-memory object model. 

- 😀 easier to understand
- 😀 can easily report on and manipulate/trsansform/export the data in any way after reading
- ☹ high memory usage. not suitable for large log files.

### Stream Mode (-s switch)
Parse through the file stream collecting metrics along the way.
- 😀 faster, memory efficient
- ☹ more complex to add additional metrics
