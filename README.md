# User Management API

Final project for the Microsoft Back-End Development with .NET on Coursera. It is a simple ASP.NET Core Web API with CRUD endpoints for managing users.
One of the goals was to use the help of Microsof Copilot.

## Overview

This API allows you to perform the following operations:
- Create a new user
- Retrieve a list of all users
- Retrieve a specific user by ID
- Update an existing user
- Delete a user

## Features

- **Middleware for Error Handling**: Catches unhandled exceptions and returns consistent error responses in JSON format.
- **Logging Middleware**: Logs HTTP method, request path, and response status code for each request.
- **Authentication Middleware**: Validates tokens from incoming requests and allows access only to users with valid tokens.

## Copilot Assistance

This project was created with the help of Microsoft Copilot. Here are some examples of how Copilot assisted with the project:

1. **Scaffolding the API**: Copilot helped in setting up the initial structure of the ASP.NET Core Web API.
2. **Creating `requests.http`**: Copilot generated the `requests.http` file for testing the API endpoints.
3. **Fixing Bugs**: Copilot provided solutions for various bugs encountered during development.
4. **Adding Middleware**: Copilot assisted in adding middleware for error handling, logging, and authentication.
