Feature: JobGroups

@JobProfile
Scenario: Navigate to Job groups page
	Given I navigate to the MP profile
	When I click the Explore job trends for this group link
	Then I am on the Job group: Elected officers and representatives page