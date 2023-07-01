Feature: Wallet service tests

As an end user 
In order to use store
I want to create account


Scenario: Get balance new inactive user
	Given New user is created
	When Get balance
	Then Get user balance response code is Internal Server Error
	And Get user balance response body is error message

Scenario: Get balance for a non existing user
	Given A non existing user
	When Get balance
	Then Get user balance response code is Internal Server Error
	And Get user balance response body is error message
	
Scenario: Get balance for active user with no transactions
	Given New user is created and activated
	When Change user IsActive status to 'true'
	And Get balance
	Then Get user balance is expected 0





Scenario Outline: Get balance for an user with valid transactions
	Given New user is created and activated
	When User is charged <ChargeAmount>
	And Get balance
	Then Get user balance is expected <ExpectedBalance>

	Examples: 
	| ChargeAmount | ExpectedBalance |
	| 10           | 10.0            |
	| 0.01         | 0.01            |
	| 9999999.99   | 9999999.99      |
	| 10000000     | 10000000.0      |


Scenario: Get balance for an user with an invalid Non Sufficient funds charge is made
	Given New user is created and activated
	When User is charged -10
	And Get balance
	Then Get user balance is expected 0
	And Get user balance response code is OK

Scenario: Get balance for an user with multiple transactions charged
	Given New user is created and activated
	When User is charged multipleChargesTable
	And Get balance
	Then Get user balance is expected [Decimal]
	And Get user balance response code is [string]

Scenario: Get balance multiple transactions resulting in Non sufficient funds - Balance is last OK charge
	Given New user is created and activated
	When User is charged multipleChargesTable
	And Get balance
	Then Get user balance is expected [Decimal]
	And Get user balance response code is [string]

Scenario: Get balance for user with valid transaction then is changed to inactive status
	Given New user is created and activated
	When User is charged 300
	And Change user IsActive status to 'false'
	And Get balance
	Then Get user balance response code is Internal Server Error
	And Get user balance response body is error message

Scenario: Get balance for user with no balance and negative charge is made
	Given New user is created and activated
	When User is charged -300
	And Get balance
	Then Get user balance is expected 0
	And Get user balance response code is OK 

Scenario Outline: Charge an user
	Given New user is created and activated
	When User is charged <ChargeAmount>
	Then Charge response code is <Response Code>
	And Charge response body is <Body GUID>

	Examples: 
	| ChargeAmount | Response Code			| Body GUID |
	| 10           | OK						| GUID      |
	| -0.01        | Internal Server Error  | Empty GUID|
	| 23450.65     | OK				        | GUID      |
	| -10000000    | Internal Server Error  | Empty GUID|

Scenario: Charge inactive user
	Given New user is created
	When User is charged 10
	Then Charge response code is Internal Server Error
	And Charge response content is not active user

Scenario: Charge non existing user

Scenario: Charge and exceed maximum balance

Scenario Outline: Charge with two digits after decimal
	Given New user is created and activated
	When User is charged <Charge amount>
	Then Charge response code is Internal Server Error
	And Charge response body is Empty GUID
	And Charge response content is precision error 

	Examples: 
	| Charge amount |
	| 0.001         |
	| 210.011       |

Scenario: Charge amount zero
	Given New user is created and activated
	When User is charged 0
	Then Charge response code is [string]
	And Charge response body is [string]
	And Charge response content is incorrect amount error


	


	