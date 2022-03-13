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

Scenario Outline: Misspelt search terms auto suggestion link verification
	Given I navigate to the <page> page
	And I enter the search term <search term> in the search field
	And I click the search button
	And I am taken to the search results page with the message Did you mean <suggested search term> displayed
	When I click the link in the message
	Then the url bears the suggested search term <suggested search term> as its parameter	
Examples:
	| page            | search term | suggested search term |
	| Explore careers | nuurse      | nurse                 |
	| Explore careers | lawwwyer    | lawyer                |
	| Search results  | mechannic   | mechanic              |
	| Search results  | astronautt  | astronaut             |

Scenario Outline: Empty search fields
	Given I navigate to the <page> page
	And the search field is empty
	When I click the search button
	Then the page does not advance	
Examples:
	| page            |  
	| Explore careers | 
	| Search results  | 

Scenario Outline: Reconciling search results count with number of pages returned
	Given I navigate to the <page> page
	And I enter the search term <search term> in the search field
	And I click the search button
	And I retrieve the number of search results
	When I work out the number of result pages from the number of search results returned
	Then the number of search results returned is commensurate with the number of search result pages
	And the Next button is no longer present on the final page	
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

Scenario: Links verifications for Other job categories side section
	Given I am at the "Job categories" web page for <Job category>
	When I click on the <Link to test> link
	Then I am taken to the <Link to test> page
Examples:
	| Job category                      | Link to test                      |
	| Administration                    | Travel and tourism                |
	| Animal care                       | Transport                         |
	| Beauty and wellbeing              | Teaching and education            |
	| Business and finance              | Sports and leisure                |
	| Computing, technology and digital | Social care                       |
	| Construction and trades           | Science and research              |
	| Creative and media                | Retail and sales                  |
	| Delivery and storage              | Manufacturing                     |
	| Emergency and uniform services    | Managerial                        |
	| Engineering and maintenance       | Law and legal                     |
	| Environment and land              | Hospitality and food              |
	| Government services               | Home services                     |
	| Healthcare                        | Government services               |
	| Home services                     | Healthcare                        |
	| Hospitality and food              | Environment and land              |
	| Law and legal                     | Engineering and maintenance       |
	| Managerial                        | Emergency and uniform services    |
	| Manufacturing                     | Delivery and storage              |
	| Retail and sales                  | Creative and media                |
	| Science and research              | Construction and trades           |
	| Social care                       | Computing, technology and digital |
	| Sports and leisure                | Business and finance              |
	| Teaching and education            | Beauty and wellbeing              |
	| Transport                         | Animal care                       |
	| Travel and tourism                | Administration                    |

Scenario: Unqualified link verification for Other job categories side section
	Given I am at the "Job categories" web page for <Job category>
	Then the <Job category> link is not present amongst the links beneath the Other job categories side section
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

Scenario: Verify Job category list
Given I am at the "Explore careers" page
And I check the list displayed below against the list of Job categories displayed on that page
	| Job category                      |
	| Administration                    |
	| Animal care                       |
	| Beauty and wellbeing              |
	| Business and finance              |
	| Computing, technology and digital |
	| Construction and trades           |
	| Creative and media                |
	| Delivery and storage              |
	| Emergency and uniform services    |
	| Engineering and maintenance       |
	| Environment and land              |
	| Government services               |
	| Healthcare                        |
	| Home services                     |
	| Hospitality and food              |
	| Law and legal                     |
	| Managerial                        |
	| Manufacturing                     |
	| Retail and sales                  |
	| Science and research              |
	| Social care                       |
	| Sports and leisure                |
	| Teaching and education            |
	| Transport                         |
	| Travel and tourism                |
Then both lists are the same

Scenario Outline: Verify Job profiles distinct count
	Given I am at the "Job categories" web page for <Job category>
	When I check the job profiles list
	Then none of the job profiles occur more than once
Examples:
	| Job category                      |
	| Administration                    |
	| Animal care                       |
	| Beauty and wellbeing              |
	| Business and finance              |
	| Computing, technology and digital |
	| Construction and trades           |
	| Creative and media                |
	| Delivery and storage              |
	| Emergency and uniform services    |
	| Engineering and maintenance       |
	| Environment and land              |
	| Government services               |
	| Healthcare                        |
	| Home services                     |
	| Hospitality and food              |
	| Law and legal                     |
	| Managerial                        |
	| Manufacturing                     |
	| Retail and sales                  |
	| Science and research              |
	| Social care                       |
	| Sports and leisure                |
	| Teaching and education            |
	| Transport                         |
	| Travel and tourism                |

Scenario Outline: Job profiles links verification
	Given I am at the "Job categories" web page for <Job category>
	When I click the link for the <Link position> Job profile under that Job category
	Then I am taken profile details page for that Job profile
Examples:
	| Job category                      | Link position |
	| Administration                    | first         |
	| Animal care                       | second        |
	| Beauty and wellbeing              | third         |
	| Business and finance              | fourth        |
	| Computing, technology and digital | fifth         |

Scenario Outline: Alphabetical ordering
	Given I navigate to the <Page> page for <Job category>
	When I examine the <List to examine> list
	Then the list is in alphabetical order
Examples:
	| Page            | Job category   | List to examine      |
	| Explore careers |                | Job categories       |
	| Job categories  | Administration | Other job categories |
	| Job categories  | Administration | Job profiles         |

Scenario Outline: Non pagination verification for Job categories Administration page
	Given I navigate to the <Page> page for <Job category>
	When I examine the page
	Then the page contains no pagination
Examples:
	| Page            | Job category   |
	| Job categories  | Administration |

Scenario: Placeholder text verification for search field
	Given I navigate to the web page "Search results"
	Then the search results field placeholder text is "Enter a job title"

Scenario Outline: Number of results found verification
	Given I navigate to the <page> page
	And I enter the search term <search term> in the search field
	When I click the search button
	Then the number of results stated as found is the equal to the actual number of Job profiles listed thereunder	
Examples:
	| page            | search term    |
	| Explore careers | Aircraft pilot |
	| Search results  | Nurse          |
	| Explore careers | Carpenter      |
	| Search results  | Cartographer   |

Scenario Outline: Breadcrumb linking verification
	Given I navigate to the <Page> page for <Job category>
	When I click the Explore careers breadcrumb
	Then I am on the "Explore careers" page
Examples:
	| Page           | Job category   |
	| Job categories | Administration |
	| Search results |                | 