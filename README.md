# File Emailer

File Emailer is a simple application that monitors a directory and automatically sends new files via email. 

## Features

- Monitors a specified directory for new files.
- Automatically sends new files via email.
- Easy configuration of monitoring and email sending parameters.

## Prerequisites

- Tested using .NET 8.0 

- No external dependencies

## Installation

1. Clone this GitHub repository to your local machine:

```bash
git clone https://github.com/AndreCostaaa/file-emailer.git
cd file-emailer
```

## Configuration

1. Create a `config.json` file in the root directory of the project with the following content:

```json
{
  "smtpServer": "",
  "smtpPort": "",
  "watchFolder": "",
  "archiveFolder": "",
  "senderEmail": "",
  "receiverEmail": "",
  "senderEmailToken": ""
}
```

Don't forget to fill out every variable

## Usage

To start the application, simply run the application:

```bash
dotnet run <path/to/config.json>
```

The application will start monitoring the specified directory and will send an email with any new files added to that directory.

## Contributing

Contributions are welcome! To propose changes, please create a branch, make your changes, and submit a pull request.

1. Fork this repository.
2. Create a new branch: git checkout -b my-new-branch.
3. Make your changes.
4. Commit your changes: git commit -m 'Add some feature'.
5. Push to the branch: git push origin my-new-branch.
6. Submit a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
