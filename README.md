# spectral-linter-wrapper
A sample wrapper for the Spectral linter

This application is a wrapper for the Spectral linter.
That is, it's a graphical interface for the Spectral linter, which is normally a command line operation.

## Running the standalone application
I suggest running this application first to see what the application is intended to look like and how it runs.

1. Before running the application, Spectral linter must be already installed and be able to run succcessfully.
For full instructions, see https://docs.stoplight.io/docs/spectral/b8391e051b7d8-installation.
The typical installation is for the **Spectral CLI client**. 
However, this requires that **npm** and **node.js** be installed prior to spectral-cli.
The page also has an all-in-one installer **Executable Binaries** for a single step installation.

1. To run the compiled application download and uncompress **Lint_Wrapper.zip**.
The only item in this file is a **Lint_Wrapper.exe**.

1. Launch the application.

The following controls are available. To run the application, follow these steps:
1. Select the target OpenFile file using the **Open target file** (#1).
1. Select the spectral rules file using **Open ruleset file** (#2).
2. This will likely named like **spectral.yaml** or **.spectral.yaml**.
1. Select **Evaluate target file** (#3). This starts the linting process.
   A command window may display briefly.
   In a moment, the bottom portion of the screen completes (#4)

The bottom portion displays a list of controls for groups of messages, or to list individual messages.
Those messages display to the right, showing a list of individual instances for each message.
