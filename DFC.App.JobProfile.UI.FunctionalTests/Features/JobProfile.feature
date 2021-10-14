Feature: JobProfile
As a citizen on the National Careers website 
I want to view job profiles information for a particular job

@JobProfile
@Smoke
Scenario: View a Job profile
	Given I navigate to the Nurse profile
	Then I am on the Nurse page

@JobProfile
@Smoke
Scenario: Selecting the Find a Course near you link takes you to the Find a Course product
	Given I navigate to the plumber profile
	When I expand all accordion sections
	And I click the Find courses near you link
	Then I am on the Find a course page

@JobProfile
Scenario: Apprenticeship are not displayed on Job Profile Page and correct message shown
	Given I navigate to the surgeon profile
	When I expand all accordion sections
	Then The appropriate message is displayed

@JobProfile
Scenario: Ensure 404 pages are displayed correctly when navigating to a page that doesn't exist
	Given I navigate to the profile-does-not-exist profile
	Then I am on the Page not found page
	When I click the Home link
	Then I am on the National Careers Service page

@JobProfile
Scenario: Viewing a Job Profile, the Breadcrumb is displayed
	Given I navigate to the dental-nurse profile
	When I click the Home: Explore careers link
	Then I am on the Explore careers page

@Jobprofile
Scenario: Valid Search on Job Profile Page
	Given I navigate to the Chef profile
	When I search for nurse under the JP search feature
	Then I am on the Search results for page

@JobProfile
@Smoke
Scenario: JP Survey - Answering YES to the survey
	Given I navigate to the careers-adviser profile
	When I click the Yes link
	Then the additional survey message is displayed for Yes response

@JobProfile
Scenario: JP Survey - Answering NO to the survey
	Given I navigate to the assistant-immigration-officer profile
	When I click the No link
	Then the additional survey message is displayed for No response
	When I click the Click here link
	Then I am on the Feedback Survey page
	When I enter the feedback National Careers Service Test Feedback
	Then I am on the Thanks page