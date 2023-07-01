Feature: User Service Tests

As an end user 
In order to use store
I want to create account

Scenario: Register new user with empty fields - status is 200 and UserId is greater than zero
	Given New user with empty fields is created
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register new user with Upper Case - status is 200 and UserId is greater than zero
	Given New user with Upper Case fields
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register new user with field is One Character - status is 200 and UserId is greater than zero
	Given New user with One Character fields
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register new user with fields lenght greater than 100 - status is 200 and UserId is greater than zero
	Given New user with Fields length greater than 100
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register new user with null fields - status is Internal Server Error
	Given New user with null fields
	Then Register new user response Status code is Internal Server Error

Scenario: Register new user with special characters - status is 200 and UserId is greater than zero
	Given New user with Special Character fields
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register new user with digit fields - status is 200 and UserId is greater than zero
	Given New user with digit fields
	Then Register new user response Status code is OK
	And Register new user response body is Id greater than zero

Scenario: Register three valid users - User Ids are auto incremented
	Given Three New Valid Users Created
	Then User Id is auto incremented for three registered users
	
Scenario: Register valid user and Id is auto incremented after user is deleted and second user created
	Given New user is created
	When User is deleted
	Given Second user is created
	Then User Id is auto incremented for two users

Scenario Outline: Get updated status for new user 
	Given New user is created with name '<FirstName>' and last name '<LastName>'
	When Change user IsActive status to '<ChangeStatusTo>'
	And Change user IsActive status to '<ChangeStatusTo2>'
	And Get user status
	Then Get user status response Status Code is '<StatusReponseCode>'
	And User status is '<ActualUserStatus>'

	Examples: 
	| FirstName | LastName | ChangeStatusTo | ChangeStatusTo2 | StatusReponseCode | ActualUserStatus |
	| Test      | TestLast | True           | True            | OK                | True             |
	| Test      | TestLast | True           | False           | OK                | False            |
	| Test      | TestLast | False          | False           | OK                | False            |
	| Test      | TestLast | False          | True            | OK                | True             |	


Scenario: Get user status for non existing user - status is Internal Server Error
	Given A non existing user
	When Get user status
    Then Get user status response Status Code is 500
    And Get user status response body is error message - NonExistingUser

Scenario: Get user status for new user - status is default false
	Given New user is created
	When Get user status
	Then User status is false

Scenario: Get user status for deleted user - status is Internal Server Error-Error Message
	Given New user is created
	When User is deleted
	And Get user status
	Then Get user status response Status Code is 500
	And Get user status response body is error message - NonExistingUser

Scenario: Set user status to non existing user - Response status is Internal Server Error
	Given A non existing user
	When Change user IsActive status to 'true'
	Then Set user status response Status Code is 'Internal Server Error'
	And Set user status response body is error message - NonExistingUser

Scenario Outline: Set user status
	Given New user is created
	When Change user IsActive status to '<ChangeStatusTo>'
	And Change user IsActive status to '<ChangeStatusTo2>'
	And Get user status
	Then Set user status response Status Code is '<ResponseStatusCode>'
	And User status is '<ActualUserStatus>'

	Examples: 
	| ChangeStatusTo | ChangeStatusTo2 | ResponseStatusCode | ActualUserStatus |
	| True           | True            | OK                 | True             |
	| False          | False           | OK                 | False            |


Scenario: Delete User for non active new created user - status is OK
	Given New user is created
	When User is deleted
	Then Delete User Response is OK

Scenario Outline: Delete User with different IsActive Status
	Given New user is created
	When Change user IsActive status to '<ChangeStatusTo>'
	And User is deleted
	Then Delete User Response is OK

	Examples: 
	| ChangeStatusTo |
	| True           |
	| False          |

Scenario: Delete non existing user - status is Internal Server Error and Error Message
	Given A non existing user
	When User is deleted
	Then Delete user response is Internal Server Error
	And Delete user response body is Error Message-NonExistingUser

Scenario: Delete an already deleted user - status is Internal Server Error and Error Message
	Given New user is created
	When User is deleted
	And User is deleted
	Then Delete user response is Internal Server Error
	And Delete user response body is Error Message-NonExistingUser


