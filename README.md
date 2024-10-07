# Spectral-Linter-Wrapper
This is an application wrapper for the Spectral linter.

That means it's a Windows application. It can be used to run Spectral lint.
Instead of displaying the results in an awkward and hard to read command window, the results are redirected to the application.
The application uses an intuitive layout.
The results can be filtered to show results for one, specific error or warning type.
In other words, it makes it much easier to read.

<img src="/support/application_screenshot.JPG" alt="Resized Image" style="width: 50%; height: 50%; border: 5px solid black;">

## Running the application
1. Before running the application, Spectral linter must be already installed and be able to run succcessfully.
For full instructions, see https://docs.stoplight.io/docs/spectral/b8391e051b7d8-installation.
The typical installation is for the **Spectral CLI client**. 
However, this requires that **npm** and **node.js** be installed prior to spectral-cli.
The page also has an all-in-one installer **Executable Binaries** for a single step installation.

1. Download and uncompress **Lint_Wrapper.zip**.
The only item in this file is a **Lint_Wrapper.exe**.

1. Launch the application.

The following controls are available. To run the application, follow these steps:
1. Select the target OpenFile file using the **Open target file**.
1. Select the spectral rules file using **Open ruleset file**.
   This will likely named like **spectral.yaml** or **.spectral.yaml**.
1. Select **Evaluate target file**. This starts the linting process.
   A command window may display briefly.
   In a moment, the bottom portion of the screen completes.

The bottom portion displays a list of controls for groups of messages, or to list individual messages.
Those messages display to the right, showing a list of individual instances for each message.
