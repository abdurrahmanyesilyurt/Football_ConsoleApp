# Football_ConsoleApp
```json
{
  "PlayerData": [
    {
      "Id": 0,                      // integer
      "TransferId": null,           // integer or null
      "PlayerId": null,             // integer or null
      "TeamId": null,               // integer or null
      "PositionId": null,           // integer or null
      "DetailedPositionId": null,   // integer or null
      "Start": "YYYY-MM-DD",        // string (date format)
      "End": "YYYY-MM-DD",          // string (date format) or null
      "Captain": false,             // boolean
      "JerseyNumber": null          // integer or null
    }
  ]
}

{
  "League": [
    {
      "Id": 0,                // integer
      "SportId": 0,           // integer or null
      "CountryId": 0,         // integer or null
      "Name": "",             // string
      "Active": false,        // boolean
      "ShortCode": "",        // string or null
      "ImagePath": "",        // string or null
      "Type": "",             // string or null
      "SubType": "",          // string or null
      "LastPlayedAt": "YYYY-MM-DD",  // string (date format) or null
      "Category": "",         // string or null
      "HasJerseys": false     // boolean
    }
  ]
}
```
## App
![image](https://github.com/user-attachments/assets/b39abbb0-b742-442c-a4a9-d2a99b64d0eb)

- ### Option 1 : 
Retrieves all team members from the API, saves them to the database, and then fetches and displays the data in the console.
- ### Option 2 :
Does Same thing as option1 but for League datas 
- ### Option 3 : 
Retrieves the league corresponding to the ID I provide.

## Test Cases : 
### ProcessDataAsync_Should_Add_New_Player

#### Purpose: 
This test case checks if a new player is correctly added to the database when it doesn't already exist in the database.
#### Process:
- Mocks the API response with player data that doesn't exist in the database.
- Verifies that the player is added and that changes are saved to the database.

### ProcessDataAsync_Should_Not_Add_Existing_Player

#### Purpose:
This test verifies that the service does not add a player if that player already exists in the database with the same ID and jersey number.
#### Process:
- Mocks the API response with player data that matches an existing record in the database.
- Ensures that no new record is added, and no database changes are saved.
ProcessDataAsync_Should_Add_Multiple_Players

#### Purpose:
This test case ensures that multiple new players are added to the database if they don't already exist.
#### Process:
- Mocks the API response with multiple player records.
- Verifies that all new players are added in a batch and that changes are saved.

### ProcessDataAsync_Should_Add_New_League

#### Purpose: 
This test ensures that a new league is correctly added to the database when it doesn't already exist.
#### Process:
- Mocks the API response with league data.
- Verifies that the league is added and that changes are saved.

### ProcessDataAsync_Should_Return_False_When_No_Data

#### Purpose:
This test checks the service's behavior when the API returns no data.
#### Process:
- Mocks an API response with an empty data array.
- Ensures that the method returns false, indicating that no data was processed.
### ProcessDataAsync_Should_Add_Log_On_Error

#### Purpose: 
This test case verifies that an error during the data processing is correctly logged.
#### Process:
- Simulates a database error when trying to add a league.
- Verifies that the error is caught, a log entry is created, and the method returns false.

### ProcessDataAsync_Should_Handle_Missing_League_Fields

#### Purpose:
This test ensures that the service can handle cases where some fields in the league data are missing.
#### Process:
- Mocks the API response with incomplete league data (missing SportId and CountryId).
- Verifies that the league is still added and that changes are saved.

### ProcessDataAsync_Should_Return_False_On_Invalid_Json

#### Purpose: 
This test checks how the service handles invalid JSON data.
#### Process:
- Mocks an API response with an invalid JSON structure.
- Ensures that the method returns false, indicating that the JSON could not be processed.

### ProcessDataAsync_Should_Return_False_When_League_Detail_Is_Null

#### Purpose:
This test verifies that the service correctly handles a null response for league details.
#### Process:
- Mocks an API response with a null value for the league data.
- Ensures that the method returns false.

### ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Player

#### Purpose:
This test case ensures that a player can be added, removed, and then added again without issues.
#### Process:
- Adds a player to the database.
- Removes the player from the database.
- Attempts to add the same player again.
- Verifies that all operations are executed correctly and that the player can be re-added after removal.

### ProcessDataAsync_Should_Add_Then_Remove_And_Add_Again_Multiple_Players

#### Purpose:
Similar to the previous test, but for multiple players. This ensures that multiple players can be added, removed, and re-added correctly.
#### Process:
- Adds multiple players to the database.
- Removes the players.
- Attempts to add the same players again.
- Verifies that all players are processed correctly in batch operations.



### Unsolved Problem : 

When I delete data from my local database while the program is running, a tracking error occurs. I have tried using  **AsNoTracking** and **Attach** methods to resolve the issue, but they did not work. If you have a solution, please feel free to contact me. Below is the log that was generated when this issue occurred:
![image](https://github.com/user-attachments/assets/f21c94e1-8e90-4657-896b-8f1966f6102c)
Error : The instance of entity type 'PlayerData' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.

The app when I did that : 

![image](https://github.com/user-attachments/assets/fae7686f-baad-47f8-8307-096626592f06)



