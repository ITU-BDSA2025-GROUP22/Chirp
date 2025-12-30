---
title: "Chirp!"
subtitle: "Project Report"
author:
  - "Tore Engberg"
  - "Timothy Lubowa"
  - "Muneeb Hussain"
  - "Nikolai Li"
  - "Jakob Juhl"
date: "2026-01-02"
toc: true
---


# Design and Architecture of _Chirp!

## Domain model


![This figure shows how our data is structured. It links the core entities (Author, Cheep, and Like) with the ASP.NET Identity system used for authentication.](Diagram/domain-diagram.png)

The domain model diagram illustrates the data structure of the Chirp application. It focuses on the relationships between the core entities: Author, Cheep, and Like, showing how users create messages and interact with them. Additionally, the diagram demonstrates the integration with ASP.NET Identity, which handles the GitHub authentication. This connection links the login data to the Author profile, ensuring that user identity is managed efficiently while keeping security separate from the application's logic.

## Architecture — In the small

![This figure shows how our Model is structured. Its structured after the union architecture model](Diagram/UnionA.png)

Thus model illustrates the architecture of our chirp application which is constructed after the principals of onion architecture. The system is devided by three layers. Chirp.Core, Chirp.Web, Chirp.Infrastructure. Det outer most layer Chirp.Web consists of the visual that the applications presents. The middle layer Chirp.Infrastructure is responsible for the data flow, services and integration with the database, and the inner most layer Chirp.Core is responsible for the domain models and logik for the application. Each layer is independent of its outer layers
## Architecture of deployed application
*Illustrate the architecture of your deployed application. Remember, you developed a client-server application. Illustrate the server component and to where it is deployed, illustrate a client component, and show how these communicate with each other.*
## User activities
*Illustrate typical scenarios of a user journey through your Chirp! application. That is, start illustrating the first page that is presented to a non-authorized user, illustrate what a non-authorized user can do with your Chirp! application, and finally illustrate what a user can do after authentication.*

*Make sure that the illustrations are in line with the actual behavior of your application.*
## Sequence of functionality/calls through _Chirp!
*With a UML sequence diagram, illustrate the flow of messages and data through your Chirp! application. Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.*

*Make sure that your illustration is complete. That is, likely for many of you there will be different kinds of "calls" and responses. Some HTTP calls and responses, some calls and responses in C# and likely some more. (Note the previous sentence is vague on purpose. I want that you create a complete illustration.)*
# Process

## Build, test, release, and deployment
*Illustrate with a UML activity diagram how your Chirp! applications are build, tested, released, and deployed. That is, illustrate the flow of activities in your respective GitHub Actions workflows.*

*Describe the illustration briefly, i.e., how your application is built, tested, released, and deployed.*
## Team work
*Show a screenshot of your project board right before hand-in. Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.*

*Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the main branch of your repository.*

### Project board
### Activity flow

## How to make _Chirp!_ work locally
Requirements to run _Chirp!_ locally:
* .NET 8.0.x SDK
* .NETCore 8.0.x Runtime
* AspNetCore 8.0.x Runtime

### Download and run
1) .NET 8 can be downloaded from the release page [releases page](https://github.com/ITU-BDSA23-GROUP13/Chirp/releases).
2) Extract the zip file.
3) Open the extracted folder in the terminal.
4) Run executable.

### Building from source
1) Clone the repository
```bash
git clone (https://github.com/ITU-BDSA2025-GROUP22/Chirp.git)
```
2) Change directory to the repository
```bash
cd Chirp
```
3) Run the project
```bash
dotnet run src/Chirp.Web
```
## How to run test suite locally
After completing the previous steps you can preceed by with the following
```bash
dotnet test
```

This will run every test using in-memory Sqlite databases.
# Ethics

## License
We use the MIT license
## LLMs, ChatGPT, Gemini, and others
ChatGPT and Gemini was used minimally, and no production code was copied directly from them. It was mainly used to clarify concepts and interpret error messages. Github copilot was also used for the same purpose of explaining error messages related to workflows. Stackoverflow and the official documentation often proved more efficient and reliable than AI suggestions since they have a very narrow context of the project.








