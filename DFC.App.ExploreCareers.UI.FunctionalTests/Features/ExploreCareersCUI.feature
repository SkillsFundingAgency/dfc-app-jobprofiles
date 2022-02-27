Feature: ExploreCareersCUI
	As a citizen on the National Careers website 
	I want to view and search job profiles information for a particular job category

Scenario Outline: Job category links verification
	Given I am at the "Explore careers" page
	When I click on the <Job category> link
	Then I am taken to the <Job category> page
Examples: 
	| Job category                      |
	| Administration                    |
	| Animal care                       |
	| Business and finance              |
	| Computing, technology and digital |
	| Emergency and uniform services    |
	| Law and legal                     |
	| Manufacturing                     |
	| Science and research              |
	| Transport                         |

Scenario Outline: Search field autosuggest field population
	Given I navigate to the <page> page
	When I enter the search term <search term> in the search field
	Then I am able to select <auto suggest option> from the resultant auto suggest
	And <auto suggest option> is populated in the search field
Examples:
	| page            | search term | auto suggest option |
	| Explore careers | nur         | Nursing associate   |
	| Explore careers | pi          | Aircraft pilot      |
	| Explore careers | la          | Lawyer              |
	| Job profiles    | nur         | Nursing associate   |
	| Job profiles    | pi          | Aircraft pilot      |
	| Job profiles    | la          | Lawyer              |
	| Search results  | nur         | Nursing associate   |
	| Search results  | pi          | Aircraft pilot      |
	| Search results  | la          | Lawyer              |

Scenario Outline: Search term not found
	Given I navigate to the <page> page
	When I enter the non existent search term <search term> in the search field
	And I click the search button
	Then I get the message "0 results found - try again using a different job title" in the search results page
	
Examples:
	| page            | search term | 
	| Explore careers | aaa         | 
	| Job profiles    | sss         | 
	| Search results  | zzz         | 

Scenario Outline: Auto suggesting search terms on search term misspelling
	Given I navigate to the <page> page
	When I enter the search term <search term> in the search field
	And I click the search button
	Then I am taken to the search results page with the message Did you mean <suggested search term> displayed
	And the message "0 results found - try again using a different job title" displayed below it
	
Examples:
	| page            | search term    | suggested search term |
	| Explore careers | nuurse         | nurse                 |
	| Job profiles    | lawwwyer       | lawyer                |
	| Search results  | phlebotomizzzt | phlebotomist          |

Scenario Outline: Empty search fields
	Given I navigate to the <page> page
	And the search field is empty
	When I click the search button
	Then the page does not advance
	
Examples:
	| page            |  
	| Explore careers | 
	| Search results  | 

Scenario Outline: Reconciling search results with pages returned
	Given I navigate to the <page> page
	And I enter the search term <search term> in the search field
	When I click the search button
	Then the number of search results returned is commensurate with the number of search result pages
	
Examples:
	| page            | search term  | 
	| Explore careers | nurse        | 
	| Job profiles    | lawyer       | 
	| Search results  | phlebotomist | 

Scenario Outline: Search results on pressing the enter button
	Given I navigate to the <page> page
	And I enter the search term <search term> in the search field
	When I press the Enter button instead of clicking search
	Then the search results screen is displayed.
	
Examples:
	| page            | search term  |
	| Explore careers | nurse        |
	| Job profiles    | lawyer       |
	| Search results  | phlebotomist | 

Scenario: Verification that the Job category list is complete
	Given I am at the "Explore careers" page
	Then the Administration Job category is present on the page
	And the Animal care Job category is present on the page
	And the Beauty and wellbeing Job category is present on the page
	And the Business and finance Job category is present on the page
	And the Computing, technology and digital Job category is present on the page
	And the Construction and trades Job category is present on the page
	And the Creative and media Job category is present on the page
	And the Delivery and storage Job category is present on the page
	And the Emergency and uniform services Job category is present on the page
	And the Engineering and maintenance Job category is present on the page
	And the Environment and land Government services Job category is present on the page
	And the Government services Job category is present on the page
	And the Healthcare Job category is present on the page
	And the Home services Job category is present on the page
	And the Hospitality and food Job category is present on the page
	And the Law and legal Job category is present on the page
	And the Managerial Job category is present on the page
	And the Manufacturing Job category is present on the page
	And the Retail and sales Job category is present on the page
	And the Science and research Job category is present on the page
	And the Social care Job category is present on the page
	And the Sports and leisure Job category is present on the page
	And the Teaching and education Job category is present on the page
	And the Transport Job category is present on the page
	And the Travel and tourism Job category is present on the page

Scenario: Links verifications for Other job categories side section
	Given I am at the "Job categories" web page for Administration
	When I click on the <Job category> link
	Then I am taken to the <Job category> page

Examples:
	| Job category                |
	| Beauty and wellbeing        |
	| Construction and trades     |
	| Creative and media          |
	| Delivery and storage        |
	| Engineering and maintenance |
	| Environment and land        |
	| Government services         |
	| Healthcare                  |
