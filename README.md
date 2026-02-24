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