# Draw-IO Library generator(Github API Folder Parser)
A console application in C# 7 that takes a Github URL as input and fetches information about the folder at that URL through the Github API. The program creates a list of files in the folder, including their names and download URLs, and saves that information as an XML document that can be consumed by draw.io

# How to Use
Clone the repository to your local machine
Open the solution in Visual Studio
Build the solution
Run the program from the command line and enter the Github URL in the following format:
https://github.com/{username}/{repository name}/tree/{branch}/{target directory}
The program will fetch information about the folder, create a list of files, and save that information as an XML document.
# XML Document Structure
The resulting XML document starts with <mxlibrary> and ends with </mxlibrary>. The contents of the document consist of an array of objects, each with the following properties:

data: the URL of the file
w: the width of the image
h: the height of the image
title: the name of the image
aspect: the aspect ratio of the image

# Method for Transforming File Names
The method TransformFileName takes a string (the file name) as input and returns a more user-friendly version of the name. The method adds spaces for camel case wording, removes the file extension from the string, replaces dashes with spaces, and removes repeatable patterns(TBD) to leave only the unique part of the name.

# Example
![image](https://user-images.githubusercontent.com/1761348/215716372-43a247d9-1d5f-49da-8ac8-8708070b4be7.png)
![image](https://user-images.githubusercontent.com/1761348/215716495-042848f2-bdc0-4451-a82b-b292b5f7b629.png)


# Note
This code is designed to work with Github API and the structure of the API may change in the future, causing this code to become outdated. Same as current draw.io library format hardcoded and may require adjustments on future.

# Contributing
If you would like to contribute to this project, please fork the repository and make a pull request with your changes. Before making a pull request, please make sure that your changes adhere to the coding style used in the project and that all tests pass.

# Issues
If you encounter any issues while using this application, please file a bug report in the repository's Issues section. Be sure to include as much information as possible, including any error messages and steps to reproduce the issue.
