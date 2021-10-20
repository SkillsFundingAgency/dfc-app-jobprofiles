Feature: JobGroups
As a citizen when i am on the Job profiles page
I want to view Job trend data for the job profile

@JobProfile
Scenario: Navigate to Job groups page
	Given I navigate to the MP profile
	When I click the Explore job trends for this group link
	Then I am on the Job group: Elected officers and representatives page