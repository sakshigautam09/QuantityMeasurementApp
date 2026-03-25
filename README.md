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
