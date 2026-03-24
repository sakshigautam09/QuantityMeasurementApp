## Day — February 19, 2026 — UC1: Feet Equality Comparison
## Topic: Compare Two Lengths in Feet

## What I Learned
    * Created a sealed Feet class with encapsulation
    * Implemented constructor validation for invalid numeric values
    * Overrode Equals() method to compare two Feet objects
    * Understood object comparison vs reference comparison
    * Applied OOP concepts like encapsulation and immutability

## Task for the Day
    * Implement UC1: 1 Feet equals 1 Feet should return true
    * Validate numeric inputs
    * Follow clean coding standards
    * Push code to GitHub repository

## What’s Next
    * Extend comparison logic to different units (Feet & Inches)
    * Introduce conversion logic

<!-- ---------------------------------------------------- -->
## Day — February 20, 2026 — UC2: Feet & Inch Equality
## Topic: Unit Conversion (Feet to Inch)

## What I Learned
    * Created Inch class
    * Implemented conversion logic (1 Feet = 12 Inches)
    * Applied abstraction to hide conversion details
    * Used polymorphism for equality comparison
    * Strengthened understanding of domain modeling

## Task for the Day
    * Compare 1 Feet and 12 Inches
    * Implement conversion inside equality method
    * Maintain separation of concerns
    * Test different positive scenarios

## What’s Next
    * Generalize units using Enum or Unit abstraction
    * Improve extensibility

<!-- ---------------------------------------------- -->
## Day — February 21, 2026 — UC3: Refactor with LengthUnit Enum
## Topic: Generalized QuantityLength Model

## What I Learned
    * Introduced LengthUnit Enum
    * Created a unified QuantityLength class
    * Reduced code duplication
    * Implemented abstraction for unit conversion
    * Improved design using single responsibility principle

## Task for the Day
    * Refactor UC1 & UC2 into one generalized model
    * Accept dynamic user input instead of hardcoded values
    * Maintain clean architecture structure
    * Organize Core, Interfaces, and Console layers

## What’s Next
    * Implement additional units like Yard
    * Prepare for arithmetic operations

<!-- --------------------------------------------------------- -->
## Day — February 22, 2026 — UC4: Add Yard & More Units
## Topic: Extending Length Categories

## What I Learned
    * Extended system to support Yard unit
    * Applied Open-Closed Principle
    * Improved conversion strategy
    * Designed scalable structure for future categories
    * Strengthened understanding of extensible architecture

## Task for the Day
    * Implement Yard comparison
    * Ensure all units convert to a base unit internally
    * Validate equality across multiple units
    * Commit and push updated structure

## What’s Next
    * Implement arithmetic operations (Addition)
    * Handle result unit normalization

<!-- ------------------------------------------------------- -->
## Day — February 23, 2026 — UC5 & UC6: Unit Conversion + Addition of Two Length Units
## Topic: Conversion & Arithmetic Operations

## What I Learned
    * UC5: Unit-to-Unit Conversion
        * Implemented conversion from one length unit to another
        * Converted via base unit internally
        * Designed flexible conversion mechanism
        * Improved abstraction and reusability

    * UC6: Addition of Two Length Units
        * Implemented addition between two length units of same category
        * Converted both units to base unit before addition
        * Returned result in first operand’s unit
        * Ensured unit-safe arithmetic operations
        * Applied SOLID principles

## Task for the Day
    * Implement conversion feature (e.g., Feet → Inch, Yard → Feet)
    * Implement addition logic
    * Test multiple unit combinations
    * Maintain separation between UI and business logic
    * Push final implementation

## What’s Next
    * Extend application to Weight and Volume categories
    * Introduce unit testing
    * Prepare ASP.NET API integration

<!-- ----------------------------------------------------- -->
## Day — February 24, 2026 — UC7: Subtraction of Two Length Units
## Topic: Arithmetic Operations with Units

## What I Learned
    * Implemented subtraction between two length units
    * Converted both operands to a base unit before performing subtraction
    * Returned result in the first operand’s unit
    * Ensured unit compatibility before performing arithmetic
    * Improved error handling for invalid operations

## Task for the Day
    * Implement subtraction logic for length units
    * Ensure base unit conversion before calculation
    * Validate unit category before subtraction
    * Maintain clean separation between UI and logic layers
    * Push final implementation

## What’s Next
    * Extend arithmetic operations to other measurement categories
    * Improve code structure for reuse across units

<!-- ---------------------------------------------------------- -->
## Day — February 25, 2026 — UC8: Division Operation Between Units
## Topic: Ratio & Division Operations

## What I Learned
    * Implemented division operation between two compatible units
    * Converted both values to base unit before division
    * Returned scalar ratio value as output
    * Handled divide-by-zero scenarios
    * Ensured strong validation before operation

## Task for the day
    * Implement division feature
    * Validate unit category before performing division
    * Handle divide-by-zero exceptions
    * Test with different length unit combinations
    * Push final implementation

## What’s Next
    * Extend system to support multiple measurement categories

<!-- -------------------------------------------------- -->
## Day — February 26, 2026 — UC9: Weight Measurement Support
## Topic: Extending Measurement Categories

## What I Learned
    * Added support for weight measurements
    * Implemented units such as Gram, Kilogram, and Tonne
    * Designed reusable measurable unit abstraction
    * Ensured operations work similarly across categories
    * Improved modularity using interfaces

## Task for the Day
    * Implement weight unit classes
    * Enable conversion and arithmetic for weight units
    * Test conversion accuracy
    * Maintain reusable architecture

## What’s Next
    * Extend support to Volume measurements

<!-- ----------------------------------------------------- -->
## Day — February 27, 2026 — UC10: Volume Measurement Support
## Topic: Extending Measurement Categories

## What I Learned
    * Implemented volume units such as Liter, Milliliter, and Gallon
    * Applied same conversion strategy using base units
    * Ensured arithmetic operations work for volume
    * Strengthened abstraction using measurable interfaces

## Task for the Day
    * Implement measurable units for volume
    * Add conversion logic for volume
    * Validate operations for compatibility
    * Test multiple scenarios

## What’s Next
    * Introduce Temperature measurements with special conversion rules

<!-- -------------------------------------------------------- -->
## Day — February 28, 2026 — UC11: Temperature Measurement Support
## Topic: Non-linear Unit Conversion

## What I Learned
    * Implemented temperature units such as Celsius, Fahrenheit, and Kelvin
    * Learned that temperature conversions are not simple multipliers
    * Designed a dedicated temperature service
    * Handled temperature comparison logic correctly
    * Prevented invalid arithmetic operations on temperature

## Task for the Day
    * Implement temperature conversion formulas
    * Add temperature comparison logic
    * Prevent invalid operations like adding temperatures
    * Test different temperature conversions

## What’s Next
    * Improve architecture and refactor code for maintainability

<!-- -------------------------------------------------------- -->
## Day — March 1, 2026 — UC12: Refactoring Using SOLID Principles
## Topic: Clean Architecture & Maintainability

## What I Learned
    * Applied SOLID principles to improve code design
    * Introduced interfaces for service and repository layers
    * Reduced tight coupling between components
    * Improved extensibility of the measurement system

## Task for the Day
    * Refactor existing codebase
    * Introduce service interfaces
    * Improve separation of responsibilities
    * Ensure backward compatibility with previous features

## What’s Next
    * Implement DTO layer for API communication

<!-- -------------------------------------------------------- -->
## Day — March 3, 2026 — UC13: DTO Integration
## Topic: Data Transfer Objects

## What I Learned
    * Introduced DTOs to transfer data between layers
    * Separated internal models from API communication structures
    * Improved maintainability and scalability
    * Learned importance of DTO mapping

## Task for the Day
    * Create QuantityDTO structure
    * Implement mapping between DTO and Model
    * Ensure service layer works with DTOs
    * Test conversion and arithmetic operations using DTOs

## What’s Next
    * Add persistence layer to store operation history

<!-- ------------------------------------------------- -->
## Day — March 6, 2026 — UC14: Repository Layer Implementation
## Topic: Data Persistence

## What I Learned
    * Implemented repository layer for database interaction
    * Used repository pattern to abstract data access
    * Ensured business logic does not directly access database
    * Improved maintainability and testability

## Task for the Day
    * Create repository interface and implementation
    * Implement save operation for measurement history
    * Maintain separation between business and data layers
    * Push final implementation

## What’s Next
    * Integrate database using SQL Server

<!-- ------------------------------------------------------- -->
## Day — March 10, 2026 — UC15: ASP.NET API Integration
## Topic: Web API Development

## What I Learned
    * Integrated the measurement system with ASP.NET Web API
    * Exposed endpoints for operations like Convert, Compare, Add, Subtract
    * Learned API routing and controller structure
    * Implemented dependency injection for services

## Task for the Day
    * Create API controllers
    * Implement endpoints for measurement operations
    * Configure dependency injection
    * Test endpoints using Swagger

## What’s Next
    * Integrate database logging and complete full system flow

<!-- ------------------------------------------------------ -->
## Day — March 15, 2026 — UC16: Database Integration with SQL Server
## Topic: Persistence & Operation History

## What I Learned
    * Integrated application with SQL Server database
    * Created tables to store measurement operations
    * Implemented database logging for each operation
    * Used repository layer to save operation history
    * Learned how to manage database interactions using ADO.NET

## Task for the Day
    * Create database schema and tables
    * Implement repository save methods
    * Store operation results and metadata
    * Test full application workflow with database persistence

## What’s Next
    * Improve test coverage with integration tests
    * Optimize architecture for scalability
    * Prepare project documentation and deployment

<!-- ---------------------------------------------------- -->
## Day — March 16, 2026 — UC-16: Database Testing
## Topic: Integration Testing

## What I Learned
    * Learned how to validate complete database workflows using integration testing
    * Understood how data flows from service layer to repository and SQL Server
    * Gained clarity on handling edge cases and ensuring data consistency

## Task for the Day
    * Tested CRUD operations with SQL Server
    * Verified operation history is stored correctly with metadata
    * Checked data accuracy directly from the database

## What’s Next
    * Optimize database queries and improve repository performance
    * Refactor code for better maintainability

<!-- --------------------------------------------------- -->
## Day — March 17, 2026 — UC-16: Optimization
## Topic: Performance & Refactoring

## What I Learned
    * Learned basics of query optimization and improving execution efficiency
    * Understood importance of clean and maintainable repository code
    * Explored ways to reduce redundancy and improve readability

## Task for the Day
    * Refactored repository methods for better structure
    * Optimized queries for faster data operations
    * Cleaned up redundant or complex code

## What’s Next
    * Start working on security implementation (UC-17)
    * Study cryptographic concepts

<!-- --------------------------------------------------- -->
## Day — March 18, 2026 — UC-17: Security Fundamentals
## Topic: Cryptography Basics

## What I Learned
    * Understood difference between hashing and encryption
    * Learned why passwords must be hashed and not stored in plain text
    * Got overview of AES encryption and secure data handling

## Task for the Day
    * Studied BCrypt and AES concepts
    * Planned how to integrate security into application
    * Identified sensitive fields requiring protection

## What’s Next
    * Implement BCrypt password hashing
    * Begin coding security services

<!-- ------------------------------------------------- -->
## Day — March 19, 2026 — UC-17: BCrypt Implementation
## Topic: Password Security

## What I Learned
    * Learned how BCrypt generates salt and secure password hashes
    * Understood one-way hashing and password verification
    * Explored secure storage of user credentials

## Task for the Day
    * Implemented password hashing using BCrypt
    * Added password verification functionality
    * Tested hashing with different inputs

## What’s Next
    * Implement AES encryption for sensitive data
    * Strengthen security layer

<!-- ------------------------------------------------ -->
## Day — March 20, 2026 — UC-17: AES Encryption
## Topic: Data Encryption

## What I Learned
    * Learned AES-256 encryption and its use for protecting sensitive data
    * Understood role of encryption keys and IV
    * Differentiated encryption from hashing

## Task for the Day
    * Implemented encryption and decryption service
    * Encrypted sensitive fields like email and results
    * Verified correct decryption of data

## What’s Next
    * Implement JWT authentication
    * Learn token-based security

<!-- ---------------------------------------------------- -->
## Day — March 21, 2026 — UC-17: JWT Implementation
## Topic: Token-Based Authentication

## What I Learned
    * Understood structure of JWT (Header, Payload, Signature)
    * Learned how tokens enable secure authentication
    * Explored token expiry and validation

## Task for the Day
    * Implemented JWT token generation
    * Configured issuer, audience, and expiry
    * Tested token generation and structure

## What’s Next
    * Integrate all security components together
    * Build complete secure flow

<!-- --------------------------------------------------- -->
## Day — March 22, 2026 — UC-17: Security Integration
## Topic: End-to-End Security Flow

## What I Learned
    * Learned how hashing, encryption, and authentication work together
    * Understood secure workflow for user operations
    * Explored best practices for data protection

## Task for the Day
    * Integrated BCrypt, AES, and JWT
    * Implemented secure register/login flow
    * Verified secure handling of user data

## What’s Next
    * Write test cases for all security features
    * Validate complete system

<!-- ---------------------------------------------------------- -->
## Day — March 23, 2026 — UC-17: Testing & Finalization
## Topic: Security Testing & Validation

## What I Learned
    * Learned how to test hashing, encryption, and token generation
    * Understood importance of validating full security workflow
    * Explored strategies for reliable testing

## Task for the Day
    * Wrote tests for BCrypt, AES, and JWT
    * Verified encryption/decryption and authentication flow
    * Ensured all components work correctly together

## What’s Next
    * Prepare project documentation
    * Optimize for production and scalability