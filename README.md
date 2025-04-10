# Booking Console App

This console app is a basic implementation of a hotel booking which covers secnerios such as room availability checks, booking allocations, and edge cases like room type mismatches or overbookings.

Prerequisites
Before running the project, ensure you have the following installed on your machine:

.NET 9 SDK: Download and install from https://dotnet.microsoft.com/en-us/download/dotnet/9.0, used this version of framework taking in consideration that until this app will be ready this version will be fully supported.
xUnit: This is the testing framework used in this project. It is included as a NuGet package.
Visual Studio Code (or any other code editor).
Terminal (macOS Terminal, PowerShell, or Command Prompt for Windows).
Restore Dependencies
Make sure all the project dependencies are restored by running the following command:

dotnet restore

This command will download all necessary NuGet packages, including xUnit and other dependencies.

Build the Project
To build the project, run:

dotnet build

This will compile the code and prepare it for testing.

Running the Tests
To run the tests using xUnit, run the following command:

dotnet test

This will automatically detect and run all tests in the project. The test results will be displayed in the terminal.

Expected Test Results The tests will check the availability of rooms for specific periods. They will validate room allocation logic for different numbers of people. Edge cases like unavailable hotels, non-existent room types, and overbookings will be handled and validated. Test Output Example

Code Structure

The project follows the following structure:

- Program.cs which is the entry point
- Data where the json format data is located
- Repositories is the layer which comunicate with the data from json files
- Services is the layer where all calculations are performed
