# Local File Converter to MP4

This file converter allows you to localy convert and video file to MP4 localy, allowing you to convert any size file.

## Description

As a game developer which primarly works with Unreal Engine, I would usually find mysself having to convert my video files to MP4 as its the easiest for me to work with within the engine. This would always lead me to either online file converters with file size restrictions, or expensive file converter apps that would either be confusing to use or not work at all.

This script works in a 3 modes:
- Single Directory
- Multiple Directories
- Drag and Drop (Single and Multiple)

Single Directory works by the user entering the path to the folder containing all of the files they would like to convert to MP4. i.e
```C:\\FilesToConver```

Multiple Directory works by the user entering multiple paths to different folders, each separated with a semi-colon. i.e
```C:\\FilesToConver; D:\\Movies; C:\\...\AnimationClips;```

Drag and Drop works by the user selecting one or more folder and simply draging and droping them onto the excecutable.

## Running the program

To run the program, simple launch double click on the exe or drag and drop the file.

You will then be prompted to allow the program to read and convert subfolders.
```Do you want to include subfolders? (y/n): ```
This preserves the subfolder structer on output, meaning all the files are in the same folders in the output directory as they apear in your input folder.

Then, you'll be asked to enter an output directory if you so please.
```Enter output directory for converted files (press Enter for default 'Converted' folder inside first input directory):```
This is completely optional but usefull if your running low on storage on one drive and would like to move the output to a drive with more storage.

After this, simply let the magic happen. The program will let give you a status on the current progress, as well as when its done converting.

## Dependencies

- .NET 8.0 SDK
- NuGet package: `Xabe.FFmpeg` (version 5.2.6)
- Currently only tested on windows 10 and 11

### FFmpeg Requirement

This project uses the FFmpeg executable for video conversion. You must have FFmpeg installed and available on your system. Download it from [ffmpeg.org](https://ffmpeg.org/download.html) and ensure it is added to your system PATH.

## Version History

* 1.1
    * Added Drag and Drop
    * Allow for user input directories, as well as multiple directories
    * Added a comprehensive progress bar
    * Added a Readme file
    * File converter now always skips MP4 files to save on resources and time
* 1.0
    * Initial Project Release
    * Input and Ouput directories hard coded to ```C:\\ToMP4```
    * Known issue - MP4 files are somtimes not skipped, tanking performace